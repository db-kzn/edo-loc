using Blazored.FluentValidation;
using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Personal
{
public partial class EmployeeDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public AddEditEmployeeRequest EmployeeModel { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private ClaimsPrincipal _authUser;
        private bool _canSelfOrgEdit = false;
        private bool _canManageUsers = false;
        private bool _canManageEmployees = false;
        private bool _canManageManagers = false;
        private bool _canManageChiefs = false;
        private bool _canManageAdmins = false;

        protected override async Task OnParametersSetAsync()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();

            _canSelfOrgEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.SelfOrg.Edit)).Succeeded;

            _canManageUsers = (await _authService.AuthorizeAsync(_authUser, Permissions.Management.Users)).Succeeded;
            _canManageEmployees = (await _authService.AuthorizeAsync(_authUser, Permissions.Management.Employees)).Succeeded;
            _canManageManagers = (await _authService.AuthorizeAsync(_authUser, Permissions.Management.Managers)).Succeeded;
            _canManageChiefs = (await _authService.AuthorizeAsync(_authUser, Permissions.Management.Chiefs)).Succeeded;
            _canManageAdmins = (await _authService.AuthorizeAsync(_authUser, Permissions.Management.Admins)).Succeeded;
        }

        private async Task OnSubmit()
        {
            var response = await _userManager.AddEditEmployeeAsync(EmployeeModel);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
