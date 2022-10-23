using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MudBlazor;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Orgs
{
    public class OrgManager : IOrgManager
    {
        private readonly HttpClient _httpClient;

        public OrgManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<IResult<List<GetAllOrgsResponse>>> GetAllAsync()
        //{
        //    var response = await _httpClient.GetAsync(Routes.OrgsEndpoints.GetAll);
        //    return await response.ToResult<List<GetAllOrgsResponse>>();
        //}

        public async Task<IResult<GetOrgByIdResponse>> GetByIdAsync(GetOrgByIdQuery request)
        {
            var response = await _httpClient.GetAsync(Routes.OrgsEndpoints.GetById(request.Id));
            return await response.ToResult<GetOrgByIdResponse>();
        }

        public async Task<IResult<Organization>> GetByInnAsync(string inn)
        {
            var response = await _httpClient.GetAsync(Routes.OrgsEndpoints.GetByInn(inn));
            return await response.ToResult<Organization>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditOrgCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.OrgsEndpoints.Save, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> GetIdByCodeAsync(string code)
        {
            var response = await _httpClient.GetAsync(Routes.OrgsEndpoints.GetIdByCode(code));
            return await response.ToResult<int>();
        }
    }
}
