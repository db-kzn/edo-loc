namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class EmployeeEndpoints
    {
        public static string GetAllByOrgId(int orgId)
        {
            return $"api/identity/user/empls/{orgId}";
        }
    }
}
