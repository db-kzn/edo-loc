using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Features.Documents.Queries
{
    public class DocCardResponse
    {
        public List<AgreementResponse> Agreements { get; set; } = new();
        //public List<PacketFileResponse> PacketFiles { get; set; } = null;

        public int Id { get; set; } = 0;                                           // 0 - Новый документ
        public int? PreviousId { get; set; }                                       // Предыдущая версия документа
        public int? ParentId { get; set; }                                         // Родительский документ
        public int RouteId { get; set; } = 0;                                      // Маршрут документа. 0 - не определен

        public string EmplId { get; set; }                                         //  Инициатор подписания
        public int EmplOrgId { get; set; }                                         // Организация инициатора
        public string ExecutorId { get; set; }                                     // Исполнитель
        public ContactResponse Executor { get; set; }

        public bool IsPublic { get; set; } = false;                                // Публичный документ - виден всем сотрудникам Организации
        public int TypeId { get; set; } = 0;                                       // Вид документа: 0 - не определен, 1 - Договор, 2 - Приложение  
        //public string TypeName { get; set; } = string.Empty;                       // Наименование вида документа
        //public string TypeShort { get; set; } = string.Empty;                      // Кратко вид документа

        public string Number { get; set; } = string.Empty;                         // Номер документа
        public DateTime? Date { get; set; } = null;                                // Дата документа
        public string Title { get; set; } = string.Empty;                          // Наименование
        public string Description { get; set; } = string.Empty;                    // Описание

        //public DocStages Stage { get; set; } = DocStages.Undefined;                // Статус документа:
        //public int CurrentStep { get; set; } = 0;                                  // Текущий этап подписания StageNumber
        //public int TotalSteps { get; set; } = 0;                                   // Всего этапов в маршруте

        public string URL { get; set; } = string.Empty;                            // Ссылка для загрузки
        public string FileName { get; set; } = string.Empty;                       // Наименование файла
        //public string StoragePath { get; set; }                                    // Сервис загруки файла
        //public int Version { get; set; }

        //public string CreatedBy { get; set; }
        //public DateTime CreatedOn { get; set; }
    }

    public class AgreementResponse
    {
        //public int? ParentId { get; set; }                         // Родительское согласование, для доп.согласований. Может не быть, если создано маршрутом
        //public bool IsRequired { get; set; } = true;               // Обязательное согласование

        public int? RouteStepId { get; set; } = null;              // Ссылка на шаблон процесса из маршрута документа
        public int StageNumber { get; set; } = 0;                  // Порядковый номер этапа подписания документа
        public bool IsAdditional { get; set; } = true;             // Дополнительный участник согласование
        public ActTypes Action { get; set; } = ActTypes.Undefined; // Действия участников заменены на типы процессов

        public string OmsCode { get; set; } = String.Empty;        // Код МО по НСИ
        public string OrgInn { get; set; } = String.Empty;         // ИНН Организации, eсли организация не зарегистированна в системе
        public int? OrgId { get; set; } = null;                    // Организация подписанта / согласованта
        public string EmplId { get; set; } = String.Empty;         // Сотрудник - согласовант / подписант

        public ContactResponse Contact { get; set; }               // Контакт участника

        public AgreementResponse() { }
        public AgreementResponse(Agreement a)
        {
            OmsCode = a.OmsCode;            // - Contact == null - Организация не зарегистрированна
            OrgInn = a.OrgInn;              // - Contact == null - Организация есть в НСИ
            OrgId = a.OrgId;                // - Contact == null - Организация зарегистрирована, нет сотрудника

            EmplId = a.EmplId;              // +
            Contact = null;                 // +

            StageNumber = a.StageNumber;    // - in Step
            RouteStepId = a.RouteStepId;    // +
            IsAdditional = a.IsAdditional;  // +
            Action = a.Action;              // +
        }
    }

    public class PacketFileResponse
    {
        public int DocumentId { get; set; }
        public int? RoutePacketFileId { get; set; }
        public RoutePacketFile RoutePacketFile { get; set; }

        public string URL { get; set; }
        public string StoragePath { get; set; }
        public string Name { get; set; }
    }
}
