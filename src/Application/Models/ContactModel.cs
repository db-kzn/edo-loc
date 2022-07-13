using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Models
{
    public class ContactModel
    {
        public int OrgId { get; set; }
        public string OrgInn { get; set; }
        public string OrgShortName { get; set; }

        public string EmplId { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public int AgreementId { get; set; }
        public AgreementStates State { get; set; }
    }
}
