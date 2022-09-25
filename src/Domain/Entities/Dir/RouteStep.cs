using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteStep : AuditableEntity<int>
    {
        // Id
        public int RouteId { get; set; }                                        // - Внешний индекс. При удалении устанавливается IsDeleted
        public Route Route { get; set; }                                        // - "Удалляенный" процесс может использоваться в активных Agreements
        public bool IsDeleted { get; set; } = false;                            // - Удаленный шаг маршрута

        public int StageNumber { get; set; } = 0;                               // + Номер этапа, к которому относится процесс
        public int Number { get; set; } = 0;                                    // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing;               // + Тип шага: подписание, согласование или рецензирование
        public MemberGroups MemberGroup { get; set; } = MemberGroups.Undefined; // + Группа участников

        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;             // + Тип организации, может быть не определен
        public int? OrgId { get; set; } = null;                                 // + Организация участник

        public bool IsKeyMember { get; set; } = false;                          // + Является ключевым участником
        public bool Requred { get; set; } = true;                               // + Обязательный шаг

        public bool SomeParticipants { get; set; } = true;                      // - Несколько участников
        public bool AllRequred { get; set; } = true;                            // + Если несколько, то условие завершения: все или любой

        public int AutoSearch { get; set; } = 0;                                // + Количество участников для автопоиска. 0 - без поиска. 10 -макс.
        public bool HasAgreement { get; set; } = false;                         // + Содержит доп.согласование
        public bool HasReview { get; set; } = false;                            // + Содержит рецензирование

        public string Description { get; set; }                                 // - Описание процесса
        public List<RouteStepMember> Members { get; set; }                      // + Участники процесса
    }
}
