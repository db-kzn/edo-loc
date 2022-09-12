using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Features.Documents.Queries;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.Documents.Queries.GetById;
using EDO_FOMS.Application.Features.Documents.Queries.GetDocAgreements;
using System.Collections.Generic;
using EDO_FOMS.Application.Features.Agreements.Queries;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Application.Features.Agreements.Commands;
using EDO_FOMS.Application.Responses.Docums;
using System;
using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Features.Documents.Commands;
using EDO_FOMS.Application.Features.Directories.Queries;

namespace EDO_FOMS.Client.Infrastructure.Managers.Doc.Document
{
    public class DocumentManager : IDocumentManager
    {
        private readonly HttpClient _httpClient;

        public DocumentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DocumentsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<PaginatedResult<GetDocumentsResponse>> GetDocsAsync(GetPagedDocumentsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetDocsPaged(request));
            return await response.ToPaginatedResult<GetDocumentsResponse>();
        }

        public async Task<IResult<DocCardResponse>> GetDocCardAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetDocCard(id));
            return await response.ToResult<DocCardResponse>();
        }

        public async Task<PaginatedResult<GetDocumentsResponse>> SearchDocsAsync(SearchDocsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.SearchDocsPaged, request);
            return await response.ToPaginatedResult<GetDocumentsResponse>();
        }

        public async Task<PaginatedResult<EmployeeAgreementsResponse>> GetEmployeeAgreementsAsync(GetPagedAgreementsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetAgrsPaged(request));
            return await response.ToPaginatedResult<EmployeeAgreementsResponse>();
        }

        public async Task<PaginatedResult<EmployeeAgreementsResponse>> SearchAgrsAsync(SearchAgrsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.SearchAgrsPaged, request);
            return await response.ToPaginatedResult<EmployeeAgreementsResponse>();
        }

        public async Task<IResult<List<GetDocAgreementsResponse>>> GetDocAgreementsAsync(int docId)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetDocAgreements(docId));
            return await response.ToResult<List<GetDocAgreementsResponse>>();
        }

        public async Task<IResult<List<AgreementsProgressResponse>>> GetAgreementsProgressAsync(int docId, int? agrId)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetAgreementsProgress(docId, agrId));
            return await response.ToResult<List<AgreementsProgressResponse>>();
        }

        public async Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetById(request.Id));
            return await response.ToResult<GetDocumentByIdResponse>();
        }

        public async Task<IResult<int>> PostAsync(AddEditDocumentCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.AddEdit, command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> PostDocAsync(AddEditDocCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.AddEditDoc, command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> PostAgreementAnswerAsync(AgreementAnswerCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.PostAgreementAnswer, command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> PostMembersAsync(AddAgreementMembersCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.AddMembers, command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<OrgsResponse>>> GetFoundOrgs(string search)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetFoundOrgs(search));
            return await response.ToResult<List<OrgsResponse>>();
        }

        public async Task<IResult<List<OrgsResponse>>> FindOrgsWithType(OrgTypes orgType, string search)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.FindOrgsWithType(orgType, search));
            return await response.ToResult<List<OrgsResponse>>();
        }

        public async Task<IResult<List<ContactResponse>>> GetFoundContacts(SearchContactsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetFoundContacts(request));

            return await response.ToResult<List<ContactResponse>>();
        }

        public async Task<IResult<List<ContactResponse>>> GetAgreementMembersAsync(int orgId, string search)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetAgreementMembers(orgId, search));
            return await response.ToResult<List<ContactResponse>>();
        }

        public async Task<IResult<List<ActiveRouteModel>>> GetActiveRoutesAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetRouteTitles);
            return await response.ToResult<List<ActiveRouteModel>>();
        }
        public async Task<IResult<List<ActiveRouteModel>>> CheckForImportsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetImportsCount);
            return await response.ToResult<List<ActiveRouteModel>>();
        }
        public async Task<IResult<List<string>>> GetImportFilesAsync(int routeId)
        {
            var response = await _httpClient.GetAsync(Routes.DocumentsEndpoints.GetImportFiles(routeId));
            return await response.ToResult<List<string>>();
        }

        public async Task<IResult<int>> ChangeStageAsync(ChangeDocStageCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.ChangeStage, request);
            return await response.ToResult<int>();
        }

        public async Task<string> GetBase64Async(string url)
        {
            var bytes = await _httpClient.GetByteArrayAsync(url);
            return Convert.ToBase64String(bytes);
        }

        public async Task<IResult<int>> PostAgreementSignedAsync(AgreementSignedCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentsEndpoints.PostAgreementSigned, command);
            return await response.ToResult<int>();
        }
    }
}