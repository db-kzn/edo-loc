using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteStageStep : AuditableEntity<int>
    {
        public int RouteId { get; set; }                          // - Внешний индекс
        public virtual Route Route { get; set; }

        public int StageNumber { get; set; }                      // + Номер этапа
        public int Number { get; set; }                           // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing; // + Тип шага: подписание, согласование или рецензирование
        public OrgTypes OrgType { get; set; }                     // + Тип организации, может быть не определен
        public bool OnlyHead { get; set; }                        // + Требуется руководитель

        public bool Requred { get; set; } = true;                 // + Обязательный шаг
        public bool SomeParticipants { get; set; } = true;        // - Несколько участников
        public bool AllRequred { get; set; } = true;              // + Если несколько, то условие завершения: все или любой

        public bool HasAgreement { get; set; } = false;           // + Содержит согласование
        public bool HasReview { get; set; } = false;              // + Содержит рецензирование
    }
}
