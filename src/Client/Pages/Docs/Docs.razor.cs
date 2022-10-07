using EDO_FOMS.Application.Features.Documents.Commands;
using EDO_FOMS.Application.Features.Documents.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Filters;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Infrastructure.Models.Dirs;
using EDO_FOMS.Client.Models;
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

namespace EDO_FOMS.Client.Pages.Docs
{
    public partial class Docs
    {
        public Origin TransformOrigin { get; set; } = Origin.TopRight;
        public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }

        [CascadingParameter]
        public NavCounts NavCounts { get; set; }

        private MudTable<DocModel> _mudTable;
        private IEnumerable<DocModel> _pagedData;
        private DocModel _doc;
        private readonly List<DocModel> _docs = new();

        //private int _importsCount;
        private bool importPossible = false;
        private readonly List<ActiveRouteModel> _activeRoutes = new();

        public OrgsResponse orgContact;
        public ContactResponse employeeContact;

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;
        //private readonly bool clearable = true;

        private bool openFilter = false;
        private readonly DocFilter Filter = new();
        private readonly DocFilter FilterDefault = new();

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

        private int _totalItems;
        private int _pageNumber = 1;
        private int _rowsPerPage;
        //private int _currentPage;
        //private int _pageSize = 10;

        private MudDatePicker _dateFrom;
        private MudDatePicker _dateTo;
        private MudDatePicker _createOnFrom;
        private MudDatePicker _createOnTo;

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
            openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetActiveRoutesAsync();
            await GetAllDocTypeTitlesAsync();

            //if (_canDocsCreate)
            //{
            //    var result = await DocManager.CheckForImportsAsync();
            //    await _jsRuntime.InvokeVoidAsync("azino.Console", result, "For Imports");
            //    if (result.Succeeded) { _importsCount = result.Data; }
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
        }
        private async Task OnSearch(string text)
        {
            _searchString = text;
            await _mudTable.ReloadServerData();
        }

        private async Task<TableData<DocModel>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetDocsAsync(state);

            if (!_loaded) { _loaded = true; }
            StateHasChanged();

            return new TableData<DocModel> { TotalItems = _totalItems, Items = _pagedData };
        }
        private async Task GetDocsAsync(TableState state = null)
        {
            //await _jsRuntime.InvokeVoidAsync("azino.Console", _stateService, "Docs State");

            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            PaginatedResult<GetDocumentsResponse> response;

            if (Filter.IsActive && !Filter.IsEmpty)
            {
                List<DocStages> stages = new();
                if (!Filter.StageDraft && !Filter.StageInProgress && !Filter.StageRejected)
                {
                    stages.AddRange(new List<DocStages> { DocStages.Draft, DocStages.InProgress, DocStages.Rejected });
                }
                else
                {
                    if (Filter.StageDraft) { stages.Add(DocStages.Draft); }
                    if (Filter.StageInProgress) { stages.Add(DocStages.InProgress); }
                    if (Filter.StageRejected) { stages.Add(DocStages.Rejected); }
                }

                List<int> types = new(); // Какие типы документов нужны !!!
                Filter.DocTypes.ForEach(dt => { if (dt.IsChecked) { types.Add(dt.Id); }});

                var filter = new SearchDocsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,

                    OrderBy = orderings,
                    DocStages = stages.ToArray(),
                    DocTypeIds = types.ToArray(),

                    MatchCase = matchCase,
                    SearchString = _searchString,

                    OrgId = Filter.OrgId,
                    UserId = Filter.UserId,
                    TextNumber = Filter.TextNumber,
                    TextTitle = Filter.TextTitle,

                    DateFrom = Filter.DateFrom,
                    DateTo = Filter.DateTo,
                    CreateOnFrom = Filter.CreateOnFrom,
                    CreateOnTo = Filter.CreateOnTo
                };

                await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Docs Filter");
                response = await DocManager.SearchDocsAsync(filter);
            }
            else
            {
                //var sort = "";
                //if (orderings?.Any() == true)
                //{
                //    foreach (var orderBy in orderings) { sort += $"{orderBy},"; }
                //    sort = sort[..^1]; // loose training ,
                //}

                var request = new GetPagedDocumentsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    DocStage = DocStages.AllActive,
                    MatchCase = matchCase
                };

                await _jsRuntime.InvokeVoidAsync("azino.Console", request, "Docs Request");
                response = await DocManager.GetDocsAsync(request);
            }

            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Docs Paged Response");

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;
            //_pageNumber = response.CurrentPage;

            _docs.Clear();
            var data = response.Data;
            data.ForEach((d) => _docs.Add(NewDoc(d)));

            _pagedData = _docs;
            //await _jsRuntime.InvokeVoidAsync("azino.Console", _docs);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var result = await _stateManager.RefreshNavCountsAsync(_authUser.GetUserId());

                if (result.Succeeded)
                {
                    var counts = result.Data;

                    NavCounts.Progress = counts.Progress;
                    NavCounts.Docs = counts.Docs;
                    NavCounts.Archive = counts.Archive;

                    //await _jsRuntime.InvokeVoidAsync("azino.Console", counts, "Counts: ");
                }
            }

            _loaded = true;
        }

        private async Task GetActiveRoutesAsync()
        {
            var response = await DocManager.GetActiveRoutesAsync();

            if (response.Succeeded)
            {
                _activeRoutes.Clear();
                response.Data.ForEach(rt => _activeRoutes.Add(rt));

                importPossible = _activeRoutes.Any(r => r.ParseFileName) && _canDocsCreate;
                await _jsRuntime.InvokeVoidAsync("azino.Console", _activeRoutes, "Active Routes");
            }
        }
        private async Task GetAllDocTypeTitlesAsync()
        {
            var response = await DirManager.GetAllDocTypeTitlesAsync();

            if (!response.Succeeded) { return; }
            
            await _jsRuntime.InvokeVoidAsync("azino.Console", response.Data, "Doc Type Titles");

            var docTypeTitles = response.Data;

            docTypeTitles.ForEach(dtt => {
                Filter.DocTypes.Add(new FilterDocTypeModel(dtt));
                FilterDefault.DocTypes.Add(new FilterDocTypeModel(dtt));
            });
        }

        //(TableRowClickEventArgs<DocModel> e)
        //if (e is null) { throw new ArgumentNullException(nameof(e)); }
        private async Task OnRowClickAsync() => await ChooseAction(_doc);
        private async Task ChooseAction(DocModel doc)
        {
            if (doc.Stage == DocStages.Draft)
            {
                AddEditDoc(doc);
            }
            else
            {
                var result = await ShowInProcessAsync(doc);
                if (!result.Cancelled) { await _mudTable.ReloadServerData(); }
            }
        }

        private void AddDocAsync(int routeId)
        {
            //var result = await AddEditDocAsync(new(1, DateTime.Today));
            //if (!result.Cancelled) { await _mudTable.ReloadServerData(); }
            _navigationManager.NavigateTo($"/docs/doc-card/{routeId}/{0}");
        }
        private void AddEditDoc(DocModel doc)
        {
            _navigationManager.NavigateTo($"/docs/doc-card/{doc.RouteId}/{doc.DocId}");

            ////var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            //var parameters = new DialogParameters()
            //{
            //    {
            //        nameof(DraftDialog._doc),
            //        new AddEditDocumentCommand
            //        {
            //            Id = doc.DocId,
            //            EmplId = doc.EmplId,
            //            EmplOrgId = doc.EmplOrgId,
            //            DocParentId = doc.ParentId,

            //            RouteId = doc.RouteId,
            //            Stage = doc.Stage,

            //            TypeId = doc.TypeId ?? 0,
            //            Number = doc.Number,
            //            Date = doc.Date,

            //            Title = doc.Title,
            //            Description = doc.Description,
            //            IsPublic = doc.IsPublic,

            //            CurrentStep = doc.CurrentStep,
            //            TotalSteps = doc.TotalSteps,
            //            UploadRequest = new() { FileName = doc.FileName },
            //            URL = doc.URL
            //        }
            //    }
            //};

            //var dialog = _dialogService.Show<DraftDialog>("", parameters); //, options
            //return await dialog.Result;
        }
        private async Task<DialogResult> ShowInProcessAsync(DocModel doc)
        {
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var parameters = new DialogParameters() { { nameof(InProgressDialog.Doc), doc } };
            var options = new DialogOptions { CloseButton = true };

            var dialog = _dialogService.Show<InProgressDialog>("", parameters, options);

            return await dialog.Result;
        }

        private async void ImportFiles()
        {
            var parameters = new DialogParameters() { }; //{ nameof(InProgressDialog.Doc), doc }
            var options = new DialogOptions { CloseButton = true }; // MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true

            var dialog = _dialogService.Show<AvailableImportDialog>("", parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled) { await RenewAsync(); }
        }

        private static string OrgName(OrgsResponse c)
        {
            if (c == null) { return null; }

            var info = string.IsNullOrWhiteSpace(c.ShortName) ? c.Inn : $"{c.Inn}, {c.ShortName}";

            var name = c.Name.ToLower();
            name = name.Length > 32 ? name.Substring(0, 32) : name;

            return $"[{info}] {name}";
        }
        private async Task<IEnumerable<OrgsResponse>> SearchOrgAsync(string search)
        {
            var response = await DocManager.GetFoundOrgs(search);
            return response.Succeeded ? response.Data : new();
        }
        private void OrgChanged(OrgsResponse o)
        {
            orgContact = o;
            Filter.OrgId = o?.Id;

            OnAttendee();
        }

        private static string EmployeeName(ContactResponse c)
        {
            if (c == null) { return null; }
            return $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";
        }
        private async Task<IEnumerable<ContactResponse>> SearchEmployeeAsync(string search)
        {
            var request = NewSearchContactsRequest((int)UserBaseRoles.Undefined, OrgTypes.Undefined, search);
            return await SearchContactsAsync(request);
        }
        private void EmployeeChanged(ContactResponse c)
        {
            employeeContact = c;
            Filter.UserId = c?.Id;

            OnAttendee();
        }

        public static SearchContactsRequest NewSearchContactsRequest(UserBaseRoles baseRole, OrgTypes orgType, string searchString)
        {
            return new()
            {
                BaseRole = baseRole,
                OrgType = orgType,
                SearchString = searchString
            };
        }
        private async Task<IEnumerable<ContactResponse>> SearchContactsAsync(SearchContactsRequest request)
        {
            var response = await DocManager.GetFoundContacts(request);
            return response.Succeeded ? response.Data : new();
        }

        private void OnAttendee()
        {
            Filter.ChangedAttendee = Filter.OrgId != FilterDefault.OrgId || Filter.UserId != FilterDefault.UserId;
            FilterIsActive();
        }

        private void OnStage(string stage)
        {
            if (stage == "Draft") Filter.StageDraft = !Filter.StageDraft;
            else if (stage == "InProgress") Filter.StageInProgress = !Filter.StageInProgress;
            else if (stage == "Rejected") Filter.StageRejected = !Filter.StageRejected;

            Filter.ChangedStage = Filter.StageDraft != FilterDefault.StageDraft ||
                Filter.StageInProgress != FilterDefault.StageInProgress ||
                Filter.StageRejected != FilterDefault.StageRejected;

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnType(FilterDocTypeModel dt)
        {
            dt.IsChecked = !dt.IsChecked;

            Filter.ChangedType = false;

            Filter.DocTypes.ForEach(dt =>
            {
                var dtd = FilterDefault.DocTypes.Find(t => t.Id == dt.Id);
                if (dt.IsChecked != dtd.IsChecked) { Filter.ChangedType = true; }
            });

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnText()
        {
            Filter.ChangedText = Filter.TextNumber != FilterDefault.TextNumber ||
                Filter.TextTitle != FilterDefault.TextTitle;

            FilterIsActive();
        }

        private void OnDate(string c, DateTime? d)
        {
            if (c == "From") Filter.DateFrom = d;
            else if (c == "To") Filter.DateTo = d;

            Filter.ChangedDate = Filter.DateFrom != FilterDefault.DateFrom ||
                Filter.DateTo != FilterDefault.DateTo;

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
            Filter.IsActive = Filter.ChangedAttendee || Filter.ChangedStage || Filter.ChangedType
                || Filter.ChangedText || Filter.ChangedDate || Filter.ChangedCreateOn;
        }
        //private void FilterIsEmpty()
        //{
        //    Filter.IsEmpty = !(Filter.StageDraft || Filter.StageInProgress || Filter.StageRejected)
        //        || !(Filter.TypeContract || Filter.TypeAgreement);
        //}
        private void FilterReset()
        {
            orgContact = null;
            employeeContact = null;
            Filter.OrgId = FilterDefault.OrgId;
            Filter.UserId = FilterDefault.UserId;

            Filter.StageDraft = FilterDefault.StageDraft;
            Filter.StageInProgress = FilterDefault.StageInProgress;
            Filter.StageRejected = FilterDefault.StageRejected;

            Filter.DocTypes.Clear();
            FilterDefault.DocTypes.ForEach(dt => Filter.DocTypes.Add(new FilterDocTypeModel(dt)));

            Filter.TextNumber = FilterDefault.TextNumber;
            Filter.TextTitle = FilterDefault.TextTitle;

            Filter.DateFrom = FilterDefault.DateFrom;
            Filter.DateTo = FilterDefault.DateTo;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedAttendee = false;
            Filter.ChangedStage = false;
            Filter.ChangedType = false;
            Filter.ChangedText = false;
            Filter.ChangedDate = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }

        private async Task RunProcessAsync(int id) => await ChandeStageAsync(id, DocStages.InProgress);
        private async Task StopProcessAsync(int id) => await ChandeStageAsync(id, DocStages.Draft);
        private async Task DeleteDocAsync(int id) => await ChandeStageAsync(id, DocStages.Deleted);

        private async Task ChandeStageAsync(int id, DocStages stage)
        {
            _loaded = false;

            ChangeDocStageCommand request = new() { Id = id, Stage = stage };

            var response = await DocManager.ChangeStageAsync(request);

            if (!response.Succeeded)
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
                return;
            }

            await _mudTable.ReloadServerData();
        }
        private DocModel NewDoc(GetDocumentsResponse d)
        {
            return new()
            {
                DocId = d.Id,
                ParentId = d.ParentId,
                PreviousId = d.PreviousId,

                EmplId = d.EmplId,
                EmplOrgId = d.EmplOrgId,

                KeyOrgId = d.KeyOrgId,
                RecipientShort = d.RecipientShort,
                RecipientInn = d.RecipientInn,

                RouteId = d.RouteId,
                Stage = d.Stage,
                StageName = _localizer[d.Stage.ToString()],
                HasChanges = d.HasChanges,

                TypeId = d.TypeId,
                TypeName = d.TypeName,
                TypeShort = d.TypeShort,

                Number = d.Number,
                Date = d.Date,
                DateStr = d.Date?.ToString("d") ?? string.Empty,

                Title = d.Title,
                Description = d.Description,
                IsPublic = d.IsPublic,
                DepartmentId = d.DepartmentId,

                CurrentStep = d.CurrentStep,
                TotalSteps = d.TotalSteps,
                Version = d.Version,

                URL = d.URL,
                //StoragePath = d.StoragePath,
                FileName = d.FileName,

                CreatedBy = d.CreatedBy,
                CreatedOn = d.CreatedOn.AddHours(tz),
                CreatedOnStr = d.CreatedOn.AddHours(tz).ToString("g")
            };
        }
        private static string UnreadRowClassFunc(DocModel d, int _) => d.HasChanges ? "unread" : string.Empty;

        //private bool Search(DocModel doc)
        //{
        //    if (string.IsNullOrWhiteSpace(_searchString)) return true;
        //    var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        //    return doc.StageName?.Contains(_searchString, comparison) == true ||
        //        doc.TypeName?.Contains(_searchString, comparison) == true ||
        //        doc.Number?.Contains(_searchString, comparison) == true ||
        //        doc.DateStr?.Contains(_searchString, comparison) == true ||
        //        doc.Title?.Contains(_searchString, comparison) == true ||
        //        doc.CreatedOnStr?.Contains(_searchString, comparison) == true;
        //}

        private string DocTypeIcon(DocIcons icon) => DirManager.DocTypeIcon(icon);
    }
}
