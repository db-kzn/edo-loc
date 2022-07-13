using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Shared.Constants.Permission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace EDO_FOMS.Server.Controllers.Identity
{
    [Authorize]
    [Route("api/identity/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Checking the existence of the user's organization
        /// </summary>
        /// <param name="inn"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Users.View)]
        [AllowAnonymous]
        [HttpGet("org-exists/{inn}")]
        public async Task<IActionResult> GetUserOrgExists(string inn)
        {
            var userOrgExists = await _userService.GetUserOrgExists(inn);
            return Ok(userOrgExists);
        }

        /// <summary>
        /// Register a User by Certificate
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpPost("cert")]
        public async Task<IActionResult> RegisterByCertAsync(RegisterByCertRequest request)
        {
            var origin = Request.Headers["origin"];
            var mediator = HttpContext.RequestServices.GetService<IMediator>();
            return Ok(await _userService.RegisterByCertAsync(request, origin, mediator));
        }

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Users.View)]
        //[AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Get a Organization By Inn
        /// </summary>
        /// <param name="inn"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize]
        [HttpGet("org/{inn}")]
        public async Task<IActionResult> GetByInn(string inn)
        {
            var org = await _userService.GerUserOrgAsync(inn);
            return Ok(org);
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]//(Policy = Permissions.Users.View)
        [HttpGet("orgId:{orgId}")]
        public async Task<IActionResult> GetAllByOrgId(int orgId)
        {
            var users = await _userService.GetAllByOrgIdAsync(orgId);
            return Ok(users);
        }

        /// <summary>
        /// Adding or Updating a employee by the chief
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.SelfOrg.Edit)]
        [HttpPost("employee")]
        public async Task<IActionResult> AddEditEmployeeAsync(AddEditEmployeeRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.AddEditEmployeeAsync(request, origin));
        }


        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            return Ok(await _userService.ConfirmEmailAsync(userId, code));
        }

        ///// <summary>
        ///// Get User Details
        ///// </summary>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _userService.GetAllAsync();
        //    return Ok(users);
        //}

        ///// <summary>
        ///// Get User Roles By Id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.View)]
        //[HttpGet("roles/{id}")]
        //public async Task<IActionResult> GetRolesAsync(string id)
        //{
        //    var userRoles = await _userService.GetRolesAsync(id);
        //    return Ok(userRoles);
        //}

        ///// <summary>
        ///// Update Roles for User
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.Edit)]
        //[HttpPut("roles/{id}")]
        //public async Task<IActionResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        //{
        //    return Ok(await _userService.UpdateRolesAsync(request));
        //}

        ///// <summary>
        ///// Register a User
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        //{
        //    var origin = Request.Headers["origin"];
        //    return Ok(await _userService.RegisterAsync(request, origin));
        //}

        ///// <summary>
        ///// Adding a new user by the administrator
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.Create)]
        //[HttpPost("new")]
        //public async Task<IActionResult> NewUserAsync(NewUserRequest request)
        //{
        //    var origin = Request.Headers["origin"];
        //    return Ok(await _userService.NewUserAsync(request, origin));
        //}

        ///// <summary>
        ///// Updating a user by the administrator
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.Create)]
        //[HttpPost("edit")]
        //public async Task<IActionResult> EditUserAsync(EditUserRequest request)
        //{
        //    var origin = Request.Headers["origin"];
        //    return Ok(await _userService.EditUserAsync(request, origin));
        //}

        ///// <summary>
        ///// Toggle User Status (Activate and Deactivate)
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[HttpPost("toggle-status")]
        //public async Task<IActionResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        //{
        //    return Ok(await _userService.ToggleUserStatusAsync(request));
        //}

        ///// <summary>
        ///// Forgot Password
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[HttpPost("forgot-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        //{
        //    var origin = Request.Headers["origin"];
        //    return Ok(await _userService.ForgotPasswordAsync(request, origin));
        //}

        ///// <summary>
        ///// Reset Password
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns>Status 200 OK</returns>
        //[HttpPost("reset-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        //{
        //    return Ok(await _userService.ResetPasswordAsync(request));
        //}

        ///// <summary>
        ///// Export to Excel
        ///// </summary>
        ///// <param name="searchString"></param>
        ///// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.System.Export)]
        //[HttpGet("export")]
        //public async Task<IActionResult> Export(string searchString = "")
        //{
        //    var data = await _userService.ExportToExcelAsync(searchString);
        //    return Ok(data);
        //}
    }
}