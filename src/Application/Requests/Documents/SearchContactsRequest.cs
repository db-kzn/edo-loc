using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests.Documents
{
    public class SearchContactsRequest
    {
        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Undefined;
        public string SearchString { get; set; } = "";
    }
}
