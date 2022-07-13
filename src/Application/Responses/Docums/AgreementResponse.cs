using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Responses.Docums
{
    public class AgreementResponse
    {
        public string UserId { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public string OrgId { get; set; }
        public string InnLe { get; set; }

        public int Step { get; set; }
        public OrgTypes OrgType { get; set; }
    }
}
