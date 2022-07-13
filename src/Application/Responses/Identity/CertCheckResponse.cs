using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Responses.Identity
{
    public class CertCheckResponse
    {
        public CertCheckResults Result { get; set; }
        public List<OrgCardModel> OrgCards { get; set; }
    }
}
