using EDO_FOMS.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared.Components
{
    public partial class UserAvatar
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        private string GivenName { get; set; }
        private string Surname { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }

        [Parameter]
        public string ImageDataUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            this.Email = user.GetEmail().Replace(".com", string.Empty);
            this.GivenName = user.GetGivenName();
            this.Surname = user.GetSurname();
            if (this.GivenName.Length > 0)
            {
                FirstLetterOfName = GivenName[0];
            }
            var UserId = user.GetUserId();
            var imageResponse = await _accountManager.GetProfilePictureAsync(UserId);
            if (imageResponse.Succeeded)
            {
                ImageDataUrl = imageResponse.Data;
            }
        }
    }
}
