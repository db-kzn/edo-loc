using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Responses.Agreements
{
    public class EmployeeAgreementsResponse
    {
        public int AgreementId { get; set; }
        public string EmplId { get; set; }

        public int? EmplOrgId { get; set; }
        public int? ParentAgreementId { get; set; }

        public int? KeyOrgId { get; set; } = null;
        public string RecipientShort { get; set; } = string.Empty;
        public string RecipientInn { get; set; } = string.Empty;

        public List<ContactModel> Recipients { get; set; } // Список организаций получателей

        // Организация получателя
        //public OrgTypes EmplOrgType { get; set; } 
        //public string EmplOrgShortName { get; set; }
        //public string EmplOrgOrgInn { get; set; }

        // Организация отправителя
        public int IssuerOrgId { get; set; }
        //public Organization Issuer { get; set; }
        public OrgTypes IssuerType { get; set; }
        public string IssuerOrgInn { get; set; }
        public string IssuerOrgShortName { get; set; }

        // Документ - предмет согласования
        public int DocId { get; set; }
        public int? DocParentId { get; set; }
        public int DocRouteId { get; set; }
        public bool DocIsPublic { get; set; }

        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string DocTypeShort { get; set; }

        public string DocNumber { get; set; }
        public DateTime? DocDate { get; set; }
        public string DocTitle { get; set; }

        public string DocDescription { get; set; }
        public string DocURL { get; set; }
        public string DocFileName { get; set; }

        public DocStages DocStage { get; set; }
        public bool DocHasChanges { get; set; }
        public int DocCurrentStep { get; set; }
        public int DocTotalSteps { get; set; }

        public int DocVersion { get; set; }
        public string DocCreatedBy { get; set; }
        public DateTime DocCreatedOn { get; set; }

        // Данные о согласовании
        public int Step { get; set; } // Порядковый номер этапа подписания документа
        public AgreementStates State { get; set; } = AgreementStates.Undefined; // Обновляется после прохождения этапа
        public ActTypes Action { get; set; }
        public bool IsCanceled { get; set; } = false;       // Отмененное согласование

        public DateTime CreatedOn { get; set; }             // Время создания
        public DateTime? Received { get; set; } = null;     // Время получения
        public DateTime? Opened { get; set; } = null;       // Время открытия
        public DateTime? Answered { get; set; } = null;     // Время действия

        public string Remark { get; set; } // Замечания
        public string SignURL { get; set; } // Ссылка на файл подписи
    }
}
