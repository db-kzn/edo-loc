using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using MudBlazor;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class DocumentType : AuditableEntity<int>
    {
        public virtual List<Route> Routes { get; set; } = new();
        public virtual List<RouteDocType> RouteDocTypes { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Primary;

        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Short { get; set; } = string.Empty;
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [MaxLength(300)]
        public string NameEn { get; set; } = string.Empty;
        [MaxLength(4000)]
        public string Description { get; set; } = string.Empty;
    }
}