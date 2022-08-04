using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteCard
    {
        [Inject] private IDocumentTypeManager DocTypeManager { get; set; }
        private List<DocTypeResponse> _docTypes = new();

        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit;

        private int tz;
        private int delay;
        private int duration;

        MudTabs _tabs;
        public AddEditRouteCommand _route { get; set; } = new();

        private IEnumerable<DocTypeResponse> SelectedDocTypes { get; set; } = new HashSet<DocTypeResponse>() { };
        private IEnumerable<OrgTypes> SelectedOrgTypes { get; set; } = new HashSet<OrgTypes>() {};

        private MudDropContainer<RouteStageStep> _dropContainer;
        private readonly List<RouteStage> _stages = new() { new RouteStage() { Number = 1 } };
        private List<RouteStageStep> _steps = new() { };

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
        }

        private async Task LoadDocumentTypesAsync()
        {
            var response = await DocTypeManager.GetAllAsync();
            if (response.Succeeded) { _docTypes = response.Data; }
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

        private void AddNewStage() => _stages.Add(new RouteStage() { Number = _stages.Count + 1 });
        private void DeleteStage()
        {
            int count = _stages.Count;
            if (count == 1) return;
            _stages.RemoveAt(count - 1);
            _steps = _steps.Where(s => s.StageNumber != count).ToList();
        }

        private async Task AddStepAsync(int stageNumber)
        {
            var step = new RouteStageStep() { StageNumber = stageNumber };
            _steps.Add(step);

            await AddEditStepAsync(step);
        }
        private async Task EditStepAsync(RouteStageStep step)
        {
            await AddEditStepAsync(step);
        }

        private async Task AddEditStepAsync(RouteStageStep step)
        {
            var parameters = new DialogParameters() { { nameof(RouteStepDialog.Step), step } };

            var dialog = _dialogService.Show<RouteStepDialog>("", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                var s = (RouteStageStep)result.Data;
                if (s is null) { _steps.Remove(step); }
                _dropContainer.Refresh();
            }
        }

        private static void StepUpdated(MudItemDropInfo<RouteStageStep> info)
        {
            if (int.TryParse(info.DropzoneIdentifier, out int stageNumber))
            {
                info.Item.StageNumber = stageNumber;
            }
        }

        private string StepClass(bool required)
        {
            var border = required ? "border-solid" : "border-dotted";
            var css = $"{border} px-0 py-0 my-4 rounded-lg border-2 mud-border-lines-default";
            return css;
        }
    }
}
