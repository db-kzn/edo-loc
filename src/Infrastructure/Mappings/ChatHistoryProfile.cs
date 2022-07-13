using AutoMapper;
using EDO_FOMS.Application.Interfaces.Chat;
using EDO_FOMS.Application.Models.Chat;
using EDO_FOMS.Infrastructure.Models.Identity;

namespace EDO_FOMS.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<EdoFomsUser>>().ReverseMap();
        }
    }
}