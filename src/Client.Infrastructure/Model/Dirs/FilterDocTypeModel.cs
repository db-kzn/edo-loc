using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Client.Infrastructure.Models.Dirs
{
    public class FilterDocTypeModel
    {
        public int Id { get; set; }
        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Default;
        public string Label { get; set; } = string.Empty;

        public bool IsChecked { get; set; } = false;

        public FilterDocTypeModel(FilterDocTypeModel dt)
        {
            Id = dt.Id;
            Icon = dt.Icon;
            Color = dt.Color;
            Label = dt.Label;

            IsChecked = dt.IsChecked;
        }
        public FilterDocTypeModel(GetAllDocTypeTitlesResponse response)
        {
            Id = response.Id;
            Icon = response.Icon;
            Color = response.Color;
            Label = response.Label;

            IsChecked = false;
        }
    }
}
