using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Application.Features.Directories.Queries
{
    public class DocTypesResponse
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Primary;

        public string Short { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }

        public string NameEn { get; set; }
        public string Description { get; set; }
    }
}
