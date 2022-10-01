using AutoMapper;
using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Application.Responses.Identity;

namespace EDO_FOMS.Client.Infrastructure.Mappings
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            CreateMap<AddEditRouteCommand, RouteCardResponse>().ReverseMap();
        }
    }
}
