using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class Agreement : AuditableEntity<int>
    {
        public int Number { get; set; } = 0;               // Номер в цепочке процессов
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }
        public int? ParentId { get; set; }                 // Родительское согласование, для доп.согласований

        public int OrgId { get; set; }                     // Организация подписанта / согласованта
        public Organization Org { get; set; }              // OrgId, OrgType, OrgInn, OrgShortName
        public string EmplId { get; set; }                 // Сотрудник - согласовант / подписант

        // Если организация не зарегистированна в системе
        public string OrgInn { get; set; }                 // ИНН Организации
        public OrgTypes OrgType { get; set; }              // Тип Организации
        public bool OnlyHead { get; set; }                 // Требуется руководитель

        public bool InSeries { get; set; } = false;        // Последовательное прохождение
        public bool AllRequred { get; set; } = true;       // Условие завершения: все или любой (по организации)
        public bool AllowAgreement { get; set; } = false;  // Разрешено доп.согласование
        public bool AllowReview { get; set; } = false;     // Разрешено рецензирование

        public int Step { get; set; }                      // Порядковый номер этапа подписания документа
        public AgreementStates State { get; set; } = AgreementStates.Undefined; // Обновляется после прохождения этапа
        public AgreementActions Action { get; set; } = AgreementActions.Undefined;
        public bool IsCanceled { get; set; } = false;      // Отмененное согласование

        public string Remark { get; set; }                 // Замечания / предложения
        public DateTime? Received { get; set; } = null;    // Время получения
        public DateTime? Opened { get; set; } = null;      // Время открытия
        public DateTime? Answered { get; set; } = null;    // Время действия

        public string SignURL { get; set; }                // Ссылка на файл подписи
        public int? CertId { get; set; }                   // Идентификатор сертификата используемого для подписания
    }
}
