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
    public partial class AvailableImportDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDocumentTypeManager DocTypeManager { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }
        [Inject] private IOrgManager OrgManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private bool _loaded = false;

        private MudTable<ActiveRouteModel> _mudTable;
        private ActiveRouteModel _selectedRoute;
        private List<ActiveRouteModel> availableImports = null;

        private List<DocTypeResponse> _allDocTypes = new();

        private RouteCardResponse Route { get; set; } = new();
        private IEnumerable<DocTypeResponse> RouteDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        private FileParseModel Pattern { get; set; } = new();

        public AddEditDocCommand BlankDoc { get; set; } = new();
        private List<DocActModel> BlankActs { get; set; } = new();
        private ContactResponse BlankExecutor { get; set; } = null;

        private ClaimsPrincipal _authUser;
        private string emplId;
        private int emplOrgId;

        private enum ImportStates
        {
            Undefined = 0,

            ChooseRoute = 1,
            InProcess = 2,
            Сompleted = 3
        }
        private ImportStates importState = ImportStates.ChooseRoute;
        private int importErrors = 0;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            emplId = _authUser.GetUserId();
            emplOrgId = int.Parse(_authUser.GetOrgId());

            var result = await DocManager.CheckForImportsAsync();
            await _jsRuntime.InvokeVoidAsync("azino.Console", result, "For Imports");
            if (result.Succeeded) { availableImports = result.Data; }

            var data = await DocTypeManager.GetAllAsync();
            if (data.Succeeded) { _allDocTypes = data.Data; }

            _loaded = true;
        }

        void Cancel() => MudDialog.Cancel(); //void Close() => MudDialog.Close(DialogResult.Cancel());
        void Ok() => MudDialog.Close(DialogResult.Ok(true));

        async Task OnRowClickAsync()
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", _selectedRoute, "Selected Route");
            if (_selectedRoute.Count == 0) { Cancel(); }

            importState = ImportStates.InProcess;
            StateHasChanged();

            await ImportFiles();
        }

        private async Task ImportFiles()
        {
            importErrors = 0;

            // + Определить ID маршрута
            var routeId = _selectedRoute.Id;

            // + Получить маршрут отправки документов
            var routeLoadSucceeded = await LoadRouteAsync(routeId);
            if (!routeLoadSucceeded) { Cancel(); }

            // + Создать Бланк документа и процессов заданного маршрута
            var blankDocInitSucceeded = await NewBlankDocInitAsync();

            // + Получить список файлов
            List<string> importFiles = null;
            var files = await DocManager.GetImportFilesAsync(routeId);
            var filesLoadSucceeded = files.Succeeded;
            if (filesLoadSucceeded) { importFiles = files.Data; }
            await _jsRuntime.InvokeVoidAsync("azino.Console", importFiles, "Import Files");

            var initiator = new ActMember()
            {
                RouteStepId = null,
                StageNumber = 0,

                Act = ActTypes.Initiation,
                IsAdditional = false,

                OrgInn = _authUser.GetInnLe(),
                OrgId = BlankDoc.EmplOrgId,
                EmplId = BlankDoc.EmplId
            };

            // Последовательно обработать файлы, без загрузки с сервера
            foreach (string fileName in importFiles)
            {
                var doc = CloneDoc(BlankDoc);
                var acts = new List<DocActModel>();
                BlankActs.ForEach(a => acts.Add(NewDocAct(a)));

                if (Route.NameOfFile) { doc.Title = Path.GetFileNameWithoutExtension(fileName); }

                await _jsRuntime.InvokeVoidAsync("azino.Console", fileName, "Import File");

                // Разбор имени файла
                if (Route.ParseFileName)
                {
                    // Маска уже корректна

                    if (!string.IsNullOrWhiteSpace(Pattern.DocTitle))
                    {
                        doc.Title = TryParse(fileName, Pattern.DocTitle);
                    }
                    if (!string.IsNullOrWhiteSpace(Pattern.DocNumber))
                    {
                        doc.Number = TryParse(fileName, Pattern.DocNumber);
                    }
                    if (!string.IsNullOrWhiteSpace(Pattern.DocDate))
                    {
                        var date = TryParse(fileName, Pattern.DocDate);
                        if (date.Length == 6)
                        {
                            doc.Date = DateTime.ParseExact(date, "ddMMyy", CultureInfo.InvariantCulture);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(Pattern.DocNotes))
                    {
                        doc.Description = TryParse(fileName, Pattern.DocNotes);
                    }

                    if (!string.IsNullOrWhiteSpace(Pattern.CodeMo))
                    {
                        var codeMo = TryParse(fileName, Pattern.CodeMo);
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

                            var res = await DocManager.GetFoundContacts(request);
                            await _jsRuntime.InvokeVoidAsync("azino.Console", res, "Chiefs of MO");

                            if (res.Succeeded && res.Data.Count > 0)
                            {
                                var contacts = res.Data;
                                acts.ForEach(a =>
                                {
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

                            var res = await DocManager.GetFoundContacts(request);
                            await _jsRuntime.InvokeVoidAsync("azino.Console", res, "Chiefs of SMO");

                            if (res.Succeeded && res.Data.Count > 0)
                            {
                                var contacts = res.Data;
                                acts.ForEach(a =>
                                {
                                    if (a.Step.OrgType == OrgTypes.SMO && a.Step.MemberGroup == MemberGroups.OnlyHead)
                                    {
                                        AddMainContacts(a, contacts);
                                    }
                                });
                            }
                        }
                    }
                }

                doc.URL = string.Empty;
                doc.UploadRequest.Data = Array.Empty<byte>();
                doc.UploadRequest.FileName = fileName;
                doc.UploadRequest.Extension = Path.GetExtension(fileName);

                doc.CurrentStep = 1;
                doc.ExecutorId = BlankExecutor?.Id ?? emplId;

                doc.Members = new() { initiator };
                acts.ForEach(a =>
                {
                    AddMainContact(a);
                    a.Members.ForEach(m => doc.Members.Add(NewDocActMember(a, m)));
                });

                await _jsRuntime.InvokeVoidAsync("azino.Console", doc, "Doc To Save");

                // Проверить на ошибки
                var errors = 0; // Проверить заголовок и т.п.?

                acts.ForEach(a =>
                {
                    if (a.Step.Requred && a.Contact is null && a.Members.Count == 0) { errors++; }
                });

                if (errors > 0)
                {
                    await _jsRuntime.InvokeVoidAsync("azino.Console", errors, "Errors");

                    importErrors++;
                    continue;
                }

                try
                {
                    // Если нет ошибок |> Отправить на сервер
                    var response = await DocManager.PostDocAsync(doc);

                    // Получить ответ с сервера, если ошибка - учесть
                    if (!response.Succeeded) { importErrors++; }
                }
                catch (Exception) { importErrors++; }
            }

            // + Вывести сообщение: успешно / ошибок
            importState = ImportStates.Сompleted;
            StateHasChanged();
        }

        private async Task<bool> LoadRouteAsync(int id)
        {
            var response = await DirManager.GetRouteCardAsync(id);

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

            var mask = (Route.ParseFileName && !string.IsNullOrWhiteSpace(Pattern.FileMask)) ? Pattern.FileMask : "*";
            await _jsRuntime.InvokeVoidAsync("azino.Console", mask, "File Mask");

            RouteDocTypes = Route.DocTypeIds.Select(id => _allDocTypes.Find(t => t.Id == id)).ToHashSet();

            return true;
        }
        private async Task<bool> NewBlankDocInitAsync()
        {
            BlankDoc = new()
            {
                EmplOrgId = emplOrgId,
                EmplId = emplId,

                TypeId = (Route.DocTypeIds.Count > 0) ? Route.DocTypeIds[0] : 1,
                Date = Route.DateIsToday ? DateTime.Today : null,

                RouteId = Route.Id,
                TotalSteps = Route.Stages.Count, // Steps - первоначальное название этапов

                Members = new(),
                UploadRequest = new UploadRequest()
                {
                    IsServerImport = true,
                    Data = null,
                    UploadType = UploadType.Document,
                    FileName = string.Empty,
                    Extension = string.Empty
                }
            };

            BlankExecutor = Route.Executor;

            BlankActs.Clear();
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

                BlankActs.Add(act);
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", BlankDoc, "NEW BlankDoc");
            await _jsRuntime.InvokeVoidAsync("azino.Console", BlankActs, "Route BlankActs");

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

            if (!act.Members.Exists(m => m.UserId == act.Contact.Id))
            {
                //await _jsRuntime.InvokeVoidAsync("azino.Console", m, "NEW Doc");
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
        private static string ContactName(ContactResponse c) =>
            $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";
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

        private static AddEditDocCommand CloneDoc(AddEditDocCommand blank)
        {
            var newDoc = new AddEditDocCommand()
            {
                ParentId = blank.ParentId,

                EmplId = blank.EmplId,
                EmplOrgId = blank.EmplOrgId,
                ExecutorId = blank.ExecutorId,

                TypeId = blank.TypeId,
                Number = blank.Number,
                Date = blank.Date,

                Title = blank.Title,
                Description = blank.Description,
                IsPublic = blank.IsPublic,

                RouteId = blank.RouteId,
                Stage = blank.Stage,
                CurrentStep = blank.CurrentStep,
                TotalSteps = blank.TotalSteps,

                URL = blank.URL,
                UploadRequest = new UploadRequest()
                {
                    IsServerImport = blank.UploadRequest.IsServerImport,
                    Data = blank.UploadRequest.Data,
                    UploadType = blank.UploadRequest.UploadType,
                    FileName = blank.UploadRequest.FileName,
                    Extension = blank.UploadRequest.Extension
                },

                Members = new()
            };

            blank.Members.ForEach(m => newDoc.Members.Add(m));

            return newDoc;
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
        private static DocActModel NewDocAct(DocActModel a)
        {
            var act = new DocActModel()
            {
                Step = a.Step,
                Contact = a.Contact,
                Members = new()
            };

            a.Members.ForEach(m => act.Members.Add(m));

            return act;
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
    }
}
