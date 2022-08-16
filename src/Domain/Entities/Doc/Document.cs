﻿using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class Document : AuditableEntityWithExtendedAttributes<int, int, Document, DocumentExtendedAttribute>
    {
        public virtual List<Agreement> Agreements { get; set; } = new();

        public string EmplId { get; set; }                                // Инициатор документа -> Заменить на EmployeeId
        public int EmplOrgId { get; set; }                                // Отправитель - организация инициатора
        [ForeignKey("EmplOrgId")]
        public virtual Organization Issuer { get; set; }
        
        public int? ParentId { get; set; }                                // Родительский документ
        [ForeignKey("ParentId")]
        public virtual Document Parent { get; set; }
        public int? PreviousId { get; set; }                              // Предыдущая версия документа

        public int RouteId { get; set; }                                  // Маршрут отправки документа
        public DocStages Stage { get; set; } = DocStages.Undefined;       // Статус документа:
        public bool HasChanges { get; set; } = true;                      // Аналог - не прочитан

        public int TypeId { get; set; } = 0;                              // Вид документа: Договор, Приложение
        public virtual DocumentType Type { get; set; }
        public string Number { get; set; } = string.Empty;                // Номер документа
        public DateTime Date { get; set; }                                // Дата документа

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;           // Примечание
        public bool IsPublic { get; set; } = false;                       // Публичный - доступен всем сотрудникам организации

        public int CurrentStep { get; set; }                              // Текущий этап подписания
        public int TotalSteps { get; set; }                               // Всего этапов подписания

        public string URL { get; set; }
        public string StoragePath { get; set; }
        public string FileName { get; set; }
        public int Version { get; set; }                                  // Номер попытки прохождения документа

        //public bool IsPackage { get; set; } = false;                      // Является пакетом документов, а не единичным файлом
        //public bool CalcHash { get; set; } = false;                       // Рассчитывать хэш документа
        //public bool AttachedSign { get; set; } = false;                   // Прикрепленная подпись руководителя
        //public bool DisplayedSign { get; set; } = false;                  // Отображаемая подпись руководителя

        //public bool AllowRevocation { get; set; } = true;                 // Возможность отзывать документ с маршрута
        //public bool UseVersioning { get; set; } = false;                  // Используется версионность
        //public EndActions EndAction { get; set; } = EndActions.ToArchive; // Действие по завершению маршрута
    }
}