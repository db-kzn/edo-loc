using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.System
{
    public class Param : AuditableEntity<int>
    {
        public int ParamGroupId { get; set; }
        public ParamGroup Group { get; set; }

        public string Property { get; set; }
        public string Value { get; set; }
    }
}
