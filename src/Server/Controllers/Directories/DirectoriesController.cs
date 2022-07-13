using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.Directories
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoriesController : BaseApiController<DirectoriesController>
    {
        /// <summary>
        /// Get Companies
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var companies = await _mediator.Send(
                new GetCompaniesQuery(
                    new GetPagedCompaniesRequest(pageSize, pageNumber, searchString, orderBy, matchCase)
                ));
            return Ok(companies);
        }

        /// <summary>
        /// Search Companies
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpPost("search-companies")]
        public async Task<IActionResult> SearchCompanies(SearchCompaniesRequest request)
        {
            var docs = await _mediator.Send(new SearchCompaniesQuery(request));
            return Ok(docs);
        }

        /// <summary>
        /// Check Companies For Imports
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("check-companies-for-imports")]
        public async Task<IActionResult> CheckCompaniesForImports()
        {
            return Ok(await _mediator.Send(new CheckCompaniesForImportsQuery()));
        }

        /// <summary>
        /// Import FOMS Companies
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("import-foms")]
        public async Task<IActionResult> ImportFoms()
        {
            return Ok(await _mediator.Send(new ImportFomsCommand()));
        }

        /// <summary>
        /// Import SMO Companies
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("import-smo")]
        public async Task<IActionResult> ImportSmo()
        {
            return Ok(await _mediator.Send(new ImportSmoCommand()));
        }

        /// <summary>
        /// Import MO Companies
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("import-mo")]
        public async Task<IActionResult> ImportMo()
        {
            return Ok(await _mediator.Send(new ImportMoCommand()));
        }
    }
}
