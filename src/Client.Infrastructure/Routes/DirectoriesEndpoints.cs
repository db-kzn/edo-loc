using EDO_FOMS.Application.Requests.Directories;
using System.Linq;

namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class DirectoriesEndpoints
    {
        public const string Ctrl = "api/directories";

        public const string CheckCompaniesForImports = $"{Ctrl}/check-companies-for-imports";

        public const string ImportFoms = $"{Ctrl}/import-foms";
        public const string ImportSmo = $"{Ctrl}/import-smo";
        public const string ImportMo = $"{Ctrl}/import-mo";

        public const string SearchCompaniesPaged = $"{Ctrl}/search-companies";

        public static string GetCompaniesPaged(GetPagedCompaniesRequest r)
        {
            var sort = (r.OrderBy?.Any() == true) ? string.Join(",", r.OrderBy) : ""; // Id Descending

            //if (r.OrderBy?.Any() == true)
            //{
            //    foreach (var orderBy in r.OrderBy)
            //    {
            //        sort += $"{orderBy},";
            //    }
            //    sort = sort[..^1]; // loose training ,
            //}

            return $"{Ctrl}/companies?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&matchCase={r.MatchCase}&orderBy={sort}";
        }
    }
}
