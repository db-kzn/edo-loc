using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.System
{
    public interface IAdminManager : IManager
    {
        //Task<IResult<List<OrgsResponse>>> GetAllOrgsAsync();
        Task<PaginatedResult<OrgsResponse>> GetPagedOrgsAsync(GetPagedOrgsRequest request);
        Task<PaginatedResult<OrgsResponse>> SearchOrgsAsync(SearchOrgsRequest request);

        //Task<IResult<List<UserResponse>>> GetAllUsersAsync();
        Task<PaginatedResult<UserResponse>> GetPagedUsersAsync(GetPagedUsersRequest request);
        Task<PaginatedResult<UserResponse>> SearchUsersAsync(SearchUsersRequest request);

        //Task<IResult<List<CertsResponse>>> GetAllCertsAsync();
        Task<PaginatedResult<CertsResponse>> GetPagedCertsAsync(GetPagedCertsRequest request);
        Task<PaginatedResult<CertsResponse>> SearchCertsAsync(SearchCertsRequest request);

        //Task<IResult<int>> AddEditUserAsync(AddEditUserCommand request);
        Task<IResult<int>> AddEditOrgAsync(AddEditOrgCommand request);
        
        Task<IResult> AddUserAsync(NewUserRequest request);
        Task<IResult> EditUserAsync(EditUserRequest request);
        Task<IResult> UpdateUsersOrgTypeAsync(UpdateUsersOrgTypeRequest typeIx);

        Task<IResult<List<GetUserCertsResponse>>> GetUserCertsAsync(string userId);
        Task<IResult<int>> AddEditCertAsync(AddEditCertCommand request);
        Task<IResult<int>> DeleteCertAsync(int certId);

        Task<IResult<HomeConfiguration>> GetHomeParamsAsync();
        Task<IResult<bool>> SaveHomeParamsAsync(HomeConfiguration home);
        Task<IResult<MailConfiguration>> GetMailParamsAsync();
        Task<IResult<bool>> SaveMailParamsAsync(MailConfiguration mail);
        Task<IResult<bool>> PostMailAsync(MailModel mail);
    }
}
