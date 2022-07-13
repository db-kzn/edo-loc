using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Filters;
using EDO_FOMS.Client.Infrastructure.Managers.System;
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
    public partial class Certs
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        private MudTable<CertsResponse> _mudTable;
        private IEnumerable<CertsResponse> _pagedData;
        private CertsResponse _cert;
        private readonly List<CertsResponse> _certList = new();

        private bool openFilter = true;
        private readonly CertFilter Filter = new();
        private readonly CertFilter FilterDefault = new();

        private bool _loaded = false;
        private string _searchString = "";
        
        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit = false;

        private int tz;
        private bool dense;
        private bool matchCase = false;

        private int delay;
        private int duration;

        private int _totalItems;
        private int _pageNumber = 1;
        private int _rowsPerPage;

        private MudDatePicker _fromDateFrom;
        private MudDatePicker _fromDateTo;

        private MudDatePicker _tillDateFrom;
        private MudDatePicker _tillDateTo;

        private MudDatePicker _createOnFrom;
        private MudDatePicker _createOnTo;

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
            openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            //await GetCertsAsync();

            //HubConnection = HubConnection.TryInitialize(_navigationManager);
            //if (HubConnection.State == HubConnectionState.Disconnected)
            //{
            //    await HubConnection.StartAsync();
            //}
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

        //if (e is null) { throw new ArgumentNullException(nameof(e)); }
        //TableRowClickEventArgs<GetAllCertsResponse> e
        async Task EditCert() => await AddEdigCert(_cert);
        async Task AddCert() => await AddEdigCert(new());
        async Task AddEdigCert(CertsResponse cert)
        {
            var param = new DialogParameters
            {
                {
                    nameof(CertEditDialog.AddEditCertModel),
                    new AddEditCertCommand
                    {
                        Id = cert.Id,

                        Thumbprint = cert.Thumbprint,
                        UserId = cert.UserId,
                        Snils = cert.Snils,

                        IsActive = cert.IsActive,
                        SignAllowed = cert.SignAllowed,
                        IssuerInn = cert.IssuerInn,

                        SerialNumber = cert.SerialNumber,
                        Algorithm = cert.Algorithm,
                        Version = cert.Version,

                        FromDate = cert.FromDate,
                        TillDate = cert.TillDate,
                        CreatedOn = cert.CreatedOn
                    }
                }
            };

            var dialog = _dialogService.Show<CertEditDialog>("", param);
            var result = await dialog.Result;

            if (!result.Cancelled) { await ResetAsync(); }
        }

        private async Task<TableData<CertsResponse>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetCertsAsync(state);

            return new TableData<CertsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }
        private async Task GetCertsAsync(TableState state = null)
        {
            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            PaginatedResult<CertsResponse> response;

            if (Filter.IsActive && !Filter.IsEmpty)
            {
                var filter = new SearchCertsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,

                    OrderBy = orderings,
                    SearchString = _searchString,
                    MatchCase = matchCase,

                    TextCertId = Filter.TextCertId,
                    TextThumbPrint = Filter.TextThumbPrint,
                    TextSnils = Filter.TextSnils,

                    CertIsActive = Filter.CertIsActive,
                    SignAllowed = Filter.SignAllowed,

                    FromDateFrom = Filter.FromDateFrom,
                    FromDateTo = Filter.FromDateTo,

                    TillDateFrom = Filter.TillDateFrom,
                    TillDateTo = Filter.TillDateTo,

                    CreateOnFrom = Filter.CreateOnFrom,
                    CreateOnTo = Filter.CreateOnTo
                };

                await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Certs Filter");
                response = await AdmManager.SearchCertsAsync(filter);
            }
            else
            {
                var request = new GetPagedCertsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    MatchCase = matchCase
                };

                response = await AdmManager.GetPagedCertsAsync(request);
            }

            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Certs Response");

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;

            _certList.Clear();
            var data = response.Data;
            data.ToList().ForEach(x =>
            {
                x.CreatedOn = x.CreatedOn.AddHours(tz);
                _certList.Add(x);
            });

            _pagedData = _certList;
            _loaded = true;
        }

        private async Task ResetAsync()
        {
            _cert = new();
            await _mudTable.ReloadServerData();
        }

        private void OnText()
        {
            Filter.ChangedText = (Filter.TextCertId != FilterDefault.TextCertId
                || Filter.TextThumbPrint != FilterDefault.TextThumbPrint
                || Filter.TextSnils != FilterDefault.TextSnils);

            FilterIsActive();
        }
        private void OnState(string state)
        {
            if (state == "CertIsActive")
            {
                Filter.CertIsActive = (Filter.CertIsActive == null) ? true
                    : Filter.CertIsActive == true ? false : null;
            }
            else if (state == "SignAllowed")
            {
                Filter.SignAllowed = (Filter.SignAllowed == null) ? true
                    : Filter.SignAllowed == true ? false : null;
            }

            Filter.ChangedStates = (Filter.CertIsActive != FilterDefault.CertIsActive
                || Filter.SignAllowed != FilterDefault.SignAllowed);

            FilterIsActive();
        }

        private void OnFromDate(string c, DateTime? d)
        {
            if (c == "From") { Filter.FromDateFrom = d; }
            else if (c == "To") { Filter.FromDateTo = d; }

            Filter.ChangedCreateOn = Filter.FromDateFrom != FilterDefault.FromDateFrom ||
                Filter.FromDateTo != FilterDefault.FromDateTo;

            FilterIsActive();
        }
        private void OnTillDate(string c, DateTime? d)
        {
            if (c == "From") { Filter.TillDateFrom = d; }
            else if (c == "To") { Filter.TillDateTo = d; }

            Filter.ChangedCreateOn = Filter.TillDateFrom != FilterDefault.TillDateFrom ||
                Filter.TillDateTo != FilterDefault.TillDateTo;

            FilterIsActive();
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
            Filter.IsActive = Filter.ChangedText || Filter.ChangedStates
                || Filter.ChangedFromDate || Filter.ChangedTillDate || Filter.ChangedCreateOn;
        }
        private void FilterIsEmpty()
        {
            Filter.IsEmpty = false;
        }
        private void FilterReset()
        {
            Filter.TextCertId = FilterDefault.TextCertId;
            Filter.TextThumbPrint = FilterDefault.TextThumbPrint;
            Filter.TextSnils = FilterDefault.TextSnils;

            Filter.CertIsActive = FilterDefault.CertIsActive;
            Filter.SignAllowed = FilterDefault.SignAllowed;

            Filter.FromDateFrom = FilterDefault.FromDateFrom;
            Filter.FromDateTo = FilterDefault.FromDateTo;

            Filter.TillDateFrom = FilterDefault.TillDateFrom;
            Filter.TillDateTo = FilterDefault.TillDateTo;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedText = false;
            Filter.ChangedStates = false;
            Filter.ChangedFromDate = false;
            Filter.ChangedTillDate = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }
    }
}
