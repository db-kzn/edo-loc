using AutoMapper;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;

namespace EDO_FOMS.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}