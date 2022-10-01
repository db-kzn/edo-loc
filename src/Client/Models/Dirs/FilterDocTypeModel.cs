using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Client.Models.Dirs
{
    public class FilterDocTypeModel
    {
        public int Id { get; set; }
        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Default;
        public string Label { get; set; } = string.Empty;

        public bool IsChecked { get; set; } = false;
    }
}
