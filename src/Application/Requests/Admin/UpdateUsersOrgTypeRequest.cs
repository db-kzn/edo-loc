using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests.Admin
{
    public class UpdateUsersOrgTypeRequest
    {
        public int OrgId { get; set; }
        public string InnLe { get; set; }
        public OrgTypes OrgType { get; set; }
    }
}
