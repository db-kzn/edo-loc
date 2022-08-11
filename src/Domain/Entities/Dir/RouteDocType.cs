using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteDocType
    {
        public int? RouteId { get; set; }
        public Route Route { get; set; }

        public int? DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
