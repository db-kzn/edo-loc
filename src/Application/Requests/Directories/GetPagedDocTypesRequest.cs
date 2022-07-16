namespace EDO_FOMS.Application.Requests.Directories
{
    public class GetPagedDocTypesRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public bool MatchCase { get; set; }

        public GetPagedDocTypesRequest() { }
        public GetPagedDocTypesRequest(
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
