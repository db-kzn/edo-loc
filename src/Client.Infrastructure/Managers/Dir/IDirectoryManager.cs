using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Dir
{
    public interface IDirectoryManager : IManager
    {

        Task<PaginatedResult<CompaniesResponse>> GetCompaniesAsync(GetPagedCompaniesRequest request);
        Task<PaginatedResult<CompaniesResponse>> SearchCompaniesAsync(SearchCompaniesRequest request);

        Task<PaginatedResult<DocTypesResponse>> GetDocTypesAsync(GetPagedDocTypesRequest request);
        Task<PaginatedResult<DocTypesResponse>> SearchDocTypesAsync(SearchDocTypesRequest request);

        Task<IResult<CheckCompaniesForImportsResponse>> CheckCompaniesForImportsAsync();
        Task<IResult<ImportResponse>> ImportFomsAsync();
        Task<IResult<ImportResponse>> ImportSmoAsync();
        Task<IResult<ImportResponse>> ImportMoAsync();
    }
}
