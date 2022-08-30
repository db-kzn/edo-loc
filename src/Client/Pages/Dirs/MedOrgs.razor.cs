using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Filters;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs;

public partial class MedOrgs
{
    public Origin TransformOrigin { get; set; } = Origin.TopRight;
    public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

    [Inject] private IDirectoryManager DirManager { get; set; }

    private MudTable<CompaniesResponse> _mudTable;
    private IEnumerable<CompaniesResponse> _pagedData = new List<CompaniesResponse>();
    private CompaniesResponse _company;
    private readonly List<CompaniesResponse> _companies = new();

    //private readonly bool resetValueOnEmptyText = true;
    //private readonly bool coerceText = true;
    //private readonly bool coerceValue = false;
    //private readonly bool clearable = true;

    //private bool openFilter = true;
    //private readonly MedOrgFilter Filter = new();
    //private readonly MedOrgFilter FilterDefault = new();

    private bool _loaded = false;
    private string _searchString = "";

    private ClaimsPrincipal _authUser;
    private string userId;
    private bool _canDocsCreate;

    private int tz;
    private bool dense;
    private bool matchCase;

    private int delay;
    private int duration;

    private int _totalItems = 0;
    private int _pageNumber = 1;
    private int _rowsPerPage;

    private bool importAvailable = false;
    private CheckCompaniesForImportsResponse _companiesForImports = new();

    protected override async Task OnInitializedAsync()
    {
        _authUser = await _authManager.CurrentUser();
        //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
        _canDocsCreate = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.Create)).Succeeded;

        userId = _authUser.GetUserId();
        tz = _stateService.Timezone;

        _rowsPerPage = _stateService.RowsPerPage;
        dense = _stateService.Dense;
        matchCase = _stateService.MatchCase;
        //openFilter = _stateService.FilterIsOpen;

        delay = _stateService.TooltipDelay;
        duration = _stateService.TooltipDuration;

        await CheckCompaniesForImportsAsync();
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
    //private void ToggleFilter()
    //{
    //    openFilter = !openFilter;
    //    _stateService.FilterIsOpen = openFilter;
    //}

    private async Task ImportFOMS()
    {
        _loaded = false;
        var response = await DirManager.ImportFomsAsync();
        await CheckImportResultAsync(response);
    }
    private async Task ImportSMO()
    {
        _loaded = false;
        var response = await DirManager.ImportSmoAsync();
        await CheckImportResultAsync(response);
    }
    private async Task ImportMO()
    {
        _loaded = false;
        var response = await DirManager.ImportMoAsync();
        await CheckImportResultAsync(response);
    }

    private async Task CheckCompaniesForImportsAsync()
    {
        if (_canDocsCreate)
        {
            var result = await DirManager.CheckCompaniesForImportsAsync();
            await _jsRuntime.InvokeVoidAsync("azino.Console", result, "Companies For Imports");

            if (result.Succeeded)
            {
                _companiesForImports = result.Data;
                importAvailable = _companiesForImports.Fund || _companiesForImports.SMO || _companiesForImports.MO;
            }
        }
    }
    private async Task CheckImportResultAsync(IResult<ImportResponse> response)
    {
        _loaded = true;

        if (!response.Succeeded)
        {
            response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
            return;
        }

        var rows = response.Data;
        var results = $"<ul>" +
            $"<li>{_localizer["Total"]} : {rows.Total}</li>" +
            $"<li>{_localizer["Added"]} : {rows.Added}</li>" +
            $"<li>{_localizer["Updated"]} : {rows.Updated}</li>" +
            $"<li>{_localizer["Skipped"]} : {rows.Skipped}</li>" +
            $"</ul>";
        
        _snackBar.Add(results,
            Severity.Normal,
            config => { 
                config.Icon = Icons.Material.TwoTone.Unarchive;
                config.SnackbarVariant = Variant.Outlined;
            });

        await RenewAsync();
    }

    private async Task ApplyFilter() => await _mudTable.ReloadServerData();
    private async Task RenewAsync()
    {
        //FilterReset();
        await _mudTable.ReloadServerData();
        await CheckCompaniesForImportsAsync();
    }
    private async Task OnSearch(string text)
    {
        _searchString = text;
        await _mudTable.ReloadServerData();
    }

    private async Task<TableData<CompaniesResponse>> ServerReloadAsync(TableState state = null)
    {
        _loaded = false;

        if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
        {
            _stateService.RowsPerPage = _mudTable.RowsPerPage;
        }

        _rowsPerPage = _stateService.RowsPerPage;
        _pageNumber = (state != null) ? state.Page + 1 : 1;
        await GetCompaniesAsync(state);

        if (!_loaded) { _loaded = true; }
        StateHasChanged();

        return new TableData<CompaniesResponse> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task GetCompaniesAsync(TableState state = null)
    {
        //await _jsRuntime.InvokeVoidAsync("azino.Console", _stateService, "Docs State");

        string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
            ? (orderings = state.SortDirection != SortDirection.None
                ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                : new[] { $"{state.SortLabel}" })
            : null;

        PaginatedResult<CompaniesResponse> response = null;

        //if (Filter.IsActive && !Filter.IsEmpty)
        //{
        //    //List<DocStages> stages = new();
        //    //if (!Filter.StageDraft && !Filter.StageInProgress && !Filter.StageRejected)
        //    //{
        //    //    stages.AddRange(new List<DocStages> { DocStages.Draft, DocStages.InProgress, DocStages.Rejected });
        //    //}
        //    //else
        //    //{
        //    //    if (Filter.StageDraft) { stages.Add(DocStages.Draft); }
        //    //    if (Filter.StageInProgress) { stages.Add(DocStages.InProgress); }
        //    //    if (Filter.StageRejected) { stages.Add(DocStages.Rejected); }
        //    //}

        //    //List<int> types = new();
        //    //if (!Filter.TypeContract && !Filter.TypeAgreement)
        //    //{
        //    //    types.AddRange(new List<int> { 1, 2 }); // ID from db
        //    //}
        //    //else
        //    //{
        //    //    if (Filter.TypeContract) { types.Add(1); }
        //    //    if (Filter.TypeAgreement) { types.Add(2); }
        //    //}

        //    //var filter = new SearchDocsRequest()
        //    //{
        //    //    PageSize = _rowsPerPage,
        //    //    PageNumber = _pageNumber,

        //    //    OrderBy = orderings,
        //    //    MatchCase = matchCase,
        //    //    SearchString = _searchString,
        //    //};

        //    //await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Docs Filter");
        //    //response = await DirManager.SearchCompaniesAsync(filter);
        //}
        //else
        //{
            //var sort = "";
            //if (orderings?.Any() == true)
            //{
            //    foreach (var orderBy in orderings) { sort += $"{orderBy},"; }
            //    sort = sort[..^1]; // loose training ,
            //}

            var request = new GetPagedCompaniesRequest()
            {
                PageSize = _rowsPerPage,
                PageNumber = _pageNumber,
                SearchString = _searchString,

                OrderBy = orderings,
                MatchCase = matchCase
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", request, "Dir Companies Request");
            response = await DirManager.GetCompaniesAsync(request);
        //}

        await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Dir Companies Response");

        if (!response.Succeeded)
        {
            response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
            _loaded = true;
            return;
        }

        _totalItems = response.TotalCount;
        //_pageNumber = response.CurrentPage;

        _companies.Clear();
        var data = response.Data;
        data?.ForEach((d) => _companies.Add(d));

        _pagedData = _companies;
        //await _jsRuntime.InvokeVoidAsync("azino.Console", _docs);

        _loaded = true;
    }

    private void OnRowClickAsync() { }

    //private void FilterReset()
    //{
    //    Filter.TextOrgId = FilterDefault.TextOrgId;
    //    Filter.TextInnLe = FilterDefault.TextInnLe;
    //    Filter.TextOgrn = FilterDefault.TextOgrn;

    //    Filter.TextName = FilterDefault.TextName;
    //    Filter.TextShortName = FilterDefault.TextShortName;
    //    Filter.TextEmail = FilterDefault.TextEmail;
    //    Filter.TextPhone = FilterDefault.TextPhone;

    //    Filter.TypeMO = FilterDefault.TypeMO;
    //    Filter.TypeSMO = FilterDefault.TypeSMO;
    //    Filter.TypeFund = FilterDefault.TypeFund;

    //    Filter.StateActive = FilterDefault.StateActive;
    //    Filter.StateInactive = FilterDefault.StateInactive;
    //    Filter.StateOnSubmit = FilterDefault.StateOnSubmit;
    //    Filter.StateBlocked = FilterDefault.StateBlocked;

    //    Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
    //    Filter.CreateOnTo = FilterDefault.CreateOnTo;

    //    Filter.ChangedText = false;
    //    Filter.ChangedTypes = false;
    //    Filter.ChangedStates = false;
    //    Filter.ChangedCreateOn = false;

    //    Filter.IsActive = false;
    //    Filter.IsEmpty = false;
    //}
}
