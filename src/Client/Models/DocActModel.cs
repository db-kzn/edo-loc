using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Responses.Docums;
using System.Collections.Generic;

namespace EDO_FOMS.Client.Models
{
    public class DocActModel
    {
        public RouteStageStepModel Step { get; set; }
        public ContactResponse Contact { get; set; }
        public Dictionary<string, ContactResponse> Contacts { get; set; } = new();
    }
}
