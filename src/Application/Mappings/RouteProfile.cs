using AutoMapper;
using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Mappings
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            CreateMap<AddEditRouteCommand, Route>()
                .ForMember(nameof(Route.DocTypes), opt => opt.Ignore())
                .ForMember(nameof(Route.RouteDocTypes), opt => opt.Ignore())
                .ForMember(nameof(Route.ForOrgTypes), opt => opt.Ignore())
                .ForMember(nameof(Route.Stages), opt => opt.Ignore())
                .ForMember(nameof(Route.Steps), opt => opt.Ignore())
                .ForMember(nameof(Route.Parses), opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
