using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Orgs
{
    public interface IOrgManager : IManager
    {
        //Task<IResult<List<GetAllOrgsResponse>>> GetAllAsync();
        Task<IResult<Organization>> GetByInnAsync(string inn);
        Task<IResult<int>> SaveAsync(AddEditOrgCommand request);
        Task<IResult<GetOrgByIdResponse>> GetByIdAsync(GetOrgByIdQuery request);

        Task<IResult<int>> GetIdByCodeAsync(string code);

        //Task<IResult<int>> DeleteAsync(int id);
        //Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
