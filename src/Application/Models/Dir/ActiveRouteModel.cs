namespace EDO_FOMS.Application.Models.Dir
{
    public class ActiveRouteModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int? Count { get; set; } = null;

        public string Code { get; set; } = string.Empty;
        public string Short { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool ParseFileName { get; set; } = false;
        public string Mask { get; set; } = string.Empty;
    }
}
