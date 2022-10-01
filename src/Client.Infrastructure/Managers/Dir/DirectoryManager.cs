using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using MudBlazor;
using System.Collections.Generic;
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

        public async Task<PaginatedResult<DocTypesResponse>> GetDocTypesAsync(GetPagedDocTypesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.GetDocTypesPaged(request));
            return await response.ToPaginatedResult<DocTypesResponse>();
        }
        public async Task<PaginatedResult<DocTypesResponse>> SearchDocTypesAsync(SearchDocTypesRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DirectoriesEndpoints.SearchDocTypesPaged, request);
            return await response.ToPaginatedResult<DocTypesResponse>();
        }
        public async Task<IResult<int>> DocTypePostAsync(AddEditDocTypeCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DirectoriesEndpoints.AddEditDocType, command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllDocTypeTitlesResponse>>> GetAllDocTypeTitlesAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.GetAllDocTypeTitles);
            return await response.ToResult<List<GetAllDocTypeTitlesResponse>>();
        }

        public async Task<PaginatedResult<RoutesResponse>> GetRoutesAsync(GetPagedRoutesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.GetRoutesPaged(request));
            return await response.ToPaginatedResult<RoutesResponse>();
        }
        public async Task<IResult<RouteCardResponse>> GetRouteCardAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.DirectoriesEndpoints.GetRouteCard(id));
            return await response.ToResult<RouteCardResponse>();
        }

        public async Task<IResult<int>> RoutePostAsync(AddEditRouteCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DirectoriesEndpoints.AddEditRoute, command);
            return await response.ToResult<int>();
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

        public string DocTypeIcon(DocIcons icon)
        {
            return icon switch
            {
                DocIcons.AssignmentLate => Icons.Material.Outlined.AssignmentLate,
                DocIcons.AssignmentTurnedIn => Icons.Material.Outlined.AssignmentTurnedIn,
                DocIcons.CalendarToday => Icons.Material.Outlined.CalendarToday,

                DocIcons.ContactPage => Icons.Material.Outlined.ContactPage,
                DocIcons.Description => Icons.Material.Outlined.Description,
                DocIcons.Difference => Icons.Material.Outlined.Difference,

                DocIcons.EventRepeat => Icons.Material.Outlined.EventRepeat,
                DocIcons.FactCheck => Icons.Material.Outlined.FactCheck,
                DocIcons.HelpCenter => Icons.Material.Outlined.HelpCenter,

                DocIcons.Newspaper => Icons.Material.Outlined.Newspaper,
                DocIcons.NoteAdd => Icons.Material.Outlined.NoteAdd,
                DocIcons.Receipt => Icons.Material.Outlined.Receipt,

                DocIcons.TableChart => Icons.Material.Outlined.TableChart,

                _ => Icons.Material.Outlined.HelpOutline
            };
        }
    }
}
