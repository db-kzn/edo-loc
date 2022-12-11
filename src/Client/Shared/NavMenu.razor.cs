using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared
{
    public partial class NavMenu
    {
        //string AlertAssignmentIcon { get; set; } = "<path d=\"M19,3A2,2 0 0,1 21,5V19A2,2 0 0,1 19,21H5A2,2 0 0,1 3,19V5A2,2 0 0,1 5,3H9.18C9.6,1.84 10.7,1 12,1C13.3,1 14.4,1.84 14.82,3H19M12,3A1,1 0 0,0 11,4A1,1 0 0,0 12,5A1,1 0 0,0 13,4A1,1 0 0,0 12,3M7,7V5H5V19H19V5H17V7H7M11,9H13V13.5H11V9M11,15H13V17H11V15Z\" />";
        //[Inject] private IStateManager StateManager { get; set; }
        [Parameter] public bool IsOpen { get; set; } = true;
        [Parameter] public NavCounts NavCounts { get; set; }

        private ClaimsPrincipal _authUser;

        // Agreements - for All

        // Documents
        private bool _canDocumentsView;
        //private bool _canDocumentsEdit;

        //// Acts
        //private bool _canActsView;
        //private bool _canActsEdit;

        // Administration
        private bool _canSystemEdit;
        private bool _canSystemView;

        //private readonly NavCountsModel _counts = new();

        protected override void OnInitialized()
        {
            this.NavCounts.CountChanged = () => this.StateHasChanged();
        }

        protected override async Task OnParametersSetAsync()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();

            _canDocumentsView = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.View)).Succeeded;
            //_canDocumentsEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.Edit)).Succeeded;

            //_canActsView = (await _authService.AuthorizeAsync(_authUser, Permissions.Acts.View)).Succeeded;
            //_canActsEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.Acts.Edit)).Succeeded;

            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;
            _canSystemView = (await _authService.AuthorizeAsync(_authUser, Permissions.System.View)).Succeeded;

            //_canOrgsView = (await _authService.AuthorizeAsync(_authUser, Permissions.Organizations.View)).Succeeded;
            //_canUsersView = (await _authService.AuthorizeAsync(_authUser, Permissions.Users.View)).Succeeded;
            //_canCertsView = (await _authService.AuthorizeAsync(_authUser, Permissions.Certificates.View)).Succeeded;

            if (_canDocumentsView)
            {
                await GetNavCounts(_authUser.GetUserId());
                //_navCount = _stateService.GetNavCount();
            }
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    await GetNavCounts(_authUser.GetUserId());
        //}

        private async Task GetNavCounts(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var result = await _stateManager.RefreshNavCountsAsync(_authUser.GetUserId());

                if (result.Succeeded)
                {
                    var counts = result.Data;

                    NavCounts.Progress = counts.Progress;
                    NavCounts.Docs = counts.Docs;
                    NavCounts.Archive = counts.Archive;

                    await _jsRuntime.InvokeVoidAsync("azino.Console", counts, "Counts: ");
                }
            }
        }
    }
}
