using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Filters;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class Orgs
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        private MudTable<OrgsResponse> _mudTable;
        private IEnumerable<OrgsResponse> _pagedData;
        private OrgsResponse _org ;
        private readonly List<OrgsResponse> _orgList = new();

        private bool openFilter = true;
        private readonly OrgFilter Filter = new();
        private readonly OrgFilter FilterDefault = new();

        private bool _loaded;
        private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit = false;

        private int tz;
        private bool dense;
        private bool matchCase;

        private int delay;
        private int duration;

        private int _totalItems;
        private int _pageNumber = 1;
        private int _rowsPerPage;

        private MudDatePicker _createOnFrom;
        private MudDatePicker _createOnTo;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            openFilter = _stateService.FilterIsOpen;

            //await GetOrgsAsync();
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
        private void ToggleFilter()
        {
            openFilter = !openFilter;
            _stateService.FilterIsOpen = openFilter;
        }

        private async Task ApplyFilter()
        {
            await _mudTable.ReloadServerData();
        }
        private async Task RenewAsync()
        {
            FilterReset();
            await _mudTable.ReloadServerData();
            //await GetAgreementsAsync(AgreementStates.AllActive);
        }
        private async Task OnSearch(string text)
        {
            _searchString = text;
            await _mudTable.ReloadServerData();
        }

        private async Task<TableData<OrgsResponse>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetOrgsAsync(state);

            return new TableData<OrgsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }
        private async Task GetOrgsAsync(TableState state = null)
        {
            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            PaginatedResult<OrgsResponse> response;

            if (Filter.IsActive)
            {
                List<OrgTypes> types = new();
                if (!Filter.TypeMO && !Filter.TypeSMO && !Filter.TypeFund)
                {
                    types.AddRange(new List<OrgTypes> { OrgTypes.MO, OrgTypes.SMO, OrgTypes.Fund });
                }
                else
                {
                    if (Filter.TypeMO) { types.Add(OrgTypes.MO); }
                    if (Filter.TypeSMO) { types.Add(OrgTypes.SMO); }
                    if (Filter.TypeFund) { types.Add(OrgTypes.Fund); }
                }

                List<OrgStates> states = new();
                if (Filter.StateActive || Filter.StateInactive || Filter.StateOnSubmit || Filter.StateBlocked)
                {
                    if (Filter.StateActive) { states.Add(OrgStates.Active); }
                    if (Filter.StateInactive) { states.Add(OrgStates.Inactive); }
                    if (Filter.StateOnSubmit) { states.Add(OrgStates.OnSubmit); }
                    if (Filter.StateBlocked) { states.Add(OrgStates.Blocked); }
                }
                else
                {
                    states.AddRange(new List<OrgStates> { OrgStates.Active, OrgStates.Inactive, OrgStates.OnSubmit, OrgStates.Blocked });
                }

                var filter = new SearchOrgsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,

                    OrderBy = orderings,
                    OrgTypes = types.ToArray(),
                    OrgStates = states.ToArray(),

                    SearchString = _searchString,
                    MatchCase = matchCase,

                    TextOrgId = Filter.TextOrgId,
                    TextInnLe = Filter.TextInnLe,
                    TextOgrn = Filter.TextOgrn,

                    TextName = Filter.TextName,
                    TextShortName = Filter.TextShortName,

                    TextEmail = Filter.TextEmail,
                    TextPhone = Filter.TextPhone,

                    CreateOnFrom = Filter.CreateOnFrom,
                    CreateOnTo = Filter.CreateOnTo
                };

                await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Orgs Filter");
                response = await AdmManager.SearchOrgsAsync(filter);
                await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Orgs Search Response");
            }
            else
            {
                var request = new GetPagedOrgsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    MatchCase = matchCase
                };

                response = await AdmManager.GetPagedOrgsAsync(request);
            }

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;

            _orgList.Clear();
            var data = response.Data;
            data.ToList().ForEach(o =>
                {
                    o.CreatedOn = o.CreatedOn.AddHours(tz);
                    _orgList.Add(o);
                });

            _pagedData = _orgList;
            _loaded = true;
        }

        private void AddOrg() => AddEdigOrg(0);
        private void OnRowClick() => AddEdigOrg(_org.Id);
        private void AddEdigOrg(int id) => _navigationManager.NavigateTo($"/admin/orgs/org-card/{id}");

        private async Task Reset()
        {
            _org = new();
            await RenewAsync();
        }

        private void OnText()
        {
            Filter.ChangedText = (Filter.TextOrgId != FilterDefault.TextOrgId
                || Filter.TextInnLe != FilterDefault.TextInnLe
                || Filter.TextOgrn != FilterDefault.TextOgrn
                || Filter.TextName != FilterDefault.TextName
                || Filter.TextShortName != FilterDefault.TextShortName
                || Filter.TextEmail != FilterDefault.TextEmail
                || Filter.TextPhone != FilterDefault.TextPhone);

            FilterIsActive();
        }
        private void OnState(string state)
        {
            if (state == "Active") { Filter.StateActive = !Filter.StateActive; }
            else if (state == "Inactive") { Filter.StateInactive = !Filter.StateInactive; }
            else if (state == "On Submit") { Filter.StateOnSubmit = !Filter.StateOnSubmit; }
            else if (state == "Block") { Filter.StateBlocked = !Filter.StateBlocked; }

            Filter.ChangedStates = (Filter.StateActive != FilterDefault.StateActive
                || Filter.StateInactive != FilterDefault.StateInactive
                || Filter.StateOnSubmit != FilterDefault.StateOnSubmit
                || Filter.StateBlocked != FilterDefault.StateBlocked);

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnType(string type)
        {
            if (type == "MO") { Filter.TypeMO = !Filter.TypeMO; }
            else if (type == "SMO") { Filter.TypeSMO = !Filter.TypeSMO; }
            else if (type == "Fund") { Filter.TypeFund = !Filter.TypeFund; }

            Filter.ChangedTypes = (Filter.TypeMO != FilterDefault.TypeMO
                || Filter.TypeSMO != FilterDefault.TypeSMO
                || Filter.TypeFund != FilterDefault.TypeFund);

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnCreateOn(string c, DateTime? d)
        {
            if (c == "From") { Filter.CreateOnFrom = d; }
            else if (c == "To") { Filter.CreateOnTo = d; }

            Filter.ChangedCreateOn = Filter.CreateOnFrom != FilterDefault.CreateOnFrom ||
                Filter.CreateOnTo != FilterDefault.CreateOnTo;

            FilterIsActive();
        }

        private void FilterIsActive()
        {
            Filter.IsActive = Filter.ChangedText || Filter.ChangedStates || Filter.ChangedTypes || Filter.ChangedCreateOn;
        }
        //private void FilterIsEmpty()
        //{
        //    Filter.IsEmpty = !(Filter.StateActive || Filter.StateInactive || Filter.StateOnSubmit || Filter.StateBlocked)
        //        || !(Filter.TypeMO || Filter.TypeSMO || Filter.TypeFund);
        //}
        private void FilterReset()
        {
            Filter.TextOrgId = FilterDefault.TextOrgId;
            Filter.TextInnLe = FilterDefault.TextInnLe;
            Filter.TextOgrn = FilterDefault.TextOgrn;

            Filter.TextName = FilterDefault.TextName;
            Filter.TextShortName = FilterDefault.TextShortName;
            Filter.TextEmail = FilterDefault.TextEmail;
            Filter.TextPhone = FilterDefault.TextPhone;

            Filter.TypeMO = FilterDefault.TypeMO;
            Filter.TypeSMO = FilterDefault.TypeSMO;
            Filter.TypeFund = FilterDefault.TypeFund;

            Filter.StateActive = FilterDefault.StateActive;
            Filter.StateInactive = FilterDefault.StateInactive;
            Filter.StateOnSubmit = FilterDefault.StateOnSubmit;
            Filter.StateBlocked = FilterDefault.StateBlocked;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedText = false;
            Filter.ChangedTypes = false;
            Filter.ChangedStates = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }

        //private bool Search(OrgsResponse org)
        //{
        //    if (string.IsNullOrWhiteSpace(_searchString)) { return true; }
        //    var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        //    return (
        //        //org.Id.ToString().Contains(_searchString, comparison) ||
        //        org.Inn?.Contains(_searchString, comparison) == true ||
        //        org.Ogrn?.Contains(_searchString, comparison) == true ||
        //        org.Name?.Contains(_searchString, comparison) == true ||
        //        org.ShortName?.Contains(_searchString, comparison) == true ||
        //        org.Email?.Contains(_searchString, comparison) == true
        //        //org.Phone?.Contains(_searchString, comparison) == true;
        //        );
        //}
    }
}
