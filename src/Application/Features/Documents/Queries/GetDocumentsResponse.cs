using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Features.Documents.Queries
{
    public class GetDocumentsResponse
    {
        public int Id { get; set; } = 0;
        public string EmplId { get; set; }
        
        public int EmplOrgId { get; set; }
        public int? KeyOrgId { get; set; }
        //public virtual Organization Recipient { get; set; }
        public string RecipientShort { get; set; }
        public string RecipientInn { get; set; }

        public int? ParentId { get; set; }
        public int? PreviousId { get; set; }

        public int RouteId { get; set; } = 1;
        public DocStages Stage { get; set; }
        public bool HasChanges { get; set; }

        public int? TypeId { get; set; } = 1;
        public string TypeName { get; set; }
        public string TypeShort { get; set; }
        public string Number { get; set; } = "";
        public DateTime? Date { get; set; } = DateTime.Today;

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsPublic { get; set; } = false;
        public int? DepartmentId { get; set; } = null;

        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }
        public int Version { get; set; }
        //public DateTime SignStartAt { get; set; }

        public string URL { get; set; }
        //public string StoragePath { get; set; }
        public string FileName { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}