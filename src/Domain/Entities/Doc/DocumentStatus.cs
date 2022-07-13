using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class DocumentStatus : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
