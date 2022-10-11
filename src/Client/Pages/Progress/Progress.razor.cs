using EDO_FOMS.Application.Features.Agreements.Commands;
using EDO_FOMS.Application.Features.Agreements.Queries;
using EDO_FOMS.Application.Requests.Agreements;
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
using EDO_FOMS.Shared.Constants.Storage;
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

namespace EDO_FOMS.Client.Pages.Progress
{
    public partial class Progress
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
        private HashSet<AgreementModel> _selectedItems = new();

        public OrgsResponse orgContact;
        public ContactResponse employeeContact;

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;
        //private readonly bool clearable = true;

        private bool openFilter = false;
        private readonly ProgressFilter Filter = new();
        private readonly ProgressFilter FilterDefault = new();

        private bool _loaded = false;
        private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canDocsEdit;

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
            _canDocsEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.Edit)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            openFilter = _stateService.FilterIsOpen;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetAllDocTypeTitlesAsync();

            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;
            //if (user == null) { return; }
            //if (user.Identity?.IsAuthenticated == true) { _authUserId = user.GetUserId(); }

            //await _mudTable.ReloadServerData();
            //await GetAgreementsAsync(AgreementStates.AllActive);
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

        private async Task ApplyFilter() => await _mudTable.ReloadServerData();
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
            _selectedItems.Clear();

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetAgreementsAsync(state);

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

                List<int> types = new(); // Какие типы документов нужны !!!
                Filter.DocTypes.ForEach(dt => { if (dt.IsChecked) { types.Add(dt.Id); } });

                var filter = new SearchAgrsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,

                    OrderBy = orderings,
                    DocStage = DocStages.InProgress,
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
            }
            else
            {
                var request = new GetPagedAgreementsRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    AgrState = AgreementStates.AllActive,
                    MatchCase = matchCase
                };

                response = await DocManager.GetEmployeeAgreementsAsync(request); // AgreementStates state
            }

            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Progress Response");

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

        private async Task AddMembersAsync(AgreementModel agreement)
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", agreement);

            //new EmployeeAgreementResponse { }
            var parameters = new DialogParameters() { { nameof(MembersDialog._agreement), agreement } };

            var dialog = _dialogService.Show<MembersDialog>("", parameters); //, options

            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                // ReloadAgreements ?
            }
        }

        //TableRowClickEventArgs<AgreementModel> e
        private async Task OnRowClickAsync()
        {
            if (!_loaded) return;
            _selectedItems.RemoveWhere(i => i.AgreementId == _agreement.AgreementId);
            await ShowInProcessAsync(_agreement);
        }
        private async Task ShowInProcessAsync(AgreementModel a)
        {
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var doc = AgreementToDoc(a);
            var parameters = new DialogParameters() {{ nameof(InProgressDialog.Doc), doc }};
            var options = new DialogOptions { CloseButton = true };

            var dialog = _dialogService.Show<InProgressDialog>("", parameters, options);
            var result = await dialog.Result;

            if (a.Opened == null) { a.Opened = DateTime.Now; }

            if (!result.Cancelled)
            {
                var action = result.Data.ToString();
                await _jsRuntime.InvokeVoidAsync("azino.Console", action, "ACTION: ");

                _loaded = false;
                StateHasChanged();

                if (action == nameof(AgreementActions.ToReview)) { await VerifyAnAgreementAsync(a); }
                else if (action == nameof(AgreementActions.ToRefuse)) { await RefuseAnAgreementAsync(a); }
                else if (action == nameof(AgreementActions.ToApprove)) { await ApproveAnAgreementAsync(a); }
                else if (action == nameof(AgreementActions.ToSign)) { await SignAnAgreementAsync(a); }
                else if (action == nameof(AgreementActions.ToReject)) { await RejectAnAgreementAsync(a); }
                else if (action == nameof(AgreementActions.AddMembers)) { await AddMembersAsync(a); }

                await _mudTable.ReloadServerData();
            }
            //await GetAgreementsAsync(AgreementStates.AllActive);
        }

        private async Task<string> CreateSignAsync(AgreementModel agreement)
        {
            var thumbprint = await _localStorage.GetItemAsync<string>(StorageConstants.Local.UserThumbprint);
            var base64 = await DocManager.GetBase64Async(agreement.DocURL);
            var sign = await _jsRuntime.InvokeAsync<string>("azino.SignCadesBES", thumbprint, base64, agreement.DocTitle);

            if (string.IsNullOrWhiteSpace(sign))
            {
                _snackBar.Add(_localizer["Signing failed"], Severity.Error);
                return string.Empty;
            }

            AgreementSignedCommand cmdSigned = new()
            {
                AgreementId = agreement.AgreementId,
                EmplId = agreement.EmplId,
                EmplOrgId = agreement.EmplOrgId,
                DocId = agreement.DocId,
                //Thumbprint = thumbprint,

                Data = Convert.FromBase64String(sign)//Encoding.ASCII.GetBytes(base64)
            };

            var response = await DocManager.PostAgreementSignedAsync(cmdSigned);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return string.Empty;
            }

            return thumbprint;
        }

        private async Task VerifyAnAgreementAsync(AgreementModel agreement)
        {
            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Verifed,
                Remark = "",
                Members = new(),

                URL = "",
                UploadRequest = null
            };

            await SendAgreementAnswerAsync(command);
        }
        private async Task ApproveAnAgreementAsync(AgreementModel agreement)
        {
            var thumbprint = await CreateSignAsync(agreement);
            if (string.IsNullOrEmpty(thumbprint)) { return; } // Error !!!

            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Approved,
                Remark = "",
                Members = new(),
                Thumbprint = thumbprint,

                URL = "",
                UploadRequest = null
            };

            await SendAgreementAnswerAsync(command);
        }
        private async Task SignAnAgreementAsync(AgreementModel agreement)
        {
            var thumbprint = await CreateSignAsync(agreement);
            if (string.IsNullOrEmpty(thumbprint)) { return; } // Error !!!

            AgreementAnswerCommand cmdAnswer = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Signed,
                Remark = "",
                Members = new(),
                Thumbprint = thumbprint,

                URL = "",
                UploadRequest = new()
            };

            await SendAgreementAnswerAsync(cmdAnswer);
            //await _mudTable.ReloadServerData();
            //await GetAgreementsAsync(AgreementStates.AllActive);
        }
        private async Task RejectAnAgreementAsync(AgreementModel agreement)
        {
            var dialog = _dialogService.Show<AgreementRejectDialog>(""); //, options, parameters
            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Rejected,
                Remark = result.Data.ToString(),
                Members = new(),

                URL = "",
                UploadRequest = null
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", command);

            await SendAgreementAnswerAsync(command);
        }
        private async Task RefuseAnAgreementAsync(AgreementModel agreement)
        {
            var dialog = _dialogService.Show<AgreementRejectDialog>(""); //, options, parameters
            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Refused,
                Remark = result.Data.ToString(),
                Members = new(),

                URL = "",
                UploadRequest = null
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", command);

            await SendAgreementAnswerAsync(command);
        }
        private async Task SendAgreementAnswerAsync(AgreementAnswerCommand command)
        {
            var response = await DocManager.PostAgreementAnswerAsync(command);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            await _mudTable.ReloadServerData();
            //await GetAgreementsAsync(AgreementStates.AllActive);
        }

        private async Task SignSelectedItemsAsync()
        {
            //if (_selectedItems?.Where(i => i.Action == AgreementActions.ToSign && !i.ActionBlocked).Any() == false) return;
            var agrs = _selectedItems.Where(i => i.Action == ActTypes.Signing && !i.ActionBlocked).ToArray();
            await _jsRuntime.InvokeVoidAsync("azino.Console", agrs, "To Sign: ");

            var parameters = new DialogParameters() {{ nameof(ItemsToSignDialog.Agrs), agrs }};
            var options = new DialogOptions { CloseButton = false };

            var dialog = _dialogService.Show<ItemsToSignDialog>("", parameters, options);
            var result = await dialog.Result;

            _selectedItems.Clear();

            await _mudTable.ReloadServerData();
        }
        private async Task ApproveSelectedItemsAsync()
        {
            var agrs = _selectedItems.Where(i => i.Action == ActTypes.Agreement && !i.ActionBlocked).ToArray();
            await _jsRuntime.InvokeVoidAsync("azino.Console", agrs, "To Approve: ");

            var parameters = new DialogParameters() { { nameof(ItemsToAgreeDialog.Agrs), agrs } };
            var options = new DialogOptions { CloseButton = false };

            var dialog = _dialogService.Show<ItemsToAgreeDialog>("", parameters, options);
            var result = await dialog.Result;

            _selectedItems.Clear();

            await _mudTable.ReloadServerData();
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
                DocTypeName = a.DocTypeName,//(a.DocTypeId == 1) ? "Договор" : "Доп.соглашение",  //a.DocTypeName,
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
                DocCurrentStep = a.DocCurrentStep,
                DocTotalSteps = a.DocTotalSteps,

                DocVersion = a.DocVersion,
                DocCreatedBy = a.DocCreatedBy,
                DocCreatedOn = a.DocCreatedOn.AddHours(tz),
                DocCreatedOnStr = a.DocCreatedOn.AddHours(tz).ToString("g"),

                Step = a.Step,
                State = a.State,
                Action = a.Action,
                ActionBlocked = !(a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened),
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
        private static string UnreadRowClassFunc(AgreementModel a, int _) =>
            (a.Opened == null) ? "unread" : string.Empty;

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

        //private void OnStage(string stage)
        //{
        //    if (stage == "Draft") Filter.StageDraft = !Filter.StageDraft;
        //    else if (stage == "InProgress") Filter.StageInProgress = !Filter.StageInProgress;
        //    else if (stage == "Rejected") Filter.StageRejected = !Filter.StageRejected;

        //    Filter.ChangedStage = Filter.StageDraft != FilterDefault.StageDraft ||
        //        Filter.StageInProgress != FilterDefault.StageInProgress ||
        //        Filter.StageRejected != FilterDefault.StageRejected;

        //    FilterIsActive();
        //    //FilterIsEmpty();
        //}
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
            Filter.IsActive = Filter.ChangedAttendee || Filter.ChangedType
                || Filter.ChangedText || Filter.ChangedDate || Filter.ChangedCreateOn;
        }
        //private void FilterIsEmpty() { Filter.IsEmpty = !(Filter.TypeContract || Filter.TypeAgreement); }
        private void FilterReset()
        {
            orgContact = null;
            employeeContact = null;
            Filter.OrgId = FilterDefault.OrgId;
            Filter.UserId = FilterDefault.UserId;

            //Filter.StageDraft = FilterDefault.StageDraft;
            //Filter.StageInProgress = FilterDefault.StageInProgress;
            //Filter.StageRejected = FilterDefault.StageRejected;

            Filter.DocTypes.Clear();
            FilterDefault.DocTypes.ForEach(dt => Filter.DocTypes.Add(new FilterDocTypeModel(dt)));

            Filter.TextNumber = FilterDefault.TextNumber;
            Filter.TextTitle = FilterDefault.TextTitle;

            Filter.DateFrom = FilterDefault.DateFrom;
            Filter.DateTo = FilterDefault.DateTo;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedAttendee = false;
            //Filter.ChangedStage = false;
            Filter.ChangedType = false;
            Filter.ChangedText = false;
            Filter.ChangedDate = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }

        //private async Task RemarkAnAgreementAsync(AgreementModel agreement)
        //{
        //    await _jsRuntime.InvokeVoidAsync("azino.Console", agreement);

        //    // Show AgreementRemarkDialog
        //}

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
