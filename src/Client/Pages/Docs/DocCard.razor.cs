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
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

        RouteCardResponse Route { get; set; } = new();
        private IEnumerable<DocTypeResponse> RouteDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        private IEnumerable<ContactResponse> ChiefsOfFund { get; set; } = new List<ContactResponse>();
        private IEnumerable<ContactResponse> ChiefsOfSMO { get; set; } = new List<ContactResponse>();

        private List<DocTypeResponse> _allDocTypes = new();
        public AddEditDocCommand Doc { get; set; } = new();
        private List<DocActModel> Acts { get; set; } = new();

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit;

        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = true;

        private IBrowserFile _file;
        private bool onUpload = false;

        private int tz;
        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;
            userId = _authUser.GetUserId();

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadDataAsync();

            if (RouteId is not null || RouteId != 0)
            {
                var response = await DirManager.GetRouteCardAsync((int)RouteId);
                Route = response.Data;
                await _jsRuntime.InvokeVoidAsync("azino.Console", Route, "Route");

                RouteDocTypes = Route.DocTypeIds.Select(id => _allDocTypes.Find(t => t.Id == id)).ToHashSet();
            }
            else
            {
                // Error
            }

            if (DocId is not null && DocId != 0)
            {
                // await LoadDocAsync(DocId);
            }
            else // DocId is null || DocId == 0
            {
                NewDoc();
            }

            StateHasChanged();
        }

        private void NewDoc()
        {
            Doc.TypeId = (Route.DocTypeIds.Count > 0) ? Route.DocTypeIds[0] : 1;

            Route.Steps.ForEach(s =>
            {
                var act = new DocActModel() { Step = s, Contact = null };

                if (s.OnlyHead)
                {
                    if (s.OrgType == OrgTypes.Fund) { AddContacts(act, ChiefsOfFund); }
                    else if (s.OrgType == OrgTypes.SMO) { AddContacts(act, ChiefsOfSMO); }
                }

                Acts.Add(act);
            });
        }

        private List<DocActModel> ActsByStage(int stageNumber) => Acts.Where(a => a.Step.StageNumber == stageNumber).ToList();

        private static void AddContacts(DocActModel act, IEnumerable<ContactResponse> contacts)
        {
            foreach (var c in contacts)
            {
                act.Contact = CloneContact(c);
                AddContact(act);
            }
        }
        private static void AddContact(DocActModel act)
        {
            var name = ContactName(act.Contact);

            if (!act.Contacts.ContainsKey(name))
            {
                act.Contacts.Add(name, CloneContact(act.Contact));
            }

            act.Contact = null;
        }
        private static void DelContact(DocActModel act, MudChip chip) => act.Contacts.Remove(chip.Text);

        private static ContactResponse CloneContact(ContactResponse c) => new()
        {
            Id = c.Id,
            OrgId = c.OrgId,
            InnLe = c.InnLe,

            Surname = c.Surname,
            GivenName = c.GivenName
        };
        private static string ContactName(ContactResponse c) =>
            $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";

        private static string IconByOrgType(OrgTypes ot) => ot switch
        {
            OrgTypes.Fund => Icons.Material.Outlined.HealthAndSafety,
            OrgTypes.MO => Icons.Material.Outlined.MedicalServices,
            OrgTypes.SMO => Icons.Material.Outlined.Museum,
            OrgTypes.MEO => Icons.Material.Outlined.MilitaryTech,
            OrgTypes.Treasury => Icons.Material.Outlined.AccountBalance,
            _ => Icons.Material.Outlined.Business
        };
        private string LabelByStep(RouteStageStepModel step) =>
            $"{_localizer[(step.OnlyHead) ? "Chief" : "Employee"]} " +
            $"{_localizer["of the"]} {_localizer[step.OrgType.ToString()]}";

        private async Task LoadDataAsync()
        {
            var data = await DocTypeManager.GetAllAsync();
            if (data.Succeeded) { _allDocTypes = data.Data; }

            ChiefsOfFund = await LoadChiefsAsync(OrgTypes.Fund);
            ChiefsOfSMO = await LoadChiefsAsync(OrgTypes.SMO);
        }
        private async Task<IEnumerable<ContactResponse>> LoadChiefsAsync(OrgTypes orgType)
        {
            var request = new SearchContactsRequest()
            {
                BaseRole = UserBaseRoles.Chief,
                OrgType = orgType,
                SearchString = ""
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }
        private async Task<IEnumerable<ContactResponse>> SearchContactsAsync(DocActModel act, string search)
        {
            var request = new SearchContactsRequest()
            {
                BaseRole = act.Step.OnlyHead ? UserBaseRoles.Chief : UserBaseRoles.Undefined,
                OrgType = act.Step.OrgType,
                SearchString = search
            };

            var response = await DocManager.GetFoundContacts(request);
            return (response.Succeeded) ? response.Data : new();
        }

        private void Close() => _navigationManager.NavigateTo($"/docs");
        private Task SendAsync()
        {
            return null;
        }
        private Task SaveAsync()
        {
            return null;
        }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;

            if (_file != null)
            {
                var buffer = new byte[_file.Size];
                var extension = Path.GetExtension(_file.Name);
                var fileName = Path.GetFileName(_file.Name);

                Doc.Title = Path.GetFileNameWithoutExtension(_file.Name);
                //if (string.IsNullOrWhiteSpace(_doc.Title)) {}

                const string format = "application/octet-stream";

                await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

                Doc.URL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";

                Doc.UploadRequest = new UploadRequest
                {
                    Data = buffer,
                    UploadType = UploadType.Document,
                    FileName = fileName,
                    Extension = extension
                };

                //await _jsRuntime.InvokeVoidAsync("azino.Console", _doc);
            }
        }
    }
}
