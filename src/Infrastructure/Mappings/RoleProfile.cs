using AutoMapper;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Application.Responses.Identity;

namespace EDO_FOMS.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, EdoFomsRole>().ReverseMap();
        }
    }
}