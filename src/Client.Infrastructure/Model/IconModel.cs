namespace EDO_FOMS.Client.Infrastructure.Model
{
    public class IconModel
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;

        public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Default;
        public MudBlazor.Size Size { get; set; } = MudBlazor.Size.Medium;
    }
}
