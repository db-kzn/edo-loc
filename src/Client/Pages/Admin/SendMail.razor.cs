using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Application.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using EDO_FOMS.Application.Configurations;
using Microsoft.JSInterop;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class SendMail
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        private MudTabs _tabs;

        private Color iconColor = Color.Default;
        private InputType passwordInput = InputType.Password;
        private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private MailConfiguration mail = new();
        private readonly MailModel _mailModel = new();

        private int delay;
        private int duration;

        protected override async void OnInitialized()
        {
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            var response = await AdmManager.GetMailParamsAsync();

            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Mail Params");

            if (response.Succeeded)
            {
                mail = response.Data;
                StateHasChanged();
            }
        }

        private async Task ToSend()
        {
            var response = await AdmManager.PostMailAsync(_mailModel);

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
        }

        private async Task SaveAsync()
        {
            var response = await AdmManager.SaveMailParamsAsync(mail);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }
        }

        private void ShowPassword()
        {
            if (passwordInput == InputType.Password)
            {
                iconColor = Color.Error;
                passwordInput = InputType.Text;
                passwordInputIcon = Icons.Material.Filled.Visibility;
            }
            else
            {
                iconColor = Color.Default;
                passwordInput = InputType.Password;
                passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            }
        }
    }
}
