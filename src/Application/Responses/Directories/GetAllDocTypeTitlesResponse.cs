using EDO_FOMS.Domain.Enums;
using MudBlazor;

namespace EDO_FOMS.Application.Responses.Directories;

public class GetAllDocTypeTitlesResponse
{
    public int Id { get; set; }
    public DocIcons Icon { get; set; } = DocIcons.Undefined;
    public Color Color { get; set; } = Color.Default;
    public string Label { get; set; } = string.Empty;
}
