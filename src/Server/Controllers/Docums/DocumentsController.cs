using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Features.Documents.Commands.Delete;
using EDO_FOMS.Application.Features.Documents.Queries;
using EDO_FOMS.Application.Features.Documents.Queries.GetById;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Application.Features.Documents.Queries.GetDocAgreements;
using EDO_FOMS.Application.Features.Agreements.Queries;
using EDO_FOMS.Application.Features.Agreements.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Features.Orgs.Queries;
using System.Collections.Generic;
using EDO_FOMS.Application.Features.Directories.Queries;
using EDO_FOMS.Application.Features.Documents.Commands;

namespace EDO_FOMS.Server.Controllers.Docums
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : BaseApiController<DocumentsController>
    {
        private readonly IUserService _userService;

        public DocumentsController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get Documents
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="docStage"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet]//("all")
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, DocStages docStage = DocStages.Undefined, bool matchCase = false, string orderBy = "")
        {
            var docs = await _mediator.Send(
                new GetDocumentsQuery(
                    new GetPagedDocumentsRequest(pageSize, pageNumber, searchString, orderBy, docStage, matchCase)
                ));
            return Ok(docs);
        }

        /// <summary>
        /// Get Agreements
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="agrState"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("agrs")]
        public async Task<IActionResult> GetArgs(int pageNumber, int pageSize, string searchString, AgreementStates agrState = AgreementStates.Undefined, bool matchCase = false, string orderBy = "")
        {
            var docs = await _mediator.Send(
                new GetAgreementsQuery(
                    new GetPagedAgreementsRequest(pageSize, pageNumber, searchString, orderBy, agrState, matchCase)
                ));
            return Ok(docs);
        }

        /// <summary>
        /// Get Imports Count
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("imports-count")]
        public async Task<IActionResult> GetImportsCount()
        {
            return Ok(await _mediator.Send(new CheckForImportsQuery()));
        }

        /// <summary>
        /// Get Route Titles
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpGet("route-titles")]
        public async Task<IActionResult> GetRouteTitles()
        {
            return Ok(await _mediator.Send(new GetRouteTitlesQuery()));
        }

        /// <summary>
        /// Search Documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpPost("search")]
        public async Task<IActionResult> Search(SearchDocsRequest request)
        {
            var docs = await _mediator.Send(new SearchDocsQuery(request));
            return Ok(docs);
        }

        /// <summary>
        /// Search Agreements
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpPost("agreements-search")]
        public async Task<IActionResult> AgrsSearch(SearchAgrsRequest request)
        {
            var agrs = await _mediator.Send(new SearchAgrsQuery(request));
            return Ok(agrs);
        }

        /// <summary>
        /// Search Organizations By String
        /// </summary>
        /// <param name="search"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("orgs-search")]
        public async Task<IActionResult> SearchOrgsByString(string search)
        {
            var orgs = await _mediator.Send(new SearchOrgsByStringQuery(search));
            return Ok(orgs);
        }

        /// <summary>
        /// Search Organizations By String With Org Type
        /// </summary>
        /// <param name="orgType"></param> 
        /// <param name="search"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("orgs-find")]
        public async Task<IActionResult> FindOrgsWithType(OrgTypes orgType, string search)
        {
            var orgs = await _mediator.Send(new SearchOrgsWithTypeQuery(orgType, search));
            return Ok(orgs);
        }

        /// <summary>
        /// Get Found Users
        /// </summary>
        /// <param name="orgType"></param>
        /// <param name="baseRole"></param>
        /// <param name="search"></param>
        /// <param name="take"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("contacts")]
        public async Task<IActionResult> GetFoundContacts(OrgTypes orgType, UserBaseRoles baseRole, string search, int take)
        {
            if (take > 10) { take = 10; }
            var contacts = await _userService.GetFoundContacts(orgType, baseRole, search, take);
            return Ok(contacts);
        }

        /// <summary>
        /// Get Found Users
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="search"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("members")]
        public async Task<IActionResult> GetAgreementMembers(int orgId, string search = "")
        {
            var members = await _userService.GetAgreementMembersAsync(orgId, search);
            return Ok(members);
        }

        /// <summary>
        /// Get Found Document Agreements
        /// </summary>
        /// <param name="docId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("agreements")]
        public async Task<IActionResult> GetDocAgreements(int docId)
        {
            var agreements = await _mediator.Send(new GetDocAgreementsQuery(docId));
            return Ok(agreements);
        }

        /// <summary>
        /// Get Document Agreements Progress
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="agrId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("progress")]
        public async Task<IActionResult> GetAgreementsProgress(int docId, int? agrId)
        {
            var agreements = await _mediator.Send(new AgreementsProgressQuery(docId, agrId));
            return Ok(agreements);
        }

        /// <summary>
        /// Get Found User Agreements
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("user-agreements")]
        public async Task<IActionResult> GetEmployeeAgreements(AgreementStates state)
        {
            var userAgreements = await _mediator.Send(new EmployeeAgreementsQuery(state));
            return Ok(userAgreements);
        }

        /// <summary>
        /// Get Document By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
            return Ok(document);
        }

        /// <summary>
        /// Add/Edit Document
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDocumentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Add/Edit Doc
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpPost]
        public async Task<IActionResult> PostDoc(AddEditDocCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Change Document Stage
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Edit)]
        [HttpPost("stage")]
        public async Task<IActionResult> Post(ChangeDocStageCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Add Agreement Members
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Edit)]
        [HttpPost("members")]
        public async Task<IActionResult> PostMembers(AddAgreementMembersCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Agreement Answer
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Approving)]
        [HttpPost("agreement-answer")]
        public async Task<IActionResult> PostAgreementAnswer(AgreementAnswerCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Agreement Signed
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Approving)]
        [HttpPost("agreement-signed")]
        public async Task<IActionResult> PostAgreementSigned(AgreementSignedCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteDocumentCommand { Id = id }));
        }
    }
}