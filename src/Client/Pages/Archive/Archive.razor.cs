using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Agreements;
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

namespace EDO_FOMS.Client.Pages.Archive
{
    public partial class Archive
    {
        public Origin TransformOrigin { get; set; } = Origin.TopRight;
        public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }

        [CascadingParameter]
        public NavCounts NavCounts { get; set; }

        private MudTable<AgreementModel> _mudTable;
        private IEnumerable<AgreementModel> _pagedData;
        private AgreementModel _agreement;
        private readonly List<AgreementModel> _agreements = new();

        public OrgsResponse orgContact;
        public ContactResponse employeeContact;

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;
        //private readonly bool clearable = true;

        private bool openFilter = false;
        private readonly ArchiveFilter Filter = new();
        private readonly ArchiveFilter FilterDefault = new();

        private bool _loaded = false;
        private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canDocsView;

        private int tz;
        private bool dense;
        private bool matchCase = false;

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
            _canDocsView = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.View)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetAllDocTypeTitlesAsync();

            var docTypeTitles = await DirManager.GetAllDocTypeTitlesAsync();

            if (docTypeTitles.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("azino.Console", docTypeTitles, "Doc Type Titles");
            }

            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;
            //if (user == null) { return; }
            //if (user.Identity?.IsAuthenticated == true) { _authUserId = user.GetUserId(); }

            //await GetAgreementsAsync();
            //_loaded = true;
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
        private async Task Renew()
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

        private async Task<TableData<AgreementModel>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetAgreementsAsync(state);

            if (!_loaded) { _loaded = true; }

            return new TableData<AgreementModel> { TotalItems = _totalItems, Items = _pagedData };
        }
        private async Task GetAgreementsAsync(TableState state = null)
        {
            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            PaginatedResult<EmployeeAgreementsResponse> response;

            if (Filter.IsActive && !Filter.IsEmpty)
            {
                //List<DocStages> stages = new();
                //if (Filter.StageDraft) stages.Add(DocStages.Draft);
                //if (Filter.StageInProgress) stages.Add(DocStages.InProgress);
                //if (Filter.StageRejected) stages.Add(DocStages.Rejected);

                List<int> types = new();
                Filter.DocTypes.ForEach(dt => { if (dt.IsChecked) { types.Add(dt.Id); } });

                List<AgreementActions> actions = new();
                if (!Filter.ActionToRun && !Filter.ActionToApprove && !Filter.ActionToVerify && !Filter.ActionToSign)
                {
                    actions.AddRange(new List<AgreementActions> { AgreementActions.ToRun,
                        AgreementActions.ToApprove, AgreementActions.ToReview, AgreementActions.ToSign });
                }
                else
                {
                    if (Filter.ActionToRun) { actions.Add(AgreementActions.ToRun); }
                    if (Filter.ActionToApprove) { actions.Add(AgreementActions.ToApprove); }
                    if (Filter.ActionToVerify) { actions.Add(AgreementActions.ToReview); }
                    if (Filter.ActionToSign) { actions.Add(AgreementActions.ToSign); }
                }

                var filter = new SearchAgrsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    OrderBy = orderings,

                    DocStage = DocStages.Archive,
                    AgrActions = actions.ToArray(),
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

                await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Agrs Filter");
                response = await DocManager.SearchAgrsAsync(filter);
                //await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Docs Response");
            }
            else
            {
                var request = new GetPagedAgreementsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    AgrState = AgreementStates.DocInArchive,
                    MatchCase = matchCase
                };

                response = await DocManager.GetEmployeeAgreementsAsync(request); // AgreementStates state
            }

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;

            _agreements.Clear();
            var data = response.Data;
            data.ToList().ForEach((d) => _agreements.Add(NewAgreement(d)));

            _pagedData = _agreements;
            //await _jsRuntime.InvokeVoidAsync("azino.Console", _agreements);

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

        //TableRowClickEventArgs<AgreementModel> e
        private async Task OnRowClickAsync()
        {
            if (!_loaded) return;
            await ShowInProcessAsync(_agreement); 
        }
        private async Task ShowInProcessAsync(AgreementModel a)
        {
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var doc = AgreementToDoc(a);
            var parameters = new DialogParameters() { { nameof(InProgressDialog.Doc), doc } };
            var options = new DialogOptions { CloseButton = true };

            var dialog = _dialogService.Show<InProgressDialog>("", parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                //var action = result.Data.ToString();
                //await _jsRuntime.InvokeVoidAsync("azino.Console", action);

                _loaded = false;
                await GetAgreementsAsync();
                _loaded = true;
            }
        }

        private AgreementModel NewAgreement(EmployeeAgreementsResponse a)
        {
            return new()
            {
                AgreementId = a.AgreementId,

                EmplOrgId = a.EmplOrgId,  // Организация получателя
                EmplId = a.EmplId,

                KeyOrgId = a.KeyOrgId,
                RecipientShort = a.RecipientShort,
                RecipientInn = a.RecipientInn,

                //Recipients

                IssuerOrgId = a.IssuerOrgId,
                IssuerType = a.IssuerType,
                IssuerOrgInn = a.IssuerOrgInn,
                IssuerOrgShortName = a.IssuerOrgShortName,

                DocId = a.DocId,
                DocParentId = a.DocParentId,
                DocRouteId = a.DocRouteId,
                DocIsPublic = a.DocIsPublic,

                DocTypeId = a.DocTypeId,
                DocTypeName = a.DocTypeName, //(a.DocTypeId == 1) ? "Договор" : "Доп.соглашение",  //a.DocTypeName,
                DocTypeShort = a.DocTypeShort, //(a.DocTypeId == 1) ? "Дог" : "Д/С", //a.DocTypeShort,

                DocNumber = a.DocNumber,
                DocDate = a.DocDate,
                DocDateStr = a.DocDate?.ToString("d") ?? string.Empty,
                DocTitle = a.DocTitle,

                DocDescription = a.DocDescription,
                DocURL = a.DocURL,
                DocFileName = a.DocFileName,

                DocStage = a.DocStage,
                DocStageName = _localizer[a.DocStage.ToString()],
                DocHasChanges = a.DocHasChanges,
                DocCurrentStep = a.DocCurrentStep,
                DocTotalSteps = a.DocTotalSteps,

                DocVersion = a.DocVersion,
                DocCreatedBy = a.DocCreatedBy,
                DocCreatedOn = a.DocCreatedOn.AddHours(tz),
                DocCreatedOnStr = a.DocCreatedOn.AddHours(tz).ToString("g"),

                Step = a.Step,
                State = a.State,
                Action = a.Action,
                ActionName = _localizer[a.Action.ToString()],
                IsCanceled = a.IsCanceled,

                CreatedOn = a.CreatedOn.AddHours(tz),
                Received = a.Received,
                Opened = a.Opened,
                Answered = a.Answered,

                Remark = a.Remark,
                SignURL = a.SignURL
            };
        }
        private DocModel AgreementToDoc(AgreementModel a)
        {
            return new()
            {
                EmplOrgId = (int)a.EmplOrgId,  // Организация получателя
                EmplId = a.EmplId,
                AgreementId = a.AgreementId,

                //Recipients

                //IssuerOrgId = a.IssuerOrgId,
                //IssuerType = a.IssuerType,
                //IssuerOrgInn = a.IssuerOrgInn,
                //IssuerOrgShortName = a.IssuerOrgShortName,

                DocId = a.DocId,
                ParentId = a.DocParentId,
                RouteId = a.DocRouteId,
                IsPublic = a.DocIsPublic,

                TypeId = a.DocTypeId,
                TypeName = a.DocTypeName,
                TypeShort = a.DocTypeShort,

                Number = a.DocNumber,
                Date = a.DocDate,
                DateStr = a.DocDate?.ToString("d") ?? string.Empty,
                Title = a.DocTitle,

                Description = a.DocDescription,
                URL = a.DocURL,
                FileName = a.DocFileName,

                Stage = a.DocStage,
                StageName = _localizer[a.DocStage.ToString()],
                CurrentStep = a.DocCurrentStep,
                TotalSteps = a.DocTotalSteps,

                Version = a.DocVersion,
                CreatedBy = a.DocCreatedBy,
                CreatedOn = a.DocCreatedOn,
                CreatedOnStr = a.DocCreatedOnStr
            };
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
        private void OrgChanged (OrgsResponse o)
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
        private void OnAction(string stage)
        {
            if (stage == "ToRun") Filter.ActionToRun = !Filter.ActionToRun;
            else if (stage == "ToApprove") Filter.ActionToApprove = !Filter.ActionToApprove;
            else if (stage == "ToVerify") Filter.ActionToVerify = !Filter.ActionToVerify;
            else if (stage == "ToSign") Filter.ActionToSign = !Filter.ActionToSign;

            Filter.ChangedAction = Filter.ActionToRun != FilterDefault.ActionToRun ||
                Filter.ActionToApprove != FilterDefault.ActionToApprove ||
                Filter.ActionToVerify != FilterDefault.ActionToVerify ||
                Filter.ActionToSign != FilterDefault.ActionToSign;

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
            Filter.IsActive = Filter.ChangedAttendee || Filter.ChangedAction || Filter.ChangedType
                || Filter.ChangedText || Filter.ChangedDate || Filter.ChangedCreateOn;
        }
        //private void FilterIsEmpty()
        //{
        //    Filter.IsEmpty = !(Filter.ActionToRun || Filter.ActionToApprove || Filter.ActionToVerify || Filter.ActionToSign)
        //        || !(Filter.TypeContract || Filter.TypeAgreement);
        //}
        private void FilterReset()
        {
            orgContact = null;
            employeeContact = null;
            Filter.OrgId = FilterDefault.OrgId;
            Filter.UserId = FilterDefault.UserId;

            Filter.ActionToRun = FilterDefault.ActionToRun;
            Filter.ActionToApprove = FilterDefault.ActionToApprove;
            Filter.ActionToVerify = FilterDefault.ActionToVerify;
            Filter.ActionToSign = FilterDefault.ActionToSign;

            Filter.DocTypes.Clear();
            FilterDefault.DocTypes.ForEach(dt => Filter.DocTypes.Add(new FilterDocTypeModel(dt)));

            Filter.TextNumber = FilterDefault.TextNumber;
            Filter.TextTitle = FilterDefault.TextTitle;

            Filter.DateFrom = FilterDefault.DateFrom;
            Filter.DateTo = FilterDefault.DateTo;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedAttendee = false;
            Filter.ChangedAction = false;
            Filter.ChangedType = false;
            Filter.ChangedText = false;
            Filter.ChangedDate = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }

        private static string UnreadRowClassFunc(AgreementModel d, int _) => d.DocHasChanges ? "unread" : string.Empty;

        //private bool Search(AgreementModel a)
        //{
        //    if (string.IsNullOrWhiteSpace(_searchString)) return true;
        //    var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        //    return a.DocStageName?.Contains(_searchString, comparison) == true ||
        //        a.DocTypeName?.Contains(_searchString, comparison) == true ||
        //        a.DocNumber?.Contains(_searchString, comparison) == true ||
        //        a.DocDateStr?.Contains(_searchString, comparison) == true ||
        //        a.DocTitle?.Contains(_searchString, comparison) == true ||
        //        a.DocCreatedOnStr?.Contains(_searchString, comparison) == true;
        //}

        private string DocTypeIcon(DocIcons icon) => DirManager.DocTypeIcon(icon);
    }
}
