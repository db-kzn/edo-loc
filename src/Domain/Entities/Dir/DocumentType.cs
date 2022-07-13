using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class DocumentType : AuditableEntity<int>
    {
        public bool IsActive { get; set; } = true;
        public string Name { get; set; }
        public string Short { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
    }
}