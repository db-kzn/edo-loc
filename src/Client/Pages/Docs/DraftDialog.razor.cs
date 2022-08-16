using Blazored.FluentValidation;
using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Docs
{
    public partial class DraftDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDocumentTypeManager DocTypeManager { get; set; }
        private List<DocTypeResponse> _docTypes = new();

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public AddEditDocumentCommand _doc { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => options.IncludeAllRuleSets());

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = true;

        private IBrowserFile _file;
        private bool onUpload = false;

        // Step 1
        public ContactResponse fundContact;
        public Dictionary<string, ContactResponse> FundContacts = new();

        // Step 2
        public ContactResponse smoContact;
        public Dictionary<string, ContactResponse> SmoContacts = new();

        // Step 3
        public ContactResponse moContact;
        public Dictionary<string, ContactResponse> MoContacts = new();

        // Step 4
        public ContactResponse headContact;
        public Dictionary<string, ContactResponse> HeadContacts = new();

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;
            var user = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _doc.EmplId = user.GetUserId();
            _doc.EmplOrgId = Convert.ToInt32(user.GetOrgId());

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadDataAsync(_doc.Id);
        }

        private async Task LoadDataAsync(int docId)
        {
            await LoadDocumentTypesAsync();

            if (docId == 0)
            {
                await LoadHeadsOfFundAsync();
                await LoadChiefsOfSmoAsync();
            }
            else
            {
                await LoadDocAgreementsAsync(docId);
            }
        }

        private async Task LoadHeadsOfFundAsync() =>
            await LoadContactsAsync(HeadContacts, UserBaseRoles.Chief, OrgTypes.Fund, "");

        private async Task LoadChiefsOfSmoAsync() =>
            await LoadContactsAsync(SmoContacts, UserBaseRoles.Chief, OrgTypes.SMO, "");

        private async Task LoadContactsAsync(
            Dictionary<string, ContactResponse> dic,
            UserBaseRoles userBaseRole,
            OrgTypes orgType,
            string searchString
            )
        {
            var request = NewSearchContactsRequest(userBaseRole, orgType, searchString);
            var contacts = (List<ContactResponse>) await SearchContactsAsync(request);
            AddContacts(dic, contacts);
        }

        private async Task LoadDocumentTypesAsync()
        {
            var data = await DocTypeManager.GetAllAsync();

            if (data.Succeeded)
            {
                _docTypes = data.Data;
                //await _jsRuntime.InvokeVoidAsync("azino.Console", _docTypes);
            }
        }

        private async Task LoadDocAgreementsAsync(int id)
        {
            var response = await DocManager.GetDocAgreementsAsync(id);

            if (!response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }

                return;
            }

            var agreements = response.Data;
            await _jsRuntime.InvokeVoidAsync("azino.Console", agreements);

            agreements.ForEach((a) =>
            {
                var c = new ContactResponse()
                {
                    Id = a.EmplId,
                    Surname = a.Surname,
                    GivenName = a.GivenName,

                    OrgId = a.OrgId,
                    OrgShortName = a.OrgShortName,
                    InnLe = a.OrgInn
                };

                // Step, OrgType
                if (a.Step == 1)
                {
                    AddContact(FundContacts, ref c);
                }
                else if (a.Step == 3)
                {
                    AddContact(HeadContacts, ref c);
                }
                else if (a.Step == 2)
                {
                    if (a.OrgType == OrgTypes.SMO)
                    {
                        AddContact(SmoContacts, ref c);
                    }
                    else if (a.OrgType == OrgTypes.MO)
                    {
                        AddContact(MoContacts, ref c);
                    }
                }
            });

        }

        // If there are many (>7) types of documents:
        //private async Task<IEnumerable<int>> SearchDocumentTypes(string value)
        //{
        //    // if text is null or empty, show complete list
        //    if (string.IsNullOrEmpty(value))
        //        return _documentTypes.Select(x => x.Id);

        //    return _documentTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        //        .Select(x => x.Id);
        //}

        public void Cancel() => MudDialog.Cancel();

        private async Task SaveAsync(int step = 0)
        {
            var errors = 0;

            List<(string, int)> FundIds = ContactsToIds(FundContacts.Values.ToList());
            List<(string, int)> SmoIds = ContactsToIds(SmoContacts.Values.ToList());
            List<(string, int)> MoIds = ContactsToIds(MoContacts.Values.ToList());
            List<(string, int)> HeadIds = ContactsToIds(HeadContacts.Values.ToList());

            if (fundContact != null && !HasContact(FundContacts, fundContact)) { FundIds.Add(new (fundContact.Id, fundContact.OrgId)); }
            if (smoContact != null && !HasContact(SmoContacts, smoContact)) { SmoIds.Add(new (smoContact.Id, smoContact.OrgId)); }
            if (moContact != null && !HasContact(MoContacts, moContact)) { MoIds.Add(new (moContact.Id, moContact.OrgId)); }
            if (headContact != null && !HasContact(HeadContacts, headContact)) { HeadIds.Add(new (headContact.Id, headContact.OrgId)); }

            if (SmoIds.Count == 0)
            {
                errors++;
                _snackBar.Add(_localizer["SMO Contact Required"], Severity.Warning);
            }
            if (MoIds.Count == 0)
            {
                errors++;
                _snackBar.Add(_localizer["MO Contact Required"], Severity.Warning);
            }
            if (HeadIds.Count == 0)
            {
                errors++;
                _snackBar.Add(_localizer["Chief of Fund Contact Required"], Severity.Warning);
            }

            if (errors > 0) { return; }

            onUpload = true;

            _doc.Contacts = new();
            _doc.CurrentStep = step;
            _doc.TotalSteps = 3;

            FundIds.ForEach(c => _doc.Contacts.Add(new() { Step = 1, EmplId = c.Item1, OrgId = c.Item2, Action = AgreementActions.ToApprove }));
            SmoIds.ForEach(c => _doc.Contacts.Add(new() { Step = 2, EmplId = c.Item1, OrgId = c.Item2, Action = AgreementActions.ToSign }));
            MoIds.ForEach(c => _doc.Contacts.Add(new() { Step = 2, EmplId = c.Item1, OrgId = c.Item2, Action = AgreementActions.ToSign }));
            HeadIds.ForEach(c => _doc.Contacts.Add(new() { Step = 3, EmplId = c.Item1, OrgId = c.Item2, Action = AgreementActions.ToSign }));

            _doc.Contacts.Add(new() { Step = 0, EmplId = _doc.EmplId, OrgId = _doc.EmplOrgId, Action = AgreementActions.ToRun });

            await _jsRuntime.InvokeVoidAsync("azino.Console", _doc);

            var response = await DocManager.PostAsync(_doc);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }

            onUpload = false;
        }

        private async Task SendAsync() => await SaveAsync(1);

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;

            if (_file != null)
            {
                var buffer = new byte[_file.Size];
                var extension = Path.GetExtension(_file.Name);
                var fileName = Path.GetFileName(_file.Name);

                _doc.Title = Path.GetFileNameWithoutExtension(_file.Name);
                //if (string.IsNullOrWhiteSpace(_doc.Title)) {}

                const string format = "application/octet-stream";

                await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

                _doc.URL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";

                _doc.UploadRequest = new UploadRequest
                {
                    Data = buffer,
                    UploadType = UploadType.Document,
                    FileName = fileName,
                    Extension = extension
                };

                //await _jsRuntime.InvokeVoidAsync("azino.Console", _doc);
            }
        }

        public void AddFund() => AddContact(FundContacts, ref fundContact);
        public void DelFund(MudChip chip) => FundContacts.Remove(chip.Text);

        public void AddSmo() => AddContact(SmoContacts, ref smoContact);
        public void DelSmo(MudChip chip) => SmoContacts.Remove(chip.Text);

        public void AddMo() => AddContact(MoContacts, ref moContact);
        public void DelMo(MudChip chip) => MoContacts.Remove(chip.Text);

        public void AddHead() => AddContact(HeadContacts, ref headContact);
        public void DelHead(MudChip chip) => HeadContacts.Remove(chip.Text);

        public async Task<IEnumerable<ContactResponse>> SearchFundContactsAsync(string search)
        {
            var request = NewSearchContactsRequest((int)UserBaseRoles.Undefined, OrgTypes.Fund, search);
            return await SearchContactsAsync(request);
        }

        public async Task<IEnumerable<ContactResponse>> SearchSmoContactsAsync(string search)
        {
            var request = NewSearchContactsRequest(UserBaseRoles.Chief, OrgTypes.SMO, search);
            return await SearchContactsAsync(request);
        }

        public async Task<IEnumerable<ContactResponse>> SearchMoContactsAsync(string search)
        {
            var request = NewSearchContactsRequest(UserBaseRoles.Chief, OrgTypes.MO, search);
            return await SearchContactsAsync(request);
        }

        public async Task<IEnumerable<ContactResponse>> SearchHeadContactsAsync(string search)
        {
            var request = NewSearchContactsRequest(UserBaseRoles.Chief, OrgTypes.Fund, search);
            return await SearchContactsAsync(request);
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
            return (response.Succeeded) ? response.Data : new();
        }

        private static string ContactName(ContactResponse c)
        {
            return $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";
        }

        private static void AddContacts(Dictionary<string, ContactResponse> dic, List<ContactResponse> list)
        {
            foreach (var h in list)
            {
                ContactResponse head = CloneContact(h);
                AddContact(dic, ref head);
            }
        }

        private static void AddContact(Dictionary<string, ContactResponse> dic, ref ContactResponse c)
        {
            if (!HasContact(dic, c))
            {
                dic.Add(ContactName(c), CloneContact(c));
            }
            c = null;
        }

        private static ContactResponse CloneContact(ContactResponse c)
        {
            return new()
            {
                Id = c.Id,
                OrgId = c.OrgId,
                InnLe = c.InnLe,

                Surname = c.Surname,
                GivenName = c.GivenName
            };
        }

        private static bool HasContact(Dictionary<string, ContactResponse> dic, ContactResponse c)
        {
            return dic.ContainsKey(ContactName(c));
        }

        private static List<(string, int)> ContactsToIds(List<ContactResponse> list)
        {
            List<(string, int)> ids = new();
            list.ForEach(l => ids.Add(new (l.Id, l.OrgId)));
            //foreach (var l in list) { ids.Add(l.Id); }
            return ids;
        }
    }
}
