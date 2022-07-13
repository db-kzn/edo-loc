using AutoMapper;
using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Domain.Entities.Org;

namespace EDO_FOMS.Application.Mappings
{
    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<AddEditCertCommand, Certificate>().ReverseMap();
            //CreateMap<GetCertByIdResponse, Certificate>().ReverseMap();
            CreateMap<GetUserCertsResponse, Certificate>().ReverseMap();
            CreateMap<CertsResponse, Certificate>().ReverseMap();
        }
    }
}
