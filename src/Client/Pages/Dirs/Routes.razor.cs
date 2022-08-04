using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class Routes
    {
        public Origin TransformOrigin { get; set; } = Origin.TopRight;
        public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

        [Inject] private IDirectoryManager DirManager { get; set; }

        //private bool _loaded = false;
        //private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit;

        private int tz;
        private bool dense;
        private bool matchCase;

        private int delay;
        private int duration;

        //private int _totalItems = 0;
        //private int _pageNumber = 1;
        private int _rowsPerPage;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            //openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;
        }

        private void RenewAsync()
        {
            //FilterReset();
            //await _mudTable.ReloadServerData();
        }

        private void AddRouteAsync()
        {
            //var result = await AddEditRouteAsync(new());
            _navigationManager.NavigateTo($"dirs/routes/route-card");
            //if (!result.Cancelled) { await _mudTable.ReloadServerData(); }
        }

        private async Task<DialogResult> AddEditRouteAsync(DocRouteModel route)
        {
            var parameters = new DialogParameters()
            {
                {
                    nameof(RouteDialog._route),
                    new AddEditRouteCommand
                    {
                    }
                }
            };

            var dialog = _dialogService.Show<RouteDialog>("", parameters); //, options
            return await dialog.Result;
        }
    }
}
