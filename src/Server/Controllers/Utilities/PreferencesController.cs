using System.Threading.Tasks;
using EDO_FOMS.Server.Managers.Preferences;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EDO_FOMS.Server.Controllers.Utilities
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferencesController : ControllerBase
    {
        private readonly ServerPreferenceManager _serverPreferenceManager;

        public PreferencesController(ServerPreferenceManager serverPreferenceManager)
        {
            _serverPreferenceManager = serverPreferenceManager;
        }

        /// <summary>
        /// Change Language Preference
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("changeLanguage")]
        public async Task<IActionResult> ChangeLanguageAsync(string languageCode)
        {
            var result = await _serverPreferenceManager.ChangeLanguageAsync(languageCode);
            return Ok(result);
        }

        //TODO - add actions
    }
}