using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class Agreement : AuditableEntity<int>
    {
        public int DocumentId { get; set; }                                        // Документ к которому относится согласование
        public virtual Document Document { get; set; }
        public int? ParentId { get; set; } = null;                                 // Родительское согласование, для доп.согласований
        public int? RouteStepId { get; set; } = null;                              // Ссылка на шаблон процесса из маршрута документа
        public int StageNumber { get; set; } = 0;                                  // Prev.name |> Step // Порядковый номер этапа подписания документа

        public string OmsCode { get; set; } = string.Empty;                         // Код МО по НСИ
        public string OrgInn { get; set; } = string.Empty;                         // ИНН Организации, eсли организация не зарегистированна в системе
        public int? OrgId { get; set; } = null;                                    // Организация подписанта / согласованта
        public Organization Org { get; set; }                                      // OrgId, OrgType, OrgInn, OrgShortName
        public string EmplId { get; set; } = string.Empty;                         // Сотрудник - согласовант / подписант

        public bool IsRequired { get; set; } = false;                              // Обязательное согласование
        public bool IsCanceled { get; set; } = false;                              // Отмененное согласование
        public bool IsAdditional { get; set; } = false;                            // Дополнительный участник согласование

        //public ActTypes Act { get; set; } = ActTypes.Undefined;                    // Тип процесса: подписание, согласование или рецензирование
        //public AgreementActions Action { get; set; } = AgreementActions.Undefined; // Действие участника 
        public ActTypes Action { get; set; } = ActTypes.Undefined;                 // Действия участников заменены на типы процессов
        public AgreementStates State { get; set; } = AgreementStates.Undefined;    // Состояние текущего согласования

        public DateTime? Received { get; set; } = null;                            // Время получения
        public DateTime? Opened { get; set; } = null;                              // Время открытия
        public DateTime? Answered { get; set; } = null;                            // Время действия

        public string Remark { get; set; } = string.Empty;                         // Замечания / предложения
        public string SignURL { get; set; } = string.Empty;                        // Ссылка на файл подписи
        public int? CertId { get; set; } = null;                                   // Идентификатор сертификата используемого для подписания
    }
}
