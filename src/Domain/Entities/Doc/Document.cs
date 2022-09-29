using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class Document : AuditableEntityWithExtendedAttributes<int, int, Document, DocumentExtendedAttribute>
    {
        public List<Agreement> Agreements { get; set; } = new();
        public List<DocPacketFile> PacketFiles { get; set; } = null;

        public string EmplId { get; set; }                                // Инициатор документа -> Заменить на EmployeeId
        public string ExecutorId { get; set; }                            // Исполнитель

        public int EmplOrgId { get; set; }                                // Отправитель - организация инициатора
        [ForeignKey("EmplOrgId")]
        public virtual Organization Issuer { get; set; }                  // Издатель

        public int? KeyOrgId { get; set; } = null;                        // Ключевой участник для отображения в таблице
        [ForeignKey("KeyOrgId")]
        public virtual Organization Recipient { get; set; }               // Получатель

        public int? ParentId { get; set; }                                // Родительский документ
        [ForeignKey("ParentId")]
        public virtual Document Parent { get; set; }
        public int? PreviousId { get; set; }                              // Предыдущая версия документа

        public int RouteId { get; set; }                                  // Маршрут отправки документа
        public DocStages Stage { get; set; } = DocStages.Undefined;       // Статус документа:
        public bool HasChanges { get; set; } = true;                      // Аналог - не прочитан

        public int TypeId { get; set; } = 0;                              // Вид документа: Договор, Приложение. 0 - не определен
        public virtual DocumentType Type { get; set; }
        public string Number { get; set; } = string.Empty;                // Номер документа
        public DateTime? Date { get; set; }                               // Дата документа

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;           // Примечание
        public bool IsPublic { get; set; } = false;                       // Публичный - доступен всем сотрудникам организации
        public int? DepartmentId { get; set; } = null;                    // Документ отдела
        public Department Department { get; set; }

        public int CurrentStep { get; set; }                              // Текущий этап подписания
        public int TotalSteps { get; set; }                               // Всего этапов подписания
        public int Version { get; set; }                                  // Номер попытки прохождения документа
        public DateTime SignStartAt { get; set; }                         // Подписание началось в (для присоединенных подписей PDF)

        public string URL { get; set; }
        public string StoragePath { get; set; }
        public string FileName { get; set; }

        //public bool IsPackage { get; set; } = false;                      // Является пакетом документов, а не единичным файлом
        //public bool CalcHash { get; set; } = false;                       // Рассчитывать хэш документа
        //public bool AttachedSign { get; set; } = false;                   // Прикрепленная подпись руководителя
        //public bool DisplayedSign { get; set; } = false;                  // Отображаемая подпись руководителя

        //public bool AllowRevocation { get; set; } = true;                 // Возможность отзывать документ с маршрута
        //public bool UseVersioning { get; set; } = false;                  // Используется версионность
        //public EndActions EndAction { get; set; } = EndActions.ToArchive; // Действие по завершению маршрута
    }
}