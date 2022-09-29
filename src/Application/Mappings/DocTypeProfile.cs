using AutoMapper;
using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Mappings
{
    public class DocTypeProfile : Profile
    {
        public DocTypeProfile()
        {
            CreateMap<AddEditDocTypeCommand, DocumentType>().ReverseMap();
        }
    }
}
