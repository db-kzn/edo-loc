using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Domain.Enums;
using System.Linq;

namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class DocumentsEndpoints
    {
        public const string Ctrl = "api/documents";
        public const string AddEdit = Ctrl;
        public const string AddEditDoc = $"{Ctrl}"; // /doc
        public const string Delete = Ctrl;

        public const string ChangeStage = $"{Ctrl}/stage";
        public const string AddMembers = $"{Ctrl}/members";
        public const string SearchDocsPaged = $"{Ctrl}/search";

        public const string PostAgreementAnswer = $"{Ctrl}/agreement-answer";
        public const string PostAgreementSigned = $"{Ctrl}/agreement-signed";
        public const string SearchAgrsPaged = $"{Ctrl}/agreements-search";

        public const string GetImportsCount = $"{Ctrl}/imports-count";
        public const string GetRouteTitles = $"{Ctrl}/route-titles";
        public static string GetImportFiles(int routeId) => $"{Ctrl}/import-files?routeId={routeId}";

        public static string GetById(int documentId) => $"{Ctrl}/{documentId}";
        public static string GetDocAgreements(int docId) => $"{Ctrl}/agreements?docId={docId}";
        public static string GetFoundOrgs(string search) => $"{Ctrl}/orgs-search?search={search}";
        public static string FindOrgsWithType(OrgTypes orgType, string search) => $"{Ctrl}/orgs-find?orgType={orgType}&search={search}";

        public static string GetAgreementsProgress(int docId, int? agrId) => $"{Ctrl}/progress?docId={docId}&agrId={agrId}";
        public static string GetDocAgreementStage(int docId, int? agrId) => $"{Ctrl}/doc-agreements-card?docId={docId}&agrId={agrId}";

        public static string GetEmployeeAgreements(AgreementStates state) => $"{Ctrl}/user-agreements?state={state}";
        public static string GetAgreementMembers(int orgId, string search) => $"{Ctrl}/members?orgId={orgId}&search={search}";

        public static string GetFoundContacts(SearchContactsRequest r) =>
            $"{Ctrl}/contacts?orgType={r.OrgType}&baseRole={r.BaseRole}&search={r.SearchString}&take={r.Take}&orgId={r.OrgId}";

        public static string GetDocsPaged(GetPagedDocumentsRequest r)
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

            return $"{Ctrl}?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&docStage={r.DocStage}&matchCase={r.MatchCase}&orderBy={sort}";
        }

        public static string GetAgrsPaged(GetPagedAgreementsRequest r)
        {
            var sort = (r.OrderBy?.Any() == true) ? string.Join(",", r.OrderBy) : ""; // Id Descending
            return $"{Ctrl}/agrs?pageNumber={r.PageNumber}&pageSize={r.PageSize}&searchString={r.SearchString}&agrState={r.AgrState}&matchCase={r.MatchCase}&orderBy={sort}";
        }

        public static string GetDocCard(int id) => $"{Ctrl}/card?id={id}";
    }
}