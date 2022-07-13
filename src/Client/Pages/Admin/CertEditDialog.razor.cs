using Blazored.FluentValidation;
using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class CertEditDialog
    {
        [Parameter] public AddEditCertCommand AddEditCertModel { get; set; } = new();

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        //[CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        [Inject] private IAdminManager AdmManager { get; set; }
        //private ClaimsPrincipal _authUser;
        //private bool _canSystemEdit = false;

        private async Task SaveAsync()
        {
            var response = await AdmManager.AddEditCertAsync(AddEditCertModel);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            //await HubConnection.SendAsync(AppConstants.SignalR.SendUpdateDashboard);
        }

        private async Task Delete()
        {
            var response = await AdmManager.DeleteCertAsync(AddEditCertModel.Id);

            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("azino.Console", response.Data, "Cert Delete: ");
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    //await _jsRuntime.InvokeVoidAsync("azino.Console", message);
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private void Cancel() => MudDialog.Cancel();
    }
}
