using EDO_FOMS.Application.Features.Orgs.Queries;
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

        public int StageNumber { get; set; } = 0;                   // + Номер этапа
        public int Number { get; set; } = 0;                        // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing;   // + Тип шага: подписание, согласование или рецензирование
        public bool Requred { get; set; } = true;                   // + Обязательный шаг

        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined; // + Тип организации, может быть не определен
        public int? OrgId { get; set; } = null;                     // + Индекс организации участника
        public OrgsResponse OrgMember { get; set; } = null;         // + Организация участник

        public bool OnlyHead { get; set; }                          // + Требуется руководитель
        public int AutoSearch { get; set; } = 0;                    // + Автопоиск - количество записей

        public bool SomeParticipants { get; set; } = true;          // - Несколько участников
        public bool AllRequred { get; set; } = true;                // + Если несколько, то условие завершения: все или любой

        public bool HasAgreement { get; set; } = false;             // + Содержит согласование
        public bool HasReview { get; set; } = false;                // + Содержит рецензирование

        public List<RouteStepMemberModel> Members { get; set; } = new(); // + Список участников

        public RouteStepModel() { }
        public RouteStepModel(RouteStep s)
        {
            Id = s.Id;
            RouteId = s.RouteId;

            StageNumber = s.StageNumber;
            Number = s.Number;

            ActType = s.ActType;
            AutoSearch = s.AutoSearch;

            OrgType = s.OrgType;
            OrgId = s.OrgId;
            OrgMember = null; // RouteStep has no Organization Object

            Requred = s.Requred;
            OnlyHead = s.OnlyHead;

            SomeParticipants = s.SomeParticipants;
            AllRequred = s.AllRequred;

            HasAgreement = s.HasAgreement;
            HasReview = s.HasReview;

            Members = s.Members.Select(m => new RouteStepMemberModel()
            {
                Act = m.Act,
                IsAdditional = m.IsAdditional,
                UserId = m.UserId,
                Contact = null // RouteStep has no Contact Object
            }).ToList();
        }
    }
}
