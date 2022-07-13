namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public const string Register = "api/identity/account/register";
        public const string ChangePassword = "api/identity/account/changepassword";
        public const string UpdateProfile = "api/identity/account/updateprofile";

        public static string GetProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }

        public static string UpdateProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }
    }
}