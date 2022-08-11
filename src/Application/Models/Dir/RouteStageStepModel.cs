using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteStageStepModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }                          // - Внешний индекс
        //public virtual Route Route { get; set; }

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

        public RouteStageStepModel() { }
        public RouteStageStepModel(RouteStageStep s)
        {
            Id = s.Id;
            RouteId = s.RouteId;

            StageNumber = s.StageNumber;
            Number = s.Number;

            ActType = s.ActType;
            OrgType = s.OrgType;
            OnlyHead = s.OnlyHead;

            Requred = s.Requred;
            SomeParticipants = s.SomeParticipants;
            AllRequred = s.AllRequred;

            HasAgreement = s.HasAgreement;
            HasReview = s.HasReview;
        }
    }
}
