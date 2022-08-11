using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteOrgType : AuditableEntity<int>
    {
        public int? RouteId { get; set; }
        public Route Route { get; set; }

        public OrgTypes OrgType { get; set; }
    }
}
