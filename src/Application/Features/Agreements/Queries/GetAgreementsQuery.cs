using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Specifications.Doc;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Queries;

public class GetAgreementsQuery : IRequest<PaginatedResult<GetAgreementsResponse>>
{
    public GetPagedAgreementsRequest Request { get; }
    public GetAgreementsQuery(GetPagedAgreementsRequest request)
    {
        Request = request;
    }
}

internal class GetDocumentsQueryHandler : IRequestHandler<GetAgreementsQuery, PaginatedResult<GetAgreementsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetDocumentsQueryHandler(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<GetAgreementsResponse>> Handle(GetAgreementsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;
        var now = DateTime.Now;

        Expression<Func<Agreement, GetAgreementsResponse>> expression = e => new GetAgreementsResponse
        {
            AgreementId = e.Id,
            ParentAgreementId  = e.ParentId,

            EmplId = e.EmplId,
            EmplOrgId = e.OrgId,

            KeyOrgId = e.Document.KeyOrgId,
            RecipientInn = e.Document.Recipient.Inn,
            RecipientShort = e.Document.Recipient.ShortName,

            //Recipients 

            IssuerOrgId = e.Document.EmplOrgId,
            IssuerType = e.Document.Issuer.Type,
            IssuerOrgInn = e.Document.Issuer.Inn,
            IssuerOrgShortName = e.Document.Issuer.ShortName,

            // Документ - предмет согласования
            DocId = e.DocumentId,
            DocParentId = e.Document.ParentId,
            DocRouteId = e.Document.RouteId,
            DocIsPublic = e.Document.IsPublic,

            DocTypeId = e.Document.TypeId,
            DocTypeName = e.Document.Type.Name,
            DocTypeShort = e.Document.Type.Short,

            DocNumber = e.Document.Number,
            DocDate = e.Document.Date,
            DocTitle = e.Document.Title,

            DocDescription = e.Document.Description,
            DocURL = e.Document.URL,
            DocFileName = e.Document.FileName,

            DocStage = e.Document.Stage,
            DocHasChanges = e.Document.HasChanges,
            DocCurrentStep = e.Document.CurrentStep,
            DocTotalSteps = e.Document.TotalSteps,

            DocVersion = e.Document.Version,
            DocCreatedBy = e.Document.CreatedBy,
            DocCreatedOn = e.Document.CreatedOn,

            // Данные о согласовании
            Step = e.StageNumber,
            State = (e.Received == null) ? AgreementStates.Received : e.State,
            Action = e.Action,
            IsCanceled = e.IsCanceled,

            CreatedOn = e.CreatedOn,
            Received = e.Received,
            Opened = e.Opened,
            Answered = e.Answered,

            Remark = e.Remark,
            SignURL = e.SignURL
        };

        var agrSpec = new AgreementSpecification(request.SearchString, _currentUserService.UserId, request.AgrState, request.MatchCase);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "AgreementId Descending";

        var result = await _unitOfWork.Repository<Agreement>().Entities
           .Specify(agrSpec)
           .Select(expression)
           .OrderBy(sort)
           //.GroupBy(a => a.DocId).Select(g => g.First())
           .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        var ids = result.Data.Where(a => a.Received == null).Select(a => a.AgreementId).ToArray();

        if (ids.Length > 0)
        {
            var agrs = _unitOfWork.Repository<Agreement>().Entities.Where(a => ids.Contains(a.Id)).ToList();

            agrs.ForEach(async a => {
                a.Received = now;
                a.State = AgreementStates.Received;
                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
            });

            await _unitOfWork.Commit(cancellationToken);
        }

        return result;
    }
}
