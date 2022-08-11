using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Client.Pages.Docs;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class Routes
    {
        public Origin TransformOrigin { get; set; } = Origin.TopRight;
        public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

        [Inject] private IDirectoryManager DirManager { get; set; }

        private MudTable<RouteModel> _mudTable;
        private IEnumerable<RouteModel> _pagedData;
        private RouteModel _route;
        private readonly List<RouteModel> _routes = new();


        private bool _loaded = false;
        private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit;

        private int tz;
        private bool dense;
        private bool matchCase;

        private int delay;
        private int duration;

        private int _totalItems = 0;
        private int _pageNumber = 1;
        private int _rowsPerPage;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            _pagedData = _routes;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            //openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;
        }

        private async Task RenewAsync()
        {
            //FilterReset();
            await _mudTable.ReloadServerData();
        }
        private async Task OnSearch(string text)
        {
            _searchString = text;
            await _mudTable.ReloadServerData();
        }

        private void OnToggledDense(bool toggled)
        {
            dense = toggled;
            _stateService.Dense = dense;
        }
        private async Task OnToggledMatchCase(bool toggled)
        {
            matchCase = toggled;
            _stateService.MatchCase = matchCase;
            await _mudTable.ReloadServerData();
        }

        private void OnRowClickAsync() => _navigationManager.NavigateTo($"dirs/routes/route-card/{_route.Id}");

        private async Task<TableData<RouteModel>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetRoutesAsync(state);

            if (!_loaded) { _loaded = true; }
            StateHasChanged();

            return new TableData<RouteModel> { TotalItems = 0, Items = _pagedData };
        }

        private async Task GetRoutesAsync(TableState state = null)
        {
            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            var request = new GetPagedRoutesRequest()
            {
                PageSize = _rowsPerPage,
                PageNumber = _pageNumber,
                SearchString = _searchString,

                OrderBy = orderings,
                MatchCase = matchCase
            };

            var response = await DirManager.GetRoutesAsync(request);

            if (!response.Succeeded)
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;

            _routes.Clear();
            var data = response.Data;
            data.ForEach(r => _routes.Add(NewRoute(r)));

            _pagedData = _routes;

            _loaded = true;
        }

        private static string InactiveRowClassFunc(RouteModel r, int _) => r.IsActive ? string.Empty : "inactive";

        private void AddRouteAsync()
        {
            //var result = await AddEditRouteAsync(new());
            _navigationManager.NavigateTo($"dirs/routes/route-card");
            //if (!result.Cancelled) { await _mudTable.ReloadServerData(); }
        }

        private static RouteModel NewRoute(RoutesResponse r)
        {
            return new()
            {
                Id = r.Id,
                Number = r.Number,
                Name = r.Name,
                Description = r.Description,

                ForUserRole = r.ForUserRole,
                IsPackage = r.IsPackage,
                CalcHash = r.CalcHash,
                AttachedSign = r.AttachedSign,

                IsActive = r.IsActive,
                AllowRevocation = r.AllowRevocation,
                UseVersioning = r.UseVersioning,
                HasDetails = r.HasDetails
            };
        }
    }
}
