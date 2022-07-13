using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Features.Agreements.Queries
{
    public class AgreementsProgressResponse
    {
        public int AgreementId { get; set; }
        public int DocumentId { get; set; }
        public int DocTypeId { get; set; }

        public string EmplId { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public int UserOrgId { get; set; }
        public OrgTypes UserOrgType { get; set; }
        public string UserOrgShortName { get; set; }
        public string UserOrgInn { get; set; }

        public int Step { get; set; }                       // Порядковый номер этапа подписания документа
        public AgreementStates State { get; set; } = AgreementStates.Undefined; // Обновляется после прохождения этапа
        public AgreementActions Action { get; set; }
        public bool IsCanceled { get; set; } = false;       // Отмененное согласование

        public DateTime? CreatedOn { get; set; } = null;    // Время создания
        public DateTime? Received { get; set; } = null;     // Время получения
        public DateTime? Opened { get; set; } = null;     // Время получения
        public DateTime? Answered { get; set; } = null;     // Время действия

        public string Remark { get; set; }                   // Замечания
        public string SignURL { get; set; }                      // Ссылка на файл подписи
    }
}
