using AutoMapper;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Application.Responses.Docums;

namespace EDO_FOMS.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, EdoFomsUser>().ReverseMap();
            CreateMap<ContactResponse, EdoFomsUser>().ReverseMap();
            CreateMap<ChatUserResponse, EdoFomsUser>().ReverseMap()
                .ForMember( //Specific Mapping
                    dest => dest.EmailAddress,
                    source => source.MapFrom(source => source.Email)
                    ); 
        }
    }
}