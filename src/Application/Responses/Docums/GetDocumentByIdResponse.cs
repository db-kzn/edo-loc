using System;

namespace EDO_FOMS.Application.Responses.Docums
{
    public class GetDocumentByIdResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string URL { get; set; }
        public string TypeName { get; set; }
        public int TypeId { get; set; }
    }
}