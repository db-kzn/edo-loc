using AutoMapper;
using EDO_FOMS.Application.Features.Documents.Commands;
using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Entities.Doc;

namespace EDO_FOMS.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
            CreateMap<AddEditDocCommand, Document>().ReverseMap();
            CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
        }
    }
}