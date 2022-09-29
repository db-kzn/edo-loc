using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Application.Features.Directories.Queries
{
    public class DocTypesResponse
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Default;
        public string Label { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public string Short { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
