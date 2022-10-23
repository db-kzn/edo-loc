using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Settings;
using EDO_FOMS.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EDO_FOMS.Client.Infrastructure.Managers.Identity.Roles;
using Microsoft.AspNetCore.Components;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Shared.Constants.Storage;
using EDO_FOMS.Client.Shared.Dialogs;

namespace EDO_FOMS.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }
        //[Inject] private StateService State { get; set; }
        public NavCounts NavCounts = new() { Progress = 5 };

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        private MudThemeProvider _mudThemeProvider;
        private MudTheme _theme;
        private bool _isDarkMode = false;

        private bool _drawerOpen = true;
        //private bool _rightToLeft = false;

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string Email { get; set; }
        //private string Surname { get; set; }
        //private string GivenName { get; set; }
        //private char FirstLetterOfName { get; set; }

        private int delay;
        private int duration;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _theme = EdoFomsTheme.DefaultTheme;
            _theme = await _clientPreferenceManager.GetCurrentThemeAsync();
            //_rightToLeft = await _clientPreferenceManager.IsRTL();
            _interceptor.RegisterEvent();

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            hubConnection = hubConnection.TryInitialize(_navigationManager);
            await hubConnection.StartAsync();

            hubConnection.On<string, string, string>(AppConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId == receiverUserId)
                {
                    _jsRuntime.InvokeAsync<string>("azino.PlayAudio", "notification");
                    _snackBar.Add(message, Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 10000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.Action = localizer["Chat?"];
                        config.ActionColor = Color.Primary;
                        config.Onclick = _ =>
                        {
                            _navigationManager.NavigateTo($"chat/{senderUserId}");
                            return Task.CompletedTask;
                        };
                    });
                }
            });
            hubConnection.On(AppConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = await _authManager.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(localizer["Refreshed Token."], Severity.Success);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(localizer["You are Logged Out."], Severity.Error);
                    await _authManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            });
            hubConnection.On<string, string>(AppConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResponse = await RoleManager.GetRolesAsync();
                    if (rolesResponse.Succeeded)
                    {
                        var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                            if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                _snackBar.Add(localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
                                await hubConnection.SendAsync(AppConstants.SignalR.OnDisconnect, CurrentUserId);
                                await _authManager.Logout();
                                _navigationManager.NavigateTo("/login");
                            }
                        }
                    }
                }
            });
        }

        private async Task LoadDataAsync()
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                
                Email = user.GetEmail();
                var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
                if (imageResponse.Succeeded)
                {
                    ImageDataUrl = imageResponse.Data;
                }

                var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                {
                    _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted."], Severity.Error);
                    await _authManager.Logout();
                }

                await hubConnection.SendAsync(AppConstants.SignalR.OnConnect, CurrentUserId);
            }
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

             _dialogService.Show<Logout>(localizer["Logout"], parameters, options);
        }

        private void DrawerToggle() => _drawerOpen = !_drawerOpen;
        private void DarkMode() => _isDarkMode = !_isDarkMode;
        //bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
        //_theme = isDarkMode ? AzinoTheme.DarkTheme : AzinoTheme.DefaultTheme;

        private async Task OnSignDoc()
        {
            var options = new DialogOptions { CloseButton = true };
            var dialog = _dialogService.Show<DocToSignDialog>("", options);
            _ = await dialog.Result;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }
    }
}