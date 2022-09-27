using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Main
{
    public partial class Home
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        private HomeConfiguration home = new();

        private ClaimsPrincipal _authUser;
        private bool _canSystemEdit;

        private int delay;
        private int duration;

        protected override async void OnInitialized()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await LoadParamsAsync();
        }

        private async Task LoadParamsAsync()
        {
            var response = await AdmManager.GetHomeParamsAsync();

            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Home Params");

            if (response.Succeeded)
            {
                home = response.Data;
                StateHasChanged();
            }
        }

        private async Task PageParams()
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var parameters = new DialogParameters()
            {
                {
                    nameof(PageParamsDialog._home),
                    new HomeConfiguration()
                    {
                        Title = home.Title,
                        Description = home.Description,

                        DocSupportPhone = home.DocSupportPhone,
                        DocSupportEmail = home.DocSupportEmail,

                        TechSupportPhone = home.DocSupportPhone,
                        TechSupportEmail = home.TechSupportEmail
                    }
                }
            };

            var dialog = _dialogService.Show<PageParamsDialog>("", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled) { await LoadParamsAsync(); }
        }
        private void ShowChangeLogs() => _ = _dialogService.Show<ChangeLogsDialog>();
    }
}
