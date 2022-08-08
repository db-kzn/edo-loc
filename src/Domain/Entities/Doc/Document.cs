using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class Document : AuditableEntityWithExtendedAttributes<int, int, Document, DocumentExtendedAttribute>
    {
        public virtual List<Agreement> Agreements { get; set; } = new();

        public string EmplId { get; set; }          // Инициатор документа -> Заменить на EmployeeId
        public int EmplOrgId { get; set; }          // Отправитель - организация инициатора        
        [ForeignKey("EmplOrgId")]
        public virtual Organization Issuer { get; set; }
        
        public int? ParentId { get; set; }          // Родительский документ
        [ForeignKey("ParentId")]
        public virtual Document Parent { get; set; }
        
        public int? PreviousId { get; set; }        // Предыдущая версия документа
        // Получатели по справочнику СОГЛАСОВАНИЯ
        public int RouteId { get; set; }            // Пока только один в системе
        //public int StatusIx { get; set; } = 0;      // Статус
        public Enums.DocStages Stage { get; set; }  // Статус документа:
        public bool HasChanges { get; set; }        // Аналог - не прочитан

        public int TypeId { get; set; }             // Вид документа: Договор, Приложение
        public virtual DocumentType Type { get; set; }
        public string Number { get; set; }          // Номер документа
        public DateTime Date { get; set; }          // Дата документа

        public string Title { get; set; }
        public string Description { get; set; }     // Примечание
        public bool IsPublic { get; set; } = false; // Публичный

        public int CurrentStep { get; set; }        // Текущий этап подписания
        public int TotalSteps { get; set; }         // Всего этапов подписания
        public int Version { get; set; }            // Номер попытки прохождения документа
        //public int Attempt { get; set; }            // Номер попытки прохождения документа

        public string URL { get; set; }
        public string StoragePath { get; set; }
        public string FileName { get; set; }
    }
}