namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class UserEndpoints
    {
        const string ctrl = "api/identity/user/";

        public const string GetAll = ctrl;
        public const string Register = ctrl;
        public const string RegisterByCert = $"{ctrl}cert"; // Регистрация с сертификатом
        public const string Export = $"{ctrl}export";

        public const string NewUser = $"{ctrl}new";
        public const string EditUser = $"{ctrl}edit";
        public const string AddEditEmployee = $"{ctrl}employee";

        public const string ToggleUserStatus = $"{ctrl}toggle-status";
        public const string ForgotPassword = $"{ctrl}forgot-password";
        public const string ResetPassword = $"{ctrl}reset-password";

        public static string GetUserOrgExists(string inn) => $"{ctrl}org-exists/{inn}";
        public static string GetAllByOrgId(int orgId) => $"{ctrl}orgId:{orgId}";
        public static string Get(string userId) => ctrl + userId;
        public static string GetUserRoles(string userId) => $"{ctrl}roles/{userId}";
        public static string ExportFiltered(string searchString) => $"{Export}?searchString={searchString}";
    }
}