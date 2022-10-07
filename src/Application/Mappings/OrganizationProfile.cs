using AutoMapper;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Domain.Entities.Org;

namespace EDO_FOMS.Application.Mappings
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<AddEditOrgCommand, Organization>().ReverseMap();
            CreateMap<GetOrgByIdResponse, Organization>().ReverseMap();
            //CreateMap<GetOrgByInnResponse, Organization>().ReverseMap();
            CreateMap<OrgsResponse, Organization>().ReverseMap();
        }
    }
}
