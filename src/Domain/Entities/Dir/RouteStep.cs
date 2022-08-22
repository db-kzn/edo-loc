using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteStep : AuditableEntity<int>
    {
        // Id
        public int RouteId { get; set; }                          // - Внешний индекс. При удалении устанавливается IsDeleted
        public Route Route { get; set; }                          // - Удалляемый шаг может использоваться в активных Agreements
        public bool IsDeleted { get; set; } = false;              // - Удаленный шаг маршрута
        public int StageNumber { get; set; }                      // + Номер этапа, к которому относится процесс
        public int Number { get; set; }                           // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing; // + Тип шага: подписание, согласование или рецензирование
        public OrgTypes OrgType { get; set; }                     // + Тип организации, может быть не определен
        public int AutoSearch { get; set; }                       // + Количество участников для автопоиска. 0 - без поиска. 10 -макс.
        public List<RouteStepMember> Members { get; set; }        // + Участники процесса

        public bool OnlyHead { get; set; }                        // + Требуется руководитель
        public bool Requred { get; set; } = true;                 // + Обязательный шаг
        public bool SomeParticipants { get; set; } = true;        // - Несколько участников

        public bool AllRequred { get; set; } = true;              // + Если несколько, то условие завершения: все или любой
        public bool HasAgreement { get; set; } = false;           // + Содержит доп.согласование
        public bool HasReview { get; set; } = false;              // + Содержит рецензирование
    }
}
