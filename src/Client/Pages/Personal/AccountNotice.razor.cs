using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Domain.Entities.System;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Personal
{
    public partial class AccountNotice
    {
        //[Inject] private IStateManager StateManager { get; set; }
        public string UserId { get; set; }
        private Subscribe Subscribe { get; set; } = new();

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            UserId = user.GetUserId();

            var result = await _stateManager.GetSubscribeAsync(UserId);
            if (result.Succeeded) {
                var subscribe = result.Data;

                await _jsRuntime.InvokeVoidAsync("azino.Console", subscribe, "Subscribe GET: ");

                //Subscribe = result;

                Subscribe.Email.AgreementIncoming = subscribe.Email.AgreementIncoming;
                Subscribe.Email.DocumentRejected = subscribe.Email.DocumentRejected;
                Subscribe.Email.DocumentApproved = subscribe.Email.DocumentApproved;
                Subscribe.Email.DocumentAgreed = subscribe.Email.DocumentAgreed;
            }
        }

        async void SaveChanges()
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", Subscribe, "Subscribe POST: ");

            _ = await _stateManager.PostSubscribeAsync(Subscribe);
            

            //"Notification settings saved", Severity.Success)
            //_snackBar.Add(message, severity, config => config.ShowCloseIcon = false);
        }
    }
}
