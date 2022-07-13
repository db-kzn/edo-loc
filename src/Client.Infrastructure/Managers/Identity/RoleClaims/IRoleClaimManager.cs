using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Client.Infrastructure.Managers.Identity.RoleClaims
{
    public interface IRoleClaimManager : IManager
    {
        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsAsync();

        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

        Task<IResult<string>> SaveAsync(RoleClaimRequest role);

        Task<IResult<string>> DeleteAsync(string id);
    }
}