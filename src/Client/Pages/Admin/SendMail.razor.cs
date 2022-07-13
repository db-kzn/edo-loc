using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Application.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class SendMail
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        private readonly MailModel _mailModel = new();

        private int delay;
        private int duration;

        protected override void OnInitialized()
        {
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;
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
    }
}
