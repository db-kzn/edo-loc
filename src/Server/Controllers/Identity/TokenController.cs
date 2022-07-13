using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _identityService;

        public TokenController(ITokenService identityService, ICurrentUserService currentUserService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<ActionResult> Get(TokenRequest model)
        {
            var response = await _identityService.LoginAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            var response = await _identityService.GetRefreshTokenAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Cert SignIn
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("cert")]
        public async Task<ActionResult> Cert(CertCheckRequest model)
        {
            var response = await _identityService.SignInAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Check Cert
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("cert-check")]
        public async Task<ActionResult> CertCheck(CertCheckRequest model)
        {
            var response = await _identityService.CertCheckAsync(model);
            return Ok(response);
        }
    }
}