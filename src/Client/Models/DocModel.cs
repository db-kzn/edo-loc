using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Client.Models
{
    public class DocModel
    {
        public int EmplOrgId { get; set; }   // Организация издатель - Issuer
        public string EmplId { get; set; }   // Исполнитель
        public int? AgreementId { get; set; } = null; // Согласование шага "0" - отправитель документа - контроль
        
        public List<int> Recipients { get; set; } // Список организаций получателей

        //public OrgTypes EmplOrgType { get; set; } // Организация отправителя
        //public string EmplOrgShortName { get; set; }
        //public string EmplOrgOrgInn { get; set; }

        public int DocId { get; set; }     // DocumentId
        public int? ParentId { get; set; } // DocParentId
        public int RouteId { get; set; }   // Маршрут
        public bool IsPublic { get; set; } // Публичность документа

        public int TypeId { get; set; }
        //public string Type { get; set; }
        public string TypeName { get; set; }
        public string TypeShort { get; set; }

        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public string URL { get; set; }
        public string FileName { get; set; }

        public DocStages Stage { get; set; }
        public bool HasChanges { get; set; }
        public string StageName { get; set; }
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }

        public int Version { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnStr { get; set; }
    }
}
