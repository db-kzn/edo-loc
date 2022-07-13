using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.v1.Orgs
{
    public class OrgsController : BaseApiController<OrgsController>
    {
        ///// <summary>
        ///// Get All Organizations
        ///// </summary>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var orgs = await _mediator.Send(new GetAllOrgsQuery());
        //    return Ok(orgs);
        //}

        /// <summary>
        /// Get a Organization By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.SelfOrg.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var org = await _mediator.Send(new GetOrgByIdQuery() { Id = id });
            return Ok(org);
        }

        /// <summary>
        /// Create/Update a Organization
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.SelfOrg.Edit)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditOrgCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        ///// <summary>
        ///// Delete a Organization
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.Delete)]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    return Ok(await _mediator.Send(new DeleteOrgCommand { Id = id }));
        //}

        ///// <summary>
        ///// Search Organizations and Export to Excel
        ///// </summary>
        ///// <param name="searchString"></param>
        ///// <returns></returns>
        //[Authorize(Policy = Permissions.Organizations.Export)]
        //[HttpGet("export")]
        //public async Task<IActionResult> Export(string searchString = "")
        //{
        //    return Ok(await _mediator.Send(new ExportOrganizationsQuery(searchString)));
        //}
    }
}
