using AutoMapper;
using EDO_FOMS.Application.Features.DocumentTypes.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries.GetAll;
using EDO_FOMS.Application.Features.DocumentTypes.Queries.GetById;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
            CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
        }
    }
}