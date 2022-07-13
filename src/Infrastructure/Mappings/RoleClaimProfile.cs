using AutoMapper;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Infrastructure.Models.Identity;

namespace EDO_FOMS.Infrastructure.Mappings
{
    public class RoleClaimProfile : Profile
    {
        public RoleClaimProfile()
        {
            CreateMap<RoleClaimResponse, EdoFomsRoleClaim>()
                .ForMember(nameof(EdoFomsRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(EdoFomsRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            CreateMap<RoleClaimRequest, EdoFomsRoleClaim>()
                .ForMember(nameof(EdoFomsRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(EdoFomsRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();
        }
    }
}