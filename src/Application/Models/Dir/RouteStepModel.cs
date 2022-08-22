using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteStepModel
    {
        public int Id { get; set; }                                 // - RouteStepId
        public int RouteId { get; set; }                            // - Внешний индекс
        // IsDeleted - на клиенте не используется, только на сервере
        public int StageNumber { get; set; }                        // + Номер этапа
        public int Number { get; set; }                             // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing;   // + Тип шага: подписание, согласование или рецензирование
        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined; // + Тип организации, может быть не определен
        public int AutoSearch { get; set; } = 0;                    // + Автопоиск - количество записей
        public List<RouteStepMemberModel> Members { get; set; } = new(); // + Список участников

        public bool OnlyHead { get; set; }                          // + Требуется руководитель
        public bool Requred { get; set; } = true;                   // + Обязательный шаг
        public bool SomeParticipants { get; set; } = true;          // - Несколько участников

        public bool AllRequred { get; set; } = true;                // + Если несколько, то условие завершения: все или любой
        public bool HasAgreement { get; set; } = false;             // + Содержит согласование
        public bool HasReview { get; set; } = false;                // + Содержит рецензирование

        public RouteStepModel() { }
        public RouteStepModel(RouteStep s)
        {
            Id = s.Id;
            RouteId = s.RouteId;
            StageNumber = s.StageNumber;
            Number = s.Number;

            ActType = s.ActType;
            OrgType = s.OrgType;
            AutoSearch = s.AutoSearch;
            Members = s.Members.Select(m => new RouteStepMemberModel()
            {
                Act = m.Act,
                IsAdditional = m.IsAdditional,
                UserId = m.UserId,
                Contact = null
            }).ToList();

            OnlyHead = s.OnlyHead;
            Requred = s.Requred;
            SomeParticipants = s.SomeParticipants;
            AllRequred = s.AllRequred;
            HasAgreement = s.HasAgreement;
            HasReview = s.HasReview;
        }
    }
}
