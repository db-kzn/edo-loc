namespace EDO_FOMS.Application.Requests.Directories
{
    public class GetPagedCompaniesRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public bool MatchCase { get; set; }

        public GetPagedCompaniesRequest() { }
        public GetPagedCompaniesRequest(
            int pageSize,
            int pageNumber,
            string searchString,

            string orderings,
            bool matchCase = false
            )
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            SearchString = searchString;

            OrderBy = string.IsNullOrEmpty(orderings) ? System.Array.Empty<string>() : orderings.Split(',');
            MatchCase = matchCase;
        }
    }
}
