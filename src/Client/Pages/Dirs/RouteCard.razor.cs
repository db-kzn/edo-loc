using AutoMapper;
using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Client.Infrastructure.Mappings;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteCard
    {
        [Parameter]
        public int? Id { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }
        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IDocumentTypeManager DocTypeManager { get; set; }
        private List<DocTypeResponse> _docTypes = new();

        private readonly IMapper _mapper = new MapperConfiguration(c => { c.AddProfile<RouteProfile>(); }).CreateMapper();

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit;

        private int tz;
        private int delay;
        private int duration;
        private const bool openFilter = false;

        private MudTabs _tabs;
        private MudDropContainer<RouteStepModel> _dropContainer;
        private FileParseModel Pattern { get; set; } = new();

        private RouteCardModel Route { get; set; } = new();
        private IEnumerable<DocTypeResponse> SelectedDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        private IEnumerable<OrgTypes> SelectedOrgTypes { get; set; } = new HashSet<OrgTypes>() { };

        //private ContactResponse Executor { get; set; } = null;
        private readonly bool resetValueOnEmptyText = true;
        private readonly bool coerceText = true;
        private readonly bool coerceValue = false;
        private readonly bool clearable = true;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            userId = _authUser.GetUserId();

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadDocumentTypesAsync();

            if (Id is null || Id == 0)
            {
                Route.Stages.Add(new RouteStageModel() { Number = 1 });
            }
            else
            {
                var response = await DirManager.GetRouteCardAsync((int)Id);

                if (response.Succeeded)
                {
                    var card = response.Data;
                    await _jsRuntime.InvokeVoidAsync("azino.Console", card, "Route Card Response");
                    SetRouteCard(card);
                }
                else
                {
                    // Error
                }
            }
        }

        //private void DeleteStep() { }

        private async Task LoadDocumentTypesAsync()
        {
            var response = await DocTypeManager.GetAllAsync();
            if (response.Succeeded) { _docTypes = response.Data; }
        }
        private void SetRouteCard(RouteCardResponse card)
        {
            // RouteCardResponse => RouteCardModel

            SelectedDocTypes = card.DocTypeIds.Select(id => _docTypes.Find(t => t.Id == id)).ToHashSet();
            SelectedOrgTypes = card.ForOrgTypes.ToHashSet();

            //Route = _mapper.Map<RouteCardModel>(card);

            Route.DocTypeIds = card.DocTypeIds;
            Route.ForOrgTypes = card.ForOrgTypes;
            Route.Stages = card.Stages;
            Route.Steps = card.Steps;

            Route.Id = card.Id;
            Route.Number = card.Number;
            Route.Code = card.Code;

            Route.Short = card.Short;
            Route.Name = card.Name;
            Route.Description = card.Description;

            Route.ExecutorId = card.ExecutorId;
            Route.Executor = card.Executor;
            Route.ForUserRole = card.ForUserRole;
            Route.EndAction = card.EndAction;

            Route.IsActive = card.IsActive;
            Route.DateIsToday = card.DateIsToday;
            Route.NameOfFile = card.NameOfFile;
            Route.ParseFileName = card.ParseFileName;

            Route.AllowRevocation = card.AllowRevocation;
            Route.ProtectedMode = card.ProtectedMode;
            Route.ShowNotes = card.ShowNotes;
            Route.UseVersioning = card.UseVersioning;

            Route.IsPackage = card.IsPackage;
            Route.CalcHash = card.CalcHash;
            Route.AttachedSign = card.AttachedSign;
            Route.DisplayedSign = card.DisplayedSign;

            Route.HasDetails = card.HasDetails;

            card.Parses.ForEach(c => _ = c.PatternType switch
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
        }

        private string GetMultiDocTypesText(List<string> selectedDocTypes)
        {
            if (selectedDocTypes == null || selectedDocTypes.Count == 0)
            {
                return _localizer[""];
            }

            if (selectedDocTypes.Count == 5)
            {
                return _localizer["Any Docs"];
            }

            return string.Join(", ", selectedDocTypes.Select(x => x));
        }
        private string GetMultiOrgTypesText(List<string> selectedOrgTypes)
        {
            if (selectedOrgTypes == null || selectedOrgTypes.Count == 0)
            {
                return _localizer[""];
            }

            if (selectedOrgTypes.Count == 5)
            {
                return _localizer["Any Orgs"];
            }

            return string.Join(", ", selectedOrgTypes.Select(x => _localizer[x]));
        }

        private void Close() => _navigationManager.NavigateTo("/dirs/routes");
        private async Task SaveAsync()
        {
            // RouteCardModel => AddEditRouteCardCommand

            var command = new AddEditRouteCommand
            {
                DocTypeIds = SelectedDocTypes.Select(t => t.Id).ToList(),
                ForOrgTypes = SelectedOrgTypes.ToList(),
                Stages = Route.Stages.Select(s => NewRouteStage(s)).ToList(),
                Steps = Route.Steps.Select(s => NewRouteStep(s)).ToList(),

                Parses = SelectFileParses(),

                Id = Route.Id,
                Number = Route.Number,
                Code = Route.Code,

                Short = Route.Short,
                Name = Route.Name,
                Description = Route.Description,

                ExecutorId = Route.Executor?.Id ?? string.Empty,
                ForUserRole = Route.ForUserRole,
                EndAction = Route.EndAction,

                IsActive = Route.IsActive,
                DateIsToday = Route.DateIsToday,
                NameOfFile = Route.NameOfFile,
                ParseFileName = Route.ParseFileName,

                AllowRevocation = Route.AllowRevocation,
                ProtectedMode = Route.ProtectedMode,
                ShowNotes = Route.ShowNotes,
                UseVersioning = Route.UseVersioning,

                IsPackage = Route.IsPackage,
                CalcHash = Route.CalcHash,
                AttachedSign = Route.AttachedSign,
                DisplayedSign = Route.DisplayedSign,

                HasDetails = Route.HasDetails
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", command, "Save Route");

            var response = await DirManager.RoutePostAsync(command);

            if (response.Succeeded)
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Success));
                _navigationManager.NavigateTo("/dirs/routes");
            }
            else
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }
        }

        private async Task<IEnumerable<ContactResponse>> SearchContactsAsync(string search)
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
        private static string ContactName(ContactResponse c) =>
            $"[{(string.IsNullOrWhiteSpace(c.OrgShortName) ? c.InnLe : c.OrgShortName)}] {c.Surname} {c.GivenName}";

        private void AddNewStage() => Route.Stages.Add(new RouteStageModel() { Number = Route.Stages.Count + 1 });
        private void DeleteStage()
        {
            int count = Route.Stages.Count;
            if (count == 1) return;

            Route.Stages.RemoveAt(count - 1);

            var steps = Route.Steps.Where(s => s.StageNumber != count).ToList();
            Route.Steps.Clear();
            steps.ForEach(s => Route.Steps.Add(s));
        }

        private async Task AddEditStepAsync(RouteStepModel step)
        {
            var stepClone = new RouteStepModel(step);
            //await _jsRuntime.InvokeVoidAsync("azino.Console", step, "RouteStepModel");
            //await _jsRuntime.InvokeVoidAsync("azino.Console", stepClone, "RouteStepModel Clone");

            var parameters = new DialogParameters() { { nameof(RouteStepDialog.Step), stepClone } };
            var options = new DialogOptions() { DisableBackdropClick = false, CloseButton = true, CloseOnEscapeKey = true };
            var dialog = _dialogService.Show<RouteStepDialog>("", parameters, options);
            var result = await dialog.Result;

            if (result.Cancelled) { return; }
            
            var s = (RouteStepModel)result.Data;


            if (s is null) // Delete Step
            {
                Route.Steps.Remove(step);
            }
            else // Update Step
            {
                //step.Id = s.Id;
                //step.RouteId = s.RouteId;

                //step.StageNumber = s.StageNumber;
                //step.Number = s.Number;

                step.ActType = s.ActType;
                step.AutoSearch = s.AutoSearch;

                step.OrgType = s.OrgType;
                step.OrgId = s.OrgId;
                //step.OrgMember = s.OrgMember; // Used only for View

                step.Requred = s.Requred;
                step.MemberGroup = s.MemberGroup;

                step.SomeParticipants = s.SomeParticipants;
                step.AllRequred = s.AllRequred;

                step.HasAgreement = s.HasAgreement;
                step.HasReview = s.HasReview;

                step.Description = s.Description;

                step.Members = s.Members;
                //step.Members.Clear();
                //step.Members.AddRange(s.Members);
            }

            _dropContainer.Refresh();
            
        }
        private async Task AddStepAsync(int stageNumber)
        {
            var step = new RouteStepModel() { StageNumber = stageNumber };
            Route.Steps.Add(step);

            await AddEditStepAsync(step);
        }
        private async Task EditStepAsync(RouteStepModel step)
        {
            await AddEditStepAsync(step);
        }
        private static void StepUpdated(MudItemDropInfo<RouteStepModel> info)
        {
            if (int.TryParse(info.DropzoneIdentifier, out int stageNumber))
            {
                info.Item.StageNumber = stageNumber;
            }
        }

        private static RouteStageCommand NewRouteStage(RouteStageModel s)
        {
            return new RouteStageCommand()
            {
                Id = s.Id,
                RouteId = s.RouteId,
                Number = s.Number,

                Color = s.Color,
                Name = s.Name,
                Description = s.Description,

                ActType = s.ActType,
                InSeries = s.InSeries,
                AllRequred = s.AllRequred,

                DenyRevocation = s.DenyRevocation,
                Validity = s.Validity
            };
        }
        private static RouteStepCommand NewRouteStep(RouteStepModel s)
        {
            return new RouteStepCommand()
            {
                Id = s.Id,
                RouteId = s.RouteId,

                StageNumber = s.StageNumber,
                Number = s.Number,

                ActType = s.ActType,
                AutoSearch = s.AutoSearch,

                OrgType = s.OrgType,
                OrgId = s.OrgId,

                Requred = s.Requred,
                MemberGroup = s.MemberGroup,

                SomeParticipants = s.SomeParticipants,
                AllRequred = s.AllRequred,

                HasAgreement = s.HasAgreement,
                HasReview = s.HasReview,

                Description = s.Description,
                Members = s.Members.Select(m => NewRouteStepMember(s, m)).ToList(),
            };
        }
        private static RouteStepMemberCommand NewRouteStepMember(RouteStepModel s, RouteStepMemberModel m)
        {
            return new RouteStepMemberCommand()
            {
                RouteStepId = s.Id,
                Act = m.Act,
                IsAdditional = m.IsAdditional,
                UserId = m.Contact.Id
            };
        }

        private static string StepClass(bool required)
        {
            var border = required ? "border-solid" : "border-dotted";
            var css = $"{border} px-0 py-0 my-4 rounded-lg border-2 mud-border-lines-default";
            return css;
        }
        private async Task ParseCheckAsync()
        {
            var parameters = new DialogParameters() { { nameof(RouteParseDialog.Pattern), Pattern } };
            var options = new DialogOptions() { DisableBackdropClick = true };
            var dialog = _dialogService.Show<RouteParseDialog>("", parameters, options);

            _ = await dialog.Result;
        }
        private List<RouteFileParseCommand> SelectFileParses()
        {
            var parses = new List<RouteFileParseCommand>();

            if (!string.IsNullOrWhiteSpace(Pattern.FileName))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.Sample, Pattern.FileName, ValueTypes.String));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.FileMask))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.Mask, Pattern.FileMask, ValueTypes.String));

                var dotId = Pattern.FileMask.LastIndexOf('.');
                if (dotId != -1)
                {
                    var accept = Pattern.FileMask[dotId..];
                    parses.Add(new RouteFileParseCommand(ParsePatterns.Accept, accept, ValueTypes.String));
                }
            }

            if (!string.IsNullOrWhiteSpace(Pattern.DocTitle))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.DocTitle, Pattern.DocTitle, ValueTypes.String));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.DocNumber))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.DocNumber, Pattern.DocNumber, ValueTypes.String));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.DocDate))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.DocDate, Pattern.DocDate, ValueTypes.Date));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.DocNotes))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.DocNotes, Pattern.DocNotes, ValueTypes.String));
            }

            if (!string.IsNullOrWhiteSpace(Pattern.CodeMo))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.CodeMO, Pattern.CodeMo, ValueTypes.String));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.CodeSmo))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.CodeSMO, Pattern.CodeSmo, ValueTypes.String));
            }
            if (!string.IsNullOrWhiteSpace(Pattern.CodeFund))
            {
                parses.Add(new RouteFileParseCommand(ParsePatterns.CodeFund, Pattern.CodeFund, ValueTypes.String));
            }

            return parses;
        }
    }
}
