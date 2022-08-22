using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Features.Documents.Queries;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.Documents.Queries.GetById;
using EDO_FOMS.Application.Features.Agreements.Queries;
using EDO_FOMS.Application.Features.Documents.Queries.GetDocAgreements;
using System.Collections.Generic;
using EDO_FOMS.Application.Features.Agreements.Commands;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Features.Documents.Commands;

namespace EDO_FOMS.Client.Infrastructure.Managers.Doc.Document
{
    public interface IDocumentManager : IManager
    {
        Task<PaginatedResult<GetDocumentsResponse>> GetDocsAsync(GetPagedDocumentsRequest request);
        Task<PaginatedResult<GetDocumentsResponse>> SearchDocsAsync(SearchDocsRequest request);

        Task<PaginatedResult<EmployeeAgreementsResponse>> GetEmployeeAgreementsAsync(GetPagedAgreementsRequest request);
        Task<PaginatedResult<EmployeeAgreementsResponse>> SearchAgrsAsync(SearchAgrsRequest request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);
        Task<IResult<List<GetDocAgreementsResponse>>> GetDocAgreementsAsync(int id);
        Task<IResult<List<AgreementsProgressResponse>>> GetAgreementsProgressAsync(int docId, int? agrId);

        Task<IResult<List<OrgsResponse>>> GetFoundOrgs(string search);
        Task<IResult<List<ContactResponse>>> GetFoundContacts(SearchContactsRequest request);
        Task<IResult<List<ContactResponse>>> GetAgreementMembersAsync(int orgId, string search);

        Task<IResult<List<RouteTitleModel>>> GetRouteTitlesAsync();

        Task<IResult<int>> PostAsync(AddEditDocumentCommand command);
        Task<IResult<int>> PostDocAsync(AddEditDocCommand command);

        Task<IResult<int>> PostMembersAsync(AddAgreementMembersCommand request);
        Task<IResult<int>> PostAgreementAnswerAsync(AgreementAnswerCommand command);
        Task<IResult<int>> PostAgreementSignedAsync(AgreementSignedCommand command);

        Task<IResult<int>> ChangeStageAsync(ChangeDocStageCommand request);
        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<int>> CheckForImportsAsync();

        Task<string> GetBase64Async(string url);
    }
}