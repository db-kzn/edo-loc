using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Features.System.Command;
using EDO_FOMS.Application.Features.System.Queries;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.System
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseApiController<AdminController>
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        ///// <summary>
        ///// Get All Organizations
        ///// </summary>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet("orgs")]
        //public async Task<IActionResult> GetAllOrgs()
        //{
        //    var orgs = await _mediator.Send(new GetAllOrgsQuery());
        //    return Ok(orgs);
        //}

        /// <summary>
        /// Get Paged Organizations
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("orgs")]
        public async Task<IActionResult> GetPagedOrgs(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var orgs = await _mediator.Send(new GetPagedOrgsQuery(new GetPagedOrgsRequest(pageSize, pageNumber, searchString, orderBy, matchCase)));
            return Ok(orgs);
        }

        /// <summary>
        /// Search Organizations
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpPost("orgs/search")]
        public async Task<IActionResult> Search(SearchOrgsRequest request)
        {
            var orgs = await _mediator.Send(new SearchOrgsQuery(request));
            return Ok(orgs);
        }

        ///// <summary>
        ///// Get All Users
        ///// </summary>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet("users")]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var users = await _userService.GetAllAsync();
        //    return Ok(users);
        //}

        /// <summary>
        /// Get Paged Users
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("users")]
        public async Task<IActionResult> GetPagedUsers(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var users = await _userService.GetPagedUsersAsync(new GetPagedUsersRequest(pageSize, pageNumber, searchString, orderBy, matchCase));
            return Ok(users);
        }

        /// <summary>
        /// Search Users
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpPost("users/search")]
        public async Task<IActionResult> Search(SearchUsersRequest request)
        {
            var users = await _userService.SearchUsersAsync(request);
            return Ok(users);
        }

        ///// <summary>
        ///// Get All Certificates
        ///// </summary>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet("certs")]
        //public async Task<IActionResult> GetAllCerts()
        //{
        //    var certs = await _mediator.Send(new GetAllCertsQuery());
        //    return Ok(certs);
        //}

        /// <summary>
        /// Get Paged Certs
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="matchCase"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("certs")]
        public async Task<IActionResult> GetPagedCerts(int pageNumber, int pageSize, string searchString, bool matchCase = false, string orderBy = "")
        {
            var orgs = await _mediator.Send(new GetPagedCertsQuery(new GetPagedCertsRequest(pageSize, pageNumber, searchString, orderBy, matchCase)));
            return Ok(orgs);
        }

        /// <summary>
        /// Search Certs
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpPost("certs/search")]
        public async Task<IActionResult> SearchCerts(SearchCertsRequest request)
        {
            var orgs = await _mediator.Send(new SearchCertsQuery(request));
            return Ok(orgs);
        }

        /// <summary>
        /// Get User Certificates
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("user-certs/{userId}")]
        public async Task<IActionResult> GetUserCerts(string userId)
        {
            var certs = await _mediator.Send(new GetUserCertsQuery(userId));
            return Ok(certs);
        }

        /// <summary>
        /// Get Org Card
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("org-card")]
        public async Task<IActionResult> GetOrgCard(int orgId)
        {
            return Ok(await _mediator.Send(new GetOrgCardQuery(orgId)));
        }

        /// <summary>
        /// Create/Update a Organization
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("org")]
        public async Task<IActionResult> Post(AddEditOrgCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Get Home Configuration
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("home-params")]
        public async Task<IActionResult> GetHomeParams()
        {
            return Ok(await _mediator.Send(new GetHomeParamsQuery()));
        }

        /// <summary>
        /// Save Home Configuration
        /// </summary>
        /// <param name="home"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("home-params")]
        public async Task<IActionResult> SaveMailParams(HomeConfiguration home)
        {
            return Ok(await _mediator.Send(new SaveHomeParamsCommand(home)));
        }

        /// <summary>
        /// Get Mail Configuration
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.View)]
        [HttpGet("mail-params")]
        public async Task<IActionResult> GetMailParams()
        {
            return Ok(await _mediator.Send(new GetMailParamsQuery()));
        }

        /// <summary>
        /// Save Mail Configuration
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("mail-params")]
        public async Task<IActionResult> SaveMailParams(MailConfiguration mail)
        {
            return Ok(await _mediator.Send(new SaveMailParamsCommand(mail)));
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("send-mail")]
        public async Task<IActionResult> Post(MailModel mail)
        {
            return Ok(await _userService.SendMailAsync(mail));
        }

        /// <summary>
        /// Adding a new user by the administrator
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Create)]
        [HttpPost("user/add")]
        public async Task<IActionResult> NewUserAsync(NewUserRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.NewUserAsync(request, origin));
        }

        /// <summary>
        /// Updating a user by the administrator
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Create)]
        [HttpPost("user/edit")]
        public async Task<IActionResult> EditUserAsync(EditUserRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.EditUserAsync(request, origin));
        }

        /// <summary>
        /// Update Users OrgType
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpPost("users/org-type")]
        public async Task<IActionResult> UpdateUsersOrgType(UpdateUsersOrgTypeRequest request)
        {
            var origin = Request.Headers["origin"];
            var users = await _userService.UpdateUsersOrgType(request, origin);
            return Ok(users);
        }

        /// <summary>
        /// Create/Update a Certificate
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Create)]
        [HttpPost("cert")]
        public async Task<IActionResult> Post(AddEditCertCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete Certificate
        /// </summary>
        /// <param name="certId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.System.Edit)]
        [HttpDelete("cert/{certId}")]
        public async Task<IActionResult> DeleteCert(int certId)
        {
            return Ok(await _mediator.Send(new DeleteCertCommand(certId)));
        }
    }
}
