using EDO_FOMS.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Org
{
    public class Department : AuditableEntity<int>
    {
        public int? OrgId { get; set; } = null;
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Short { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;
    }
}
