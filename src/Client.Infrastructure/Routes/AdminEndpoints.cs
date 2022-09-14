using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Requests.Orgs;
using System.Linq;

namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class AdminEndpoints
    {
        public const string Ctrl = "api/admin/";

        //public const string GetAllOrgs = $"{Ctrl}orgs";
        public const string SearchOrgs = $"{Ctrl}orgs/search";
        public static string GetPagedOrgs(GetPagedOrgsRequest r)
        {
            var sort = (r.OrderBy?.Any() == true) ? string.Join(",", r.OrderBy) : ""; // Id Descending
            return $"{Ctrl}orgs?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&matchCase={r.MatchCase}&orderBy={sort}";
        }

        //public const string GetAllUsers = $"{Ctrl}users";
        public const string SearchUsers = $"{Ctrl}users/search";
        public static string GetPagedUsers(GetPagedUsersRequest r)
        {
            var sort = (r.OrderBy?.Any() == true) ? string.Join(",", r.OrderBy) : ""; // Id Descending
            return $"{Ctrl}users?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&matchCase={r.MatchCase}&orderBy={sort}";
        }

        //public const string GetAllCerts = $"{Ctrl}certs";
        public const string SearchCerts = $"{Ctrl}certs/search";
        public static string GetPagedCerts(GetPagedCertsRequest r)
        {
            var sort = (r.OrderBy?.Any() == true) ? string.Join(",", r.OrderBy) : ""; // Id Descending
            return $"{Ctrl}certs?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&matchCase={r.MatchCase}&orderBy={sort}";
        }

        public static string GetUserCerts(string userId) => $"{Ctrl}user-certs/{userId}";

        public const string AddEditOrg = $"{Ctrl}org";
        //public const string AddEditUser = $"{Ctrl}user";
        public const string AddUser = $"{Ctrl}user/add";
        public const string EditUser = $"{Ctrl}user/edit";

        public const string AddEditCert = $"{Ctrl}cert";
        public const string DeleteCert = $"{Ctrl}cert";

        public const string UpdateUsersOrgType = $"{Ctrl}users/org-type";

        public const string GetMailParams = $"{Ctrl}mail-params";
        public const string SaveMailParams = $"{Ctrl}mail-params";
        public const string SendMail = $"{Ctrl}send-mail";
    }
}
