using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services.Identity
{
    public interface IUserService : IService
    {
        Task<Result<List<UserResponse>>> GetAllAsync();
        Task<PaginatedResult<UserResponse>> GetPagedUsersAsync(GetPagedUsersRequest request);
        Task<PaginatedResult<UserResponse>> SearchUsersAsync(SearchUsersRequest request);

        Task<Result<List<UserResponse>>> GetAllByOrgIdAsync(int orgId);

        Task<Result<List<ContactResponse>>> GetFoundContacts(OrgTypes orgType, UserBaseRoles baseRole, string searchString);

        Task<Result<List<ContactResponse>>> GetAgreementMembersAsync(int orgId, string search);

        Task<Result<int>> UpdateUsersOrgType(UpdateUsersOrgTypeRequest request, string origin);

        Task<int> GetCountAsync();

        Task<IResult<bool>> GetUserOrgExists(string inn);

        Task<Employee> GetEmployeeAsync(string userId);

        Task<IResult<Organization>> GerUserOrgAsync(string inn);

        Task<IResult<UserResponse>> GetAsync(string userId);

        //Task<IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult> RegisterByCertAsync(RegisterByCertRequest request, string origin, IMediator mediator);

        Task<IResult> NewUserAsync(NewUserRequest request, string origin);

        Task<IResult> EditUserAsync(EditUserRequest request, string origin);

        Task<IResult> AddEditEmployeeAsync(AddEditEmployeeRequest request, string origin);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");

        Task<IResult> SendMailAsync(MailModel mail);

        Task MailToUserAsync(MailToUser mail);

        Task MailsToUsersAsync(List<MailToUser> mails);
    }
}