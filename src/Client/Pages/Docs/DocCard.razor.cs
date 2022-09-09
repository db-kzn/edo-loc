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
using EDO_FOMS.Client.Infrastructure.Managers.Orgs;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        [Inject] private IOrgManager OrgManager { get; set; }

        private RouteCardResponse Route { get; set; } = new();
        private FileParseModel Pattern { get; set; } = new();

        private IEnumerable<DocTypeResponse> RouteDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        //private IEnumerable<ContactResponse> ChiefsOfFund { get; set; } = new List<ContactResponse>();
        //private IEnumerable<ContactResponse> ChiefsOfSMO { get; set; } = new List<ContactResponse>();

        private List<DocTypeResponse> _allDocTypes = new();
        public AddEditDocCommand Doc { get; set; } = new();
        private List<DocActModel> Acts { get; set; } = new();
        private ContactResponse Executor { get; set; } = null;

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;
        private readonly bool clearable = true;

        private ClaimsPrincipal _authUser;
        private string userId;

        private IBrowserFile _file = null;
        private bool onUpload = false;

        private int tz;
        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            if (RouteId is null || RouteId == 0) { NavigateToDocs(); }

            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            userId = _authUser.GetUserId();

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            var isSucceeded = await LoadDataAsync();

            if (!isSucceeded)
            {
                // Show Error on Init
            }

            StateHasChanged();
        }

        private void NavigateToDocs() => _navigationManager.NavigateTo($"/docs");

        private async Task<bool> LoadDataAsync()
        {
            var data = await DocTypeManager.GetAllAsync();
            var docTypesLoaded = data.Succeeded;
            if (docTypesLoaded) { _allDocTypes = data.Data; }

            var routeLoadSucceeded = await LoadRouteAsync(RouteId);
            var docInitSucceeded = await NewDocInitAsync();
            var docLoadSucceeded = (DocId is null || DocId == 0) || await LoadDocAsync((int)DocId);

            return docTypesLoaded && routeLoadSucceeded && docInitSucceeded && docLoadSucceeded;
        }
        private async Task<bool> LoadRouteAsync(int? id)
        {
            var response = await DirManager.GetRouteCardAsync((int)id);

            if (!response.Succeeded) { return false; }
            Route = response.Data;

            await _jsRuntime.InvokeVoidAsync("azino.Console", Route, "Route");

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

            await _jsRuntime.InvokeVoidAsync("azino.Console", Route, "Route");

            var mask = (Route.ParseFileName && !string.IsNullOrWhiteSpace(Pattern.FileMask)) ? Pattern.FileMask : "*";
            //await _jsRuntime.InvokeVoidAsync("azino.Console", mask, $"File Mask");

            RouteDocTypes = Route.DocTypeIds.Select(id => _allDocTypes.Find(t => t.Id == id)).ToHashSet();

            return true;
        }

        private async Task<IEnumerable<ContactResponse>> SearchExecutorAsync(string search)
        {
            var request = new SearchContactsRequest()
            {
                BaseRole = UserBaseRoles.Undefined,
                OrgType = OrgTypes.Undefined,
                SearchString = search
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
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

        private async Task<bool> NewDocInitAsync()
        {
            if (int.TryParse(_authUser.GetOrgId(), out int emplOrgId))
            {
                Doc.EmplOrgId = emplOrgId;
            }
            else
            {
                // Error 
                return false;
            }

            Doc.EmplId = userId;

            Doc.TypeId = (Route.DocTypeIds.Count > 0) ? Route.DocTypeIds[0] : 1;
            Doc.Date = Route.DateIsToday ? DateTime.Today : null;

            Doc.RouteId = Route.Id;
            Doc.TotalSteps = Route.Stages.Count; // Steps - первоначальное название этапов

            Executor = Route.Executor;

            foreach (var step in Route.Steps)
            {
                var act = new DocActModel() { Step = step, Contact = null };
                AddMembers(act, step.Members);

                if (step.AutoSearch > 0)
                {
                    //var take = step.SomeParticipants ? step.AutoSearch : 1;
                    var contacts = await LoadMembersAsync(step.OrgType, step.AutoSearch, step.MemberGroup == MemberGroups.OnlyHead, step.OrgId);
                    AddMainContacts(act, contacts);
                }

                Acts.Add(act);
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", Doc, "NEW Doc");
            await _jsRuntime.InvokeVoidAsync("azino.Console", Acts, "Route Acts");

            return true;
        }
        private async Task<List<ContactResponse>> LoadMembersAsync(OrgTypes orgType, int take, bool isChief, int? orgId = null)
        {
            var request = new SearchContactsRequest()
            {
                Take = take,
                BaseRole = isChief ? UserBaseRoles.Chief : UserBaseRoles.Undefined,
                OrgType = orgType,

                SearchString = "",
                OrgId = orgId
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }

        private async Task<bool> LoadDocAsync(int id)
        {
            var response = await DocManager.GetDocCardAsync(id);
            if (!response.Succeeded) { return false; }

            var doc = response.Data;

            Doc.Id = doc.Id;
            // AddEditDocCommand |> DocCardModel : PreviousId
            Doc.ParentId = doc.ParentId;
            if (RouteId != doc.RouteId) { } // Error !!

            // From System :                       EmplId, EmplOrgId
            //Doc.ExecutorId = doc.ExecutorId;
            if (Route.Executor is null) { Executor = doc.Executor; }

            Doc.IsPublic = doc.IsPublic;
            Doc.TypeId = doc.TypeId;

            Doc.Number = doc.Number;
            Doc.Date = doc.Date;
            Doc.Title = doc.Title;
            Doc.Description = doc.Description;

            // Draft:                              Stage = Draft, CurrentStep = 0;
            // From Route :                        TotalSteps

            Doc.URL = doc.URL;
            Doc.UploadRequest = new() { FileName = doc.FileName };

            // Agreements => Acts
            foreach (var agr in doc.Agreements)
            {
                var act = Acts.FirstOrDefault(a => a.Step.Id == agr.RouteStepId);
                if (act is null) { continue; }

                if (!act.Members.Exists(a => a.UserId == agr.EmplId))
                {
                    act.Members.Add(new()
                    {
                        Contact = agr.Contact,
                        Label = ContactName(agr.Contact),

                        Act = agr.Action,
                        IsAdditional = agr.IsAdditional,
                        UserId = agr.EmplId
                    });
                }
            }

            await _jsRuntime.InvokeVoidAsync("azino.Console", doc, "LOAD Doc");
            await _jsRuntime.InvokeVoidAsync("azino.Console", Acts, "Doc Acts");

            return true;
        }

        private static void AddMainContacts(DocActModel act, IEnumerable<ContactResponse> contacts)
        {
            foreach (var c in contacts)
            {
                act.Contact = c;// CloneContact(c);
                AddMainContact(act);
            }
        }
        private static void AddMainContact(DocActModel act)
        {
            if (act.Contact is null) { return; }

            if (!act.Members.Exists(m => m.Contact.Id != act.Contact.Id))
            {
                act.Members.Add(NewMainMember(act));
            }

            act.Contact = null;
        }
        private static MemberModel NewMainMember(DocActModel act)
        {
            var member = new MemberModel()
            {
                Contact = act.Contact,
                Label = ContactName(act.Contact),

                Act = act.Step.ActType,
                IsAdditional = false,
                UserId = act.Contact.Id
            };

            act.Contact = null;

            return member;
        }

        private static void AddMembers(DocActModel act, List<RouteStepMemberModel> members)
        {
            foreach (var m in members)
            {
                var member = new MemberModel()
                {
                    Label = ContactName(m.Contact),

                    Act = m.Act,
                    IsAdditional = m.IsAdditional,
                    UserId = m.UserId,

                    Contact = m.Contact
                };

                act.Members.Add(member);
            }
        }
        private static void DelContact(DocActModel act, MudChip chip)
        {
            var member = act.Members.Find(m => m.Label == chip.Text);
            act.Members.Remove(member);
        }
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

        private async Task SendAsync() => await SaveAsync(1);
        private async Task SaveAsync(int stageNumber = 0) // 0 - Draft, 1 - Send
        {
            var errors = 0;

            if (stageNumber == 1 && ((Doc.Id == 0 && _file == null) || (Doc.Id > 0 && _file == null && string.IsNullOrEmpty(Doc.URL))))
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
                if (a.Step.Requred && a.Contact is null && a.Members.Count == 0)
                {
                    errors++;
                    _snackBar.Add($"{_localizer["Contact Required"]} {_localizer[a.Step.OrgType.ToString()]}", Severity.Warning);
                }
            });

            if (errors > 0) { return; }

            Doc.CurrentStep = stageNumber;
            Doc.TotalSteps = Route.Stages.Count;
            Doc.ExecutorId = Executor?.Id ?? userId; // Исполнитель

            Doc.Members = new()
            {
                new ActMember()
                {
                    RouteStepId = null,
                    StageNumber = 0,

                    Act = ActTypes.Initiation,
                    IsAdditional = false,

                    OrgInn = _authUser.GetInnLe(),
                    OrgId = Doc.EmplOrgId,
                    EmplId = Doc.EmplId
                }
            };

            Acts.ForEach(a =>
            {
                AddMainContact(a);
                a.Members.ForEach(m => Doc.Members.Add(NewDocActMember(a, m)));
            });

            await _jsRuntime.InvokeVoidAsync("azino.Console", Doc, "Doc To Save");

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
        private static ActMember NewDocActMember(DocActModel a, MemberModel m)
        {
            return new()
            {
                // Stage & Step
                RouteStepId = a.Step.Id,
                StageNumber = a.Step.StageNumber,

                // Member
                Act = m.Act,
                IsAdditional = false,

                // Contact
                OrgInn = m.Contact.InnLe,
                OrgId = m.Contact.OrgId,
                EmplId = m.Contact.Id
            };
        }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            if (e.File is null) { return; }

            _file = e.File;
            var fileName = Path.GetFileName(_file.Name);

            if (Route.NameOfFile) { Doc.Title = Path.GetFileNameWithoutExtension(_file.Name); }

            if (Route.ParseFileName)
            {
                if (!string.IsNullOrWhiteSpace(Pattern.FileMask))
                {

                    var maskIsCorrect = false;

                    try
                    {
                        Regex mask = new(Pattern.FileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
                        maskIsCorrect = mask.IsMatch(_file.Name);
                    }
                    catch (Exception) { }

                    if (!maskIsCorrect)
                    {
                        // Route.ProtectedMode;
                        // Dialog - Accept || Decline
                    }
                }

                if (!string.IsNullOrWhiteSpace(Pattern.DocTitle))
                {
                    Doc.Title = TryParse(fileName, Pattern.DocTitle);
                }
                if (!string.IsNullOrWhiteSpace(Pattern.DocNumber))
                {
                    Doc.Number = TryParse(fileName, Pattern.DocNumber);
                }
                if (!string.IsNullOrWhiteSpace(Pattern.DocDate))
                {
                    var date = TryParse(fileName, Pattern.DocDate);
                    if (date.Length == 6)
                    {
                        Doc.Date = DateTime.ParseExact(date, "ddMMyy", CultureInfo.InvariantCulture);
                    }
                }
                if (!string.IsNullOrWhiteSpace(Pattern.DocNotes))
                {
                    Doc.Description = TryParse(fileName, Pattern.DocNotes);
                }

                if (!string.IsNullOrWhiteSpace(Pattern.CodeMo))
                {
                    var codeMo  = TryParse(fileName, Pattern.CodeMo);
                    await _jsRuntime.InvokeVoidAsync("azino.Console", codeMo, "Code MO");

                    var orgId = await OrgManager.GetIdByCodeAsync(codeMo);
                    await _jsRuntime.InvokeVoidAsync("azino.Console", orgId, "MO ID");

                    if (orgId.Succeeded && orgId.Data > 0)
                    {
                        var request = new SearchContactsRequest()
                        {
                            BaseRole = UserBaseRoles.Chief,
                            OrgType = OrgTypes.MO,
                            SearchString = string.Empty,

                            OrgId = orgId.Data
                        };

                        var response = await DocManager.GetFoundContacts(request);
                        await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Chiefs of MO");

                        if (response.Succeeded && response.Data.Count > 0)
                        {
                            var contacts = response.Data;
                            Acts.ForEach(a => { 
                                if (a.Step.OrgType == OrgTypes.MO && a.Step.MemberGroup == MemberGroups.OnlyHead)
                                {
                                    AddMainContacts(a, contacts);
                                }
                            });
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(Pattern.CodeSmo))
                {
                    var codeSmo = TryParse(fileName, Pattern.CodeSmo);
                    await _jsRuntime.InvokeVoidAsync("azino.Console", codeSmo, "Code SMO");

                    var orgId = await OrgManager.GetIdByCodeAsync(codeSmo);
                    await _jsRuntime.InvokeVoidAsync("azino.Console", orgId, "SMO ID");

                    if (orgId.Succeeded && orgId.Data > 0)
                    {
                        var request = new SearchContactsRequest()
                        {
                            BaseRole = UserBaseRoles.Chief,
                            OrgType = OrgTypes.SMO,
                            SearchString = string.Empty,

                            OrgId = orgId.Data
                        };

                        var response = await DocManager.GetFoundContacts(request);
                        await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Chiefs of SMO");

                        if (response.Succeeded && response.Data.Count > 0)
                        {
                            var contacts = response.Data;
                            Acts.ForEach(a => {
                                if (a.Step.OrgType == OrgTypes.SMO && a.Step.MemberGroup == MemberGroups.OnlyHead)
                                {
                                    AddMainContacts(a, contacts);
                                }
                            });
                        }
                    }
                }
            }

            var buffer = new byte[_file.Size];
            await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

            Doc.URL = $"data:application/octet-stream;base64,{Convert.ToBase64String(buffer)}";

            Doc.UploadRequest = new UploadRequest
            {
                Data = buffer,
                UploadType = UploadType.Document,
                FileName = fileName,
                Extension = Path.GetExtension(_file.Name)
            };
        }

        private static string TryParse(string text, string pattern)
        {
            var result = string.Empty;

            try
            {
                Match m = Regex.Match(text, pattern, RegexOptions.None, TimeSpan.FromSeconds(2));
                if (m.Success) { result = m.Value; }
            }
            catch (Exception) { }

            return result;
        }

        private static string ActTypesIcon(ActTypes act)
        {
            return act switch
            {
                ActTypes.Signing => Icons.Material.Outlined.Draw,
                ActTypes.Agreement => Icons.Material.Outlined.OfflinePin,
                ActTypes.Review => Icons.Material.Outlined.MapsUgc,
                _ => Icons.Material.Outlined.Circle
            };
        }
        private void Close() => NavigateToDocs();
    }
}
