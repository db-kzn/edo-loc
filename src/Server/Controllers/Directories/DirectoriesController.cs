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
        /// Get DocTypes
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("doc-types")]
        public async Task<IActionResult> GetDocTypes(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var companies = await _mediator.Send(
                new GetDocTypesQuery(
                    new GetPagedDocTypesRequest(pageSize, pageNumber, searchString, orderBy, matchCase)
                ));
            return Ok(companies);
        }

        /// <summary>
        /// Search DocTypes
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpPost("search-doc-types")]
        public async Task<IActionResult> SearchDocTypes(SearchDocTypesRequest request)
        {
            var docs = await _mediator.Send(new SearchDocTypesQuery(request));
            return Ok(docs);
        }

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
        /// Get Routes
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("routes")]
        public async Task<IActionResult> GetRoutes(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var routes = await _mediator.Send(
                new GetRoutesQuery(new GetPagedRoutesRequest(pageSize, pageNumber, searchString, orderBy, matchCase)));
            return Ok(routes);
        }

        /// <summary>
        /// Get Route Card
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("route-card")]
        public async Task<IActionResult> GetRouteCard(int id)
        {
            var routeCard = await _mediator.Send(new GetRouteCardQuery(id));
            return Ok(routeCard);
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

        /// <summary>
        /// Add/Edit Route
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("route")]
        public async Task<IActionResult> Post(AddEditRouteCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
