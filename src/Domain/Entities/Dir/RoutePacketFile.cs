using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RoutePacketFile : AuditableEntity<int>
    {
        // Id
        public int RouteId { get; set; }                                        // - Внешний индекс. При удалении устанавливается IsDeleted
        public Route Route { get; set; }                                        // - "Удалляенный" процесс может использоваться в активных Agreements

        public string FileType { get; set; } = string.Empty;                    // - Наименование типа файла из пакета
        public string FileMask { get; set; } = string.Empty;                    // - Маска файла из пакета
        public string FileAccept { get; set; } = string.Empty;                  // - Расширения файла
    }
}
