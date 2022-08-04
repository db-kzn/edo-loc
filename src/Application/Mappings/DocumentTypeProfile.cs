using AutoMapper;
using EDO_FOMS.Application.Features.DocumentTypes.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<DocTypeResponse, DocumentType>().ReverseMap();
            CreateMap<DocTypeResponse, DocumentType>().ReverseMap();
        }
    }
}