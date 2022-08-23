using AutoMapper;
using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Client.Infrastructure.Mappings;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteCard
    {
        [Parameter]
        public int? Id { get; set; }
        [Inject] private IDirectoryManager DirManager { get; set; }
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

        private RouteCardModel Route { get; set; } = new();
        private IEnumerable<DocTypeResponse> SelectedDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        private IEnumerable<OrgTypes> SelectedOrgTypes { get; set; } = new HashSet<OrgTypes>() { };

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
            Route.Name = card.Name;
            Route.Description = card.Description;

            Route.ForUserRole = card.ForUserRole;
            Route.EndAction = card.EndAction;

            Route.IsPackage = card.IsPackage;
            Route.CalcHash = card.CalcHash;
            Route.AttachedSign = card.AttachedSign;
            Route.DisplayedSign = card.DisplayedSign;

            Route.IsActive = card.IsActive;
            Route.AllowRevocation = card.AllowRevocation;
            Route.UseVersioning = card.UseVersioning;
            Route.HasDetails = card.HasDetails;
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

                Id = Route.Id,
                Number = Route.Number,
                Name = Route.Name,
                Description = Route.Description,

                ForUserRole = Route.ForUserRole,
                EndAction = Route.EndAction,

                IsPackage = Route.IsPackage,
                CalcHash = Route.CalcHash,
                AttachedSign = Route.AttachedSign,
                DisplayedSign = Route.DisplayedSign,

                IsActive = Route.IsActive,
                AllowRevocation = Route.AllowRevocation,
                UseVersioning = Route.UseVersioning,
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
            var parameters = new DialogParameters() { { nameof(RouteStepDialog.Step), step } };

            var dialog = _dialogService.Show<RouteStepDialog>("", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                var s = (RouteStepModel)result.Data;
                if (s is null) { Route.Steps.Remove(step); }
                _dropContainer.Refresh();
            }
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
                OrgType = s.OrgType,
                AutoSearch = s.AutoSearch,
                Members = s.Members.Select(m => NewRouteStepMember(s, m)).ToList(),

                OnlyHead = s.OnlyHead,
                Requred = s.Requred,
                SomeParticipants = s.SomeParticipants,

                AllRequred = s.AllRequred,
                HasAgreement = s.HasAgreement,
                HasReview = s.HasReview
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
    }
}
