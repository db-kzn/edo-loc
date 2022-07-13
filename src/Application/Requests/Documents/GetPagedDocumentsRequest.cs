using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests.Documents
{
    public class GetPagedDocumentsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public DocStages DocStage { get; set; }
        public bool MatchCase { get; set; }

        public GetPagedDocumentsRequest() { }
        public GetPagedDocumentsRequest(
            int pageSize,
            int pageNumber,
            string searchString,
            
            string orderings,
            DocStages docStage = DocStages.Undefined,
            bool matchCase = false
            )
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            SearchString = searchString;

            OrderBy = string.IsNullOrEmpty(orderings) ? System.Array.Empty<string>() : orderings.Split(',');
            DocStage = docStage;
            MatchCase = matchCase;
        }
    }
}