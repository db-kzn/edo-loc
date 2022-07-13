using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests.Agreements
{
    public class GetPagedAgreementsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public bool MatchCase { get; set; }
        public AgreementStates AgrState { get; set; }

        public GetPagedAgreementsRequest() { }
        public GetPagedAgreementsRequest(
            int pageSize,
            int pageNumber,
            string searchString,
            
            string orderings,
            AgreementStates agrState = AgreementStates.Undefined,
            bool matchCase = false
            )
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            SearchString = searchString;

            OrderBy = string.IsNullOrEmpty(orderings) ? System.Array.Empty<string>() : orderings.Split(',');
            AgrState = agrState;
            MatchCase = matchCase;
        }
    }
}
