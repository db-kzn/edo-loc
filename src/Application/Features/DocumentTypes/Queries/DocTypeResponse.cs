namespace EDO_FOMS.Application.Features.DocumentTypes.Queries
{
    public class DocTypeResponse
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString() { return Label; }
    }
}