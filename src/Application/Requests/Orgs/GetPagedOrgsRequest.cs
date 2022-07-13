namespace EDO_FOMS.Application.Requests.Orgs
{
    public class GetPagedOrgsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public bool MatchCase { get; set; }

        public GetPagedOrgsRequest() { }
        public GetPagedOrgsRequest(
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
