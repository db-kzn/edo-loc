using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}