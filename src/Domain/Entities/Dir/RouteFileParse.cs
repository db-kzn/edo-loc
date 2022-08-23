using EDO_FOMS.Domain.Contracts;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteFileParse : AuditableEntity<int>
    {
        // Id
        public int RouteId { get; set; }                   // - Внешний индекс
        public Route Route { get; set; }                   // - Навигационное поле

        public string Name { get; set; }                   // + Наименование правила
    }
}
