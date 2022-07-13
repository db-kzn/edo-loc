using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.Identity.Users
{
    public interface IUserManager : IManager
    {
        Task<IResult<bool>> GetUserOrgExists(string inn);
        //Task<bool> GetUserOrgExists(string inn);

        Task<IResult<List<UserResponse>>> GetAllAsync();

        Task<IResult<List<UserResponse>>> GetAllByOrgIdAsync(int orgId);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

        Task<IResult> RegisterUserAsync(RegisterRequest request);

        Task<IResult> RegisterByCertAsync(RegisterByCertRequest request);

        //Task<IResult> NewUserAsync(NewUserRequest request);

        //Task<IResult> EditUserAsync(EditUserRequest request);

        Task<IResult> AddEditEmployeeAsync(AddEditEmployeeRequest request);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}