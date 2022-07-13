using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Identity.Users
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient _httpClient;

        public UserManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<bool>> GetUserOrgExists(string inn)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetUserOrgExists(inn));
            return await response.ToResult<bool>();
        }

        public async Task<IResult<List<UserResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetAll);
            return await response.ToResult<List<UserResponse>>();
        }

        public async Task<IResult<List<UserResponse>>> GetAllByOrgIdAsync(int orgId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetAllByOrgId(orgId));
            return await response.ToResult<List<UserResponse>>();
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.Get(userId));
            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult> RegisterUserAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.Register, request);
            return await response.ToResult();
        }

        public async Task<IResult> RegisterByCertAsync(RegisterByCertRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.RegisterByCert, request);
            return await response.ToResult();
        }

        //public async Task<IResult> NewUserAsync(NewUserRequest request)
        //{
        //    var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.NewUser, request);
        //    return await response.ToResult();
        //}

        //public async Task<IResult> EditUserAsync(EditUserRequest request)
        //{
        //    var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.EditUser, request);
        //    return await response.ToResult();
        //}

        public async Task<IResult> AddEditEmployeeAsync(AddEditEmployeeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.AddEditEmployee, request);
            return await response.ToResult();
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.ToggleUserStatus, request);
            return await response.ToResult();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetUserRoles(userId));
            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.UserEndpoints.GetUserRoles(request.UserId), request);
            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.ForgotPassword, model);
            return await response.ToResult();
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.ResetPassword, request);
            return await response.ToResult();
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.UserEndpoints.Export
                : Routes.UserEndpoints.ExportFiltered(searchString));
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}