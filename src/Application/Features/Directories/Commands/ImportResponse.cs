namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class ImportResponse
    {
        public int Total { get; set; }
        public int Added { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
    }
}
