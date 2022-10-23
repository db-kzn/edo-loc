using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Responses.Agreements
{
    public class DocAgreementsCardResponse
    {
        //public List<DocStage> Stages { get; set; }
        public List<DocAgrCardResponse> Agreements { get; set; }

        public int DocId { get; set; }
        public int RouteId { get; set; }
        public DocIcons Icon { get; set; }

        public string EmplId { get; set; }                                         //  Инициатор подписания
        public int EmplOrgId { get; set; }                                         // Организация инициатора
        public string ExecutorId { get; set; }                                     // Исполнитель

        public string Number { get; set; } = string.Empty;                         // Номер документа
        public DateTime? Date { get; set; } = null;                                // Дата документа
        public string Title { get; set; } = string.Empty;                          // Наименование
        public string Description { get; set; } = string.Empty;                    // Описание
    }

    //public class DocStage
    //{
    //    public int Number { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }

    //    public ActTypes ActType { get; set; }
    //    public bool IsSerial { get; set; }
    //    public bool AllRequired { get; set; }
    //}

    public class DocAgrCardResponse
    {
        public int DocumentId { get; set; }
        public int AgreementId { get; set; }
        public int? RouteStepId { get; set; } = null;              // Ссылка на шаблон процесса из маршрута документа
        public int StageNumber { get; set; } = 0;                  // Порядковый номер этапа подписания документа
        public bool IsAdditional { get; set; } = true;             // Дополнительный участник согласование
        public bool IsCanceled { get; set; } = true;             // Отмененное участие

        public int? EmplOrgId { get; set; } = null;                // Организация подписанта / согласованта
        public OrgTypes OrgType { get; set; }
        public string OmsCode { get; set; } = string.Empty;        // Код МО по НСИ
        public string OrgInn { get; set; }
        public string OrgShort { get; set; }

        public string EmplId { get; set; }
        public string EmplTitle { get; set; }
        public string EmplSurname { get; set; }
        public string EmplGivenName { get; set; }
        //public ContactResponse Contact { get; set; }               // Контакт участника

        public string CertThumbprint { get; set; }
        //public string CertSerial { get; set; }
        public DateTime? CertFromDate { get; set; }
        public DateTime? CertTillDate { get; set; }
        public string CertAlgorithm { get; set; }

        public int Step { get; set; }                                           // Порядковый номер этапа подписания документа
        public ActTypes Action { get; set; }
        public AgreementStates State { get; set; } = AgreementStates.Undefined; // Обновляется после прохождения этапа
        public DateTime? Answered { get; set; } = null;                         // Время действия

        public string Remark { get; set; }                                      // Замечания
        public string SignURL { get; set; }                                     // Ссылка на файл подписи
    }
}
