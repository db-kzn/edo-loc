using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Responses.Docums
{
    public class DocParticipantResponse
    {
        public int Step { get; set; }
        public string EmplId { get; set; }

        public string Surname { get; set; }
        public string GivenName { get; set; }

        public int OrgId { get; set; }
        public string OrgShortName { get; set; }
        public string OrgInn { get; set; }
        public OrgTypes OrgType { get; set; }
    }
}
