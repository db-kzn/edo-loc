using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Identity.Authentication
{
    public interface IAuthenticationManager : IManager
    {
        Task<IResult> Login(TokenRequest model);

        Task<IResult> SignIn(CertCheckRequest model);

        Task<IResult<CertCheckResponse>> CertCheck(CertCheckRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}