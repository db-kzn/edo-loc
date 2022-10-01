using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Domain.Enums;
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

public partial class DocTypes
{
    public Origin TransformOrigin { get; set; } = Origin.TopRight;
    public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

    [Inject] private IDirectoryManager DirManager { get; set; }

    private MudTable<DocTypesResponse> _mudTable;
    private IEnumerable<DocTypesResponse> _pagedData = new List<DocTypesResponse>();
    private DocTypesResponse _docType;
    private readonly List<DocTypesResponse> _docTypes = new();

    //private readonly bool resetValueOnEmptyText = true;
    //private readonly bool coerceText = true;
    //private readonly bool coerceValue = false;
    //private readonly bool clearable = true;

    //private bool openFilter = true;
    //private readonly DocTypeFilter Filter = new();
    //private readonly DocTypeFilter FilterDefault = new();

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

    private async Task<TableData<DocTypesResponse>> ServerReloadAsync(TableState state = null)
    {
        _loaded = false;

        if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
        {
            _stateService.RowsPerPage = _mudTable.RowsPerPage;
        }

        _rowsPerPage = _stateService.RowsPerPage;
        _pageNumber = (state != null) ? state.Page + 1 : 1;
        await GetDocTypesAsync(state);

        if (!_loaded) { _loaded = true; }
        //StateHasChanged();

        return new TableData<DocTypesResponse> { TotalItems = _totalItems, Items = _pagedData };
    }
    private async Task GetDocTypesAsync(TableState state = null)
    {
        //await _jsRuntime.InvokeVoidAsync("azino.Console", _stateService, "Docs State");

        string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
            ? (orderings = state.SortDirection != SortDirection.None
                ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                : new[] { $"{state.SortLabel}" })
            : null;

        PaginatedResult<DocTypesResponse> response = null;

        var request = new GetPagedDocTypesRequest()
        {
            PageSize = _rowsPerPage,
            PageNumber = _pageNumber,
            SearchString = _searchString,

            OrderBy = orderings,
            MatchCase = matchCase
        };

        await _jsRuntime.InvokeVoidAsync("azino.Console", request, "Dir DocTypes Request");
        response = await DirManager.GetDocTypesAsync(request);

        await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Dir DocTypes Response");

        if (!response.Succeeded)
        {
            response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
            _loaded = true;
            return;
        }

        _totalItems = response.TotalCount;
        //_pageNumber = response.CurrentPage;

        _docTypes.Clear();
        var data = response.Data;
        data?.ForEach((d) => _docTypes.Add(d));

        _pagedData = _docTypes;
        //await _jsRuntime.InvokeVoidAsync("azino.Console", _docs);

        _loaded = true;
    }

    private async Task OnRowClickAsync() => await AddEditDocTypeAsync(_docType);
    private async Task AddDocTypeAsync() => await AddEditDocTypeAsync(new());
    private async Task AddEditDocTypeAsync(DocTypesResponse docType)
    {
        var param = new DialogParameters
        {
            { 
                nameof(DocTypeDialog.AddEditDocTypeCommand),
                new AddEditDocTypeCommand()
                {
                    Id = docType.Id,
                    IsActive = docType.IsActive,
                    Icon = docType.Icon,
                    Color = docType.Color,

                    Code = docType.Code,
                    Short = docType.Short,
                    Label = docType.Label,
                    Name = docType.Name,

                    NameEn = docType.NameEn,
                    Description = docType.Description
                }
            }
        };

        var dialog = _dialogService.Show<DocTypeDialog>("", param);
        var result = await dialog.Result;

        if (!result.Cancelled) { await _mudTable.ReloadServerData(); }
    }

    private string DocTypeIcon(DocIcons icon) => DirManager.DocTypeIcon(icon);  
}
