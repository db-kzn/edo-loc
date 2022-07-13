using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> SignInAsync(CertCheckRequest model);

        Task<Result<CertCheckResponse>> CertCheckAsync(CertCheckRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}