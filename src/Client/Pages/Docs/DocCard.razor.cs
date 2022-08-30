using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Features.Documents.Commands;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Docs
{
    public partial class DocCard
    {
        [Parameter] public int? DocId { get; set; }
        [Parameter] public int? RouteId { get; set; }

        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDocumentTypeManager DocTypeManager { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }

        private RouteCardResponse Route { get; set; } = new();
        private FileParseModel Pattern { get; set; } = new();

        private IEnumerable<DocTypeResponse> RouteDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        //private IEnumerable<ContactResponse> ChiefsOfFund { get; set; } = new List<ContactResponse>();
        //private IEnumerable<ContactResponse> ChiefsOfSMO { get; set; } = new List<ContactResponse>();

        private List<DocTypeResponse> _allDocTypes = new();
        public AddEditDocCommand Doc { get; set; } = new();
        private List<DocActModel> Acts { get; set; } = new();

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;

        private ClaimsPrincipal _authUser;
        private IBrowserFile _file;
        private bool onUpload = false;

        private int tz;
        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            if (RouteId is null || RouteId == 0) { NavigateToDocs(); }

            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadDataAsync();
            StateHasChanged();
        }

        private void NavigateToDocs() => _navigationManager.NavigateTo($"/docs");

        private async Task LoadDataAsync()
        {
            var data = await DocTypeManager.GetAllAsync();
            if (data.Succeeded) { _allDocTypes = data.Data; }

            await RouteInitAsync(RouteId);

            if (DocId is not null && DocId != 0)
            {
                await LoadDocAsync(DocId);
            }
            else
            {
                var emplId = _authUser.GetUserId();

                if (int.TryParse(_authUser.GetOrgId(), out int emplOrgId))
                {
                    await NewDocInitAsync(emplId, emplOrgId);
                }
            }
        }
        private async Task RouteInitAsync(int? id)
        {
            var response = await DirManager.GetRouteCardAsync((int)id);
            Route = response.Data;

            Route.Parses.ForEach(c => _ = c.PatternType switch
            {
                ParsePatterns.Sample => Pattern.FileName = c.Pattern,
                ParsePatterns.Mask => Pattern.FileMask = c.Pattern,
                ParsePatterns.Accept => Pattern.FileAccept = c.Pattern,

                ParsePatterns.DocTitle => Pattern.DocTitle = c.Pattern,
                ParsePatterns.DocNumber => Pattern.DocNumber = c.Pattern,
                ParsePatterns.DocDate => Pattern.DocDate = c.Pattern,
                ParsePatterns.DocNotes => Pattern.DocNotes = c.Pattern,

                ParsePatterns.CodeMO => Pattern.CodeMo = c.Pattern,
                ParsePatterns.CodeSMO => Pattern.CodeSmo = c.Pattern,
                ParsePatterns.CodeFund => Pattern.CodeFund = c.Pattern,

                _ => null
            });

            var mask = (Route.ParseFileName && !string.IsNullOrWhiteSpace(Pattern.FileMask)) ? Pattern.FileMask : "*";

            await _jsRuntime.InvokeVoidAsync("azino.Console", mask, $"File Mask");


            RouteDocTypes = Route.DocTypeIds.Select(id => _allDocTypes.Find(t => t.Id == id)).ToHashSet();
        }

        private async Task<IEnumerable<ContactResponse>> SearchContactsAsync(DocActModel act, string search)
        {
            var request = new SearchContactsRequest()
            {
                BaseRole = act.Step.MemberGroup == MemberGroups.OnlyHead ? UserBaseRoles.Chief : UserBaseRoles.Undefined,
                OrgType = act.Step.OrgType,
                SearchString = search
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }

        private async Task NewDocInitAsync(string emplId, int emplOrgId)
        {
            Doc.EmplId = emplId;
            Doc.EmplOrgId = emplOrgId;

            Doc.TypeId = (Route.DocTypeIds.Count > 0) ? Route.DocTypeIds[0] : 1;
            Doc.Date = Route.DateIsToday ? DateTime.Today : null;

            Doc.RouteId = Route.Id;
            Doc.TotalSteps = Route.Stages.Count; // TotalSteps - первоначальное название этапов

            //Route.Steps.ForEach(step =>
            foreach (var step in Route.Steps)
            {
                var act = new DocActModel() { Step = step, Contact = null };

                //if (step.OnlyHead)
                //{
                //    if (step.OrgType == OrgTypes.Fund) { AddContacts(act, ChiefsOfFund); }
                //    else if (step.OrgType == OrgTypes.SMO) { AddContacts(act, ChiefsOfSMO); }
                //}

                if (step.Members.Count > 0)
                {
                    AddContacts(act, step.Members.Select(m => m.Contact).ToList());
                }

                if (step.AutoSearch > 0)
                {
                    //var take = step.SomeParticipants ? step.AutoSearch : 1;
                    var members = await LoadMembersAsync(step.OrgType, step.AutoSearch, step.MemberGroup == MemberGroups.OnlyHead);
                    AddContacts(act, members);
                }

                Acts.Add(act);
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", Doc, "New Doc");
            await _jsRuntime.InvokeVoidAsync("azino.Console", Route, "Route");
            await _jsRuntime.InvokeVoidAsync("azino.Console", Acts, "Acts");
        }
        private async Task<List<ContactResponse>> LoadMembersAsync(OrgTypes orgType, int take, bool isChief)
        {
            var request = new SearchContactsRequest()
            {
                Take = take,
                BaseRole = isChief ? UserBaseRoles.Chief : UserBaseRoles.Undefined,
                OrgType = orgType,
                SearchString = ""
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }

        private async Task LoadDocAsync(int? docId) { }

        private static void AddContacts(DocActModel act, IEnumerable<ContactResponse> contacts)
        {
            foreach (var c in contacts)
            {
                act.Contact = c;// CloneContact(c);
                AddContact(act);
            }
        }
        private static void AddContact(DocActModel act)
        {
            var name = ContactName(act.Contact);

            if (!act.Members.Exists(m => m.Label == name))
            {
                act.Members.Add(NewMember(act));
            }

            act.Contact = null;
        }
        private static MemberModel NewMember(DocActModel act)
        {
            var member = new MemberModel()
            {
                Label = ContactName(act.Contact),

                Act = act.Step.ActType,
                IsAdditional = false,
                UserId = act.Contact.Id,

                Contact = act.Contact
            };

            act.Contact = null;

            return member;
        }
        private static void DelContact(DocActModel act, MudChip chip)
        {
            var member = act.Members.Find(m => m.Label == chip.Text);
            act.Members.Remove(member);
        }

        //private static ContactResponse CloneContact(ContactResponse c) => new()
        //{
        //    Id = c.Id,
        //    OrgId = c.OrgId,
        //    InnLe = c.InnLe,

        //    Surname = c.Surname,
        //    GivenName = c.GivenName
        //};
        private static string ContactName(ContactResponse c) =>
            $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";

        private static string IconByOrgType(OrgTypes ot) => ot switch
        {
            OrgTypes.Fund => Icons.Material.Outlined.HealthAndSafety,
            OrgTypes.MO => Icons.Material.Outlined.MedicalServices,
            OrgTypes.SMO => Icons.Material.Outlined.Museum,
            OrgTypes.MEO => Icons.Material.Outlined.LocalPolice, //MilitaryTech,
            OrgTypes.Treasury => Icons.Material.Outlined.AccountBalance,
            _ => Icons.Material.Outlined.Business
        };
        private string LabelByStep(RouteStepModel step) =>
            $"{_localizer[(step.MemberGroup == MemberGroups.OnlyHead) ? "Chief" : "Employee"]} " +
            $"{_localizer["of the"]} {_localizer[step.OrgType.ToString()]}";
        private List<DocActModel> ActsByStage(int stageNumber) => Acts.Where(a => a.Step.StageNumber == stageNumber).ToList();

        private void Close() => NavigateToDocs();
        private async Task SendAsync() => await SaveAsync(1);
        private async Task SaveAsync(int stageNumber = 0) // 0 - Draft, 1 - Send
        {
            var errors = 0;

            if (stageNumber == 1 && (_file == null || string.IsNullOrEmpty(_file.Name)))
            {
                errors++;
                _snackBar.Add($"{_localizer["Doc File Required"]}", Severity.Warning);
            }
            else if (string.IsNullOrWhiteSpace(Doc.Title))
            {
                errors++;
                _snackBar.Add($"{_localizer["Doc Title Required"]}", Severity.Warning);
            }

            Acts.ForEach(a =>
            {
                if (a.Step.Requred && a.Contact == null && a.Members.Count == 0)
                {
                    errors++;
                    _snackBar.Add($"{_localizer["Contact Required"]} {_localizer[a.Step.OrgType.ToString()]}", Severity.Warning);
                }
            });

            if (errors > 0) { return; }

            Doc.CurrentStep = stageNumber;
            Doc.TotalSteps = Route.Stages.Count;

            Doc.Members.Clear();
            Doc.Members.Add(new ActMember() // Инициатор подписания (исполнитель)
            {
                StepId = null,
                IsAdditional = false,
                Act = ActTypes.Initiation,

                OrgInn = _authUser.GetInnLe(),
                OrgId = Doc.EmplOrgId,
                EmplId = Doc.EmplId
            });

            Acts.ForEach(a =>
            {
                if (a.Contact != null && !a.Members.Any(m => m.Label == ContactName(a.Contact)))
                {
                    Doc.Members.Add(NewActMember(a, a.Contact));
                }

                foreach (var c in a.Members)
                {
                    Doc.Members.Add(NewActMember(a, c.Contact));
                }
            });

            onUpload = true;

            var response = await DocManager.PostDocAsync(Doc);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                NavigateToDocs();
            }
            else
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }

            onUpload = false;
        }

        private static ActMember NewActMember(DocActModel a, ContactResponse c)
        {
            return new()
            {
                StepId = a.Step.Id,
                IsAdditional = false,
                Act = a.Step.ActType,
                //(a.Step.ActType == ActTypes.Signing) ? AgreementActions.ToSign :
                //(a.Step.ActType == ActTypes.Agreement) ? AgreementActions.ToApprove :
                //(a.Step.ActType == ActTypes.Review) ? AgreementActions.ToReview : AgreementActions.Undefined,

                OrgInn = c.InnLe,
                OrgId = c.OrgId,
                EmplId = c.Id
            };
        }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            if (e.File is null) { return; }

            _file = e.File;

            if (Route.NameOfFile) { Doc.Title = Path.GetFileNameWithoutExtension(_file.Name); }

            if (Route.ParseFileName && !string.IsNullOrWhiteSpace(Pattern.FileMask))
            {
                var maskIsCorrect = false;

                try
                {
                    Regex mask = new(Pattern.FileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
                    maskIsCorrect = mask.IsMatch(_file.Name);
                }
                catch (Exception) {}

                if (!maskIsCorrect)
                {
                    // Route.ProtectedMode;
                    // Dialog - Accept || Decline
                }
            }

            var buffer = new byte[_file.Size];
            await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

            Doc.URL = $"data:application/octet-stream;base64,{Convert.ToBase64String(buffer)}";

            Doc.UploadRequest = new UploadRequest
            {
                Data = buffer,
                UploadType = UploadType.Document,
                FileName = Path.GetFileName(_file.Name),
                Extension = Path.GetExtension(_file.Name)
            };
        }
    }
}
