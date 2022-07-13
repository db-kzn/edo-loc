using EDO_FOMS.Application.Features.System.Command;
using EDO_FOMS.Application.Features.System.Queries;
using EDO_FOMS.Domain.Entities.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : BaseApiController<StateController>
    {
        /// <summary>
        /// Navigation Counts
        /// <param name="userId"></param>
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]//(Policy = Permissions.Documents.View)
        [HttpGet("nav-counts/{userId}")]
        public async Task<IActionResult> GetNavCounts(string userId)
        {
            return Ok(await _mediator.Send(new NavCountsQuery(userId)));
        }

        /// <summary>
        /// Notices Subscribe
        /// <param name="userId"></param>
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]//(Policy = Permissions.Documents.View)
        [HttpGet("subscribe/{userId}")]
        public async Task<IActionResult> GetSubscribe(string userId)
        {
            return Ok(await _mediator.Send(new SubscribeQuery(userId)));
        }

        /// <summary>
        /// Notices Subscribe
        /// <param name="command"></param>
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]//(Policy = Permissions.Documents.View)
        [HttpPost("subscribe")]
        public async Task<IActionResult> PostSubscribe(Subscribe command)
        {
            return Ok(await _mediator.Send(new SubscribeCommand(command)));
        }
    }
}
