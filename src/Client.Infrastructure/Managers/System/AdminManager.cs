using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Features.Certs.Commands;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.System
{
    public class AdminManager : IAdminManager
    {
        private readonly HttpClient _httpClient;

        public AdminManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<IResult<List<OrgsResponse>>> GetAllOrgsAsync()
        //{
        //    var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetAllOrgs);
        //    return await response.ToResult<List<OrgsResponse>>();
        //}

        public async Task<PaginatedResult<OrgsResponse>> GetPagedOrgsAsync(GetPagedOrgsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetPagedOrgs(request));
            return await response.ToPaginatedResult<OrgsResponse>();
        }

        public async Task<PaginatedResult<OrgsResponse>> SearchOrgsAsync(SearchOrgsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SearchOrgs, request);
            return await response.ToPaginatedResult<OrgsResponse>();
        }

        //public async Task<IResult<List<UserResponse>>> GetAllUsersAsync()
        //{
        //    var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetAllUsers);
        //    return await response.ToResult<List<UserResponse>>();
        //}

        public async Task<PaginatedResult<UserResponse>> GetPagedUsersAsync(GetPagedUsersRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetPagedUsers(request));
            return await response.ToPaginatedResult<UserResponse>();
        }

        public async Task<PaginatedResult<UserResponse>> SearchUsersAsync(SearchUsersRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SearchUsers, request);
            return await response.ToPaginatedResult<UserResponse>();
        }

        //public async Task<IResult<List<CertsResponse>>> GetAllCertsAsync()
        //{
        //    var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetAllCerts);
        //    return await response.ToResult<List<CertsResponse>>();
        //}

        public async Task<PaginatedResult<CertsResponse>> GetPagedCertsAsync(GetPagedCertsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetPagedCerts(request));
            return await response.ToPaginatedResult<CertsResponse>();
        }

        public async Task<PaginatedResult<CertsResponse>> SearchCertsAsync(SearchCertsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SearchCerts, request);
            return await response.ToPaginatedResult<CertsResponse>();
        }

        public async Task<IResult<int>> AddEditCertAsync(AddEditCertCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.AddEditCert, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> AddEditOrgAsync(AddEditOrgCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.AddEditOrg, request);
            return await response.ToResult<int>();
        }

        public async Task<IResult> AddUserAsync(NewUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.AddUser, request);
            return await response.ToResult();
        }

        public async Task<IResult> EditUserAsync(EditUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.EditUser, request);
            return await response.ToResult();
        }

        public async Task<IResult> UpdateUsersOrgTypeAsync(UpdateUsersOrgTypeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.UpdateUsersOrgType, request);
            return await response.ToResult();
        }

        public async Task<IResult<List<GetUserCertsResponse>>> GetUserCertsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetUserCerts(userId));
            return await response.ToResult<List<GetUserCertsResponse>>();
        }

        public async Task<IResult<int>> DeleteCertAsync(int certId)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.AdminEndpoints.DeleteCert}/{certId}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<HomeConfiguration>> GetHomeParamsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetHomeParams);
            return await response.ToResult<HomeConfiguration>();
        }
        public async Task<IResult<bool>> SaveHomeParamsAsync(HomeConfiguration home)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SaveHomeParams, home);
            return await response.ToResult<bool>();
        }
        public async Task<IResult<MailConfiguration>> GetMailParamsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.AdminEndpoints.GetMailParams);
            return await response.ToResult<MailConfiguration>();
        }
        public async Task<IResult<bool>> SaveMailParamsAsync(MailConfiguration mail)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SaveMailParams, mail);
            return await response.ToResult<bool>();
        }
        public async Task<IResult<bool>> PostMailAsync(MailModel mail)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AdminEndpoints.SendMail, mail);
            return await response.ToResult<bool>();
        }
    }
}
