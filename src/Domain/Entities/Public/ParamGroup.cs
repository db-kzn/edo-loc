using EDO_FOMS.Domain.Contracts;
using System.Collections.Generic;

namespace EDO_FOMS.Domain.Entities.Public
{
    public class ParamGroup : AuditableEntity<int>
    {
        public string Name { get; set; }
        public int Version { get; set; } = 0;

        public List<Param> Params { get; set; }
    }
}
