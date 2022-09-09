namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public class OrgsEndpoints
    {
        //public static string ExportFiltered(string searchString)
        //{
        //    return $"{Export}?searchString={searchString}";
        //}

        //public static string Export = "api/v1/orgs/export";

        //public static string GetAll = "api/v1/orgs";
        public const string Save = "api/v1/orgs";
        public const string Delete = "api/v1/orgs";
        public const string GetCount = "api/v1/orgs/count";

        public static string GetById(int id) { return $"api/v1/orgs/{id}"; }
        public static string GetIdByCode(string code) { return $"api/v1/orgs/id?code={code}"; }

        public static string GetByInn(string inn)
        {
            return $"api/identity/user/org/{inn}";
        }
    }
}
