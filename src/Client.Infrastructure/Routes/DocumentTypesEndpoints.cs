namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class DocumentTypesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public const string Export = "api/documentTypes/export";

        public const string GetAll = "api/documentTypes";
        public const string Delete = "api/documentTypes";
        public const string Save = "api/documentTypes";
        public const string GetCount = "api/documentTypes/count";
    }
}