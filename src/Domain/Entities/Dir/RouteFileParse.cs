using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteFileParse : AuditableEntity<int>
    {
        // Id
        public int RouteId { get; set; }                             // Внешний индекс
        public Route Route { get; set; }                             // Навигационное поле

        public ParseTypes Type { get; set; } = ParseTypes.Undefined; // Типы разбора
        public string Pattern { get; set; } = string.Empty;          // Паттерн для Regex
    }
}
