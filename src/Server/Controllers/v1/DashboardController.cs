using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.Dashboards.Queries.GetData;
using EDO_FOMS.Shared.Constants.Permission;

namespace EDO_FOMS.Server.Controllers.v1
{
    //[ApiController]
    //public class DashboardController : BaseApiController<DashboardController>
    //{
    //    /// <summary>
    //    /// Get Dashboard Data
    //    /// </summary>
    //    /// <returns>Status 200 OK </returns>
    //    [Authorize(Policy = Permissions.Documents.View)]
    //    [HttpGet]
    //    public async Task<IActionResult> GetDataAsync()
    //    {
    //        var result = await _mediator.Send(new GetDashboardDataQuery());
    //        return Ok(result);
    //    }
    //}
}