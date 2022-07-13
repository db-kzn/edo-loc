using AutoMapper;
using EDO_FOMS.Infrastructure.Models.Audit;
using EDO_FOMS.Application.Responses.Audit;

namespace EDO_FOMS.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}