using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Dir
{
    public class DirectoryManager : IDirectoryManager
    {
        private readonly HttpClient _httpClient;
        public DirectoryManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaginatedResult<CompaniesResponse>> GetCompaniesAsync(GetPagedCompaniesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.GetCompaniesPaged(request));
            return await response.ToPaginatedResult<CompaniesResponse>();
        }

        public async Task<PaginatedResult<CompaniesResponse>> SearchCompaniesAsync(SearchCompaniesRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DirectoriesEndpoints.SearchCompaniesPaged, request);
            return await response.ToPaginatedResult<CompaniesResponse>();
        }

        public async Task<IResult<CheckCompaniesForImportsResponse>> CheckCompaniesForImportsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.CheckCompaniesForImports);
            return await response.ToResult<CheckCompaniesForImportsResponse>();
        }

        public async Task<IResult<ImportResponse>> ImportFomsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.ImportFoms);
            return await response.ToResult<ImportResponse>();
        }
        public async Task<IResult<ImportResponse>> ImportSmoAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.ImportSmo);
            return await response.ToResult<ImportResponse>();
        }
        public async Task<IResult<ImportResponse>> ImportMoAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.ImportMo);
            return await response.ToResult<ImportResponse>();
        }
    }
}
