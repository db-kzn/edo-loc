using System;

namespace EDO_FOMS.Application.Requests.Directories
{
    public class GetPagedRoutesRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public bool MatchCase { get; set; }

        public GetPagedRoutesRequest() { }
        public GetPagedRoutesRequest(
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

            OrderBy = string.IsNullOrEmpty(orderings) ? Array.Empty<string>() : orderings.Split(',');
            MatchCase = matchCase;
        }
    }
}
