using Blazored.FluentValidation;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class OrgEditDialog
    {
        [Parameter] public AddEditOrgCommand AddEditOrgModel { get; set; } = new();
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        [Inject] private IAdminManager AdmManager { get; set; }
        private ClaimsPrincipal _authUser;
        private bool _canSystemEdit = false;

        private OrgTypes _orgType;

        protected override async Task OnInitializedAsync()
        {
            _orgType = AddEditOrgModel.Type;
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;
        }

        private async Task SaveAsync()
        {
            var response = await AdmManager.AddEditOrgAsync(AddEditOrgModel);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);

                if (_orgType != AddEditOrgModel.Type)
                {
                    await AdmManager.UpdateUsersOrgTypeAsync(new()
                    {
                        OrgId = AddEditOrgModel.Id,
                        InnLe = AddEditOrgModel.Inn,
                        OrgType = AddEditOrgModel.Type
                    });
                }

                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        public void Cancel() => MudDialog.Cancel();
    }
}
