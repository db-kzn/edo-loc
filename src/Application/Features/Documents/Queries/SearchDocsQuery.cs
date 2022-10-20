using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Application.Specifications.Doc;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries;

public class SearchDocsQuery : IRequest<PaginatedResult<GetDocumentsResponse>>
{
    public SearchDocsRequest Request { get; }
    public SearchDocsQuery(SearchDocsRequest request) { Request = request; }
}

internal class SearchDocsQueryHandler : IRequestHandler<SearchDocsQuery, PaginatedResult<GetDocumentsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SearchDocsQueryHandler(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<GetDocumentsResponse>> Handle(SearchDocsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;
        var docSpec = new DocumentFilterSpecification(request, _currentUserService.UserId);
        var docEntities = _unitOfWork.Repository<Document>().Entities.Specify(docSpec);

        if (request.OrgId != null || request.UserId != null)
        {
            var agrsEntities = _unitOfWork.Repository<Agreement>().Entities;
            var contactAgrs = (request.OrgId == null) ? agrsEntities : agrsEntities.Where(a => a.OrgId == request.OrgId);
            contactAgrs = (request.UserId == null) ? contactAgrs : contactAgrs.Where(a => a.EmplId == request.UserId);
            docEntities = from doc in docEntities join agr in contactAgrs on doc.Id equals agr.DocumentId select doc;
        }

        Expression<Func<Document, GetDocumentsResponse>> expression = e => new GetDocumentsResponse
        {
            Id = e.Id,
            ParentId = e.ParentId,
            PreviousId = e.PreviousId,

            EmplId = e.EmplId,
            EmplOrgId = e.EmplOrgId,

            KeyOrgId = e.KeyOrgId,
            RecipientInn = e.Recipient.Inn,
            RecipientShort = e.Recipient.ShortName,

            RouteId = e.RouteId,
            Stage = e.Stage,
            HasChanges = e.HasChanges,

            TypeId = e.TypeId,
            TypeName = e.Type.Name,
            TypeShort = e.Type.Short,
            Number = e.Number,
            Date = e.Date,

            Title = e.Title,
            Description = e.Description,
            IsPublic = e.IsPublic,
            DepartmentId = e.DepartmentId,

            CurrentStep = e.CurrentStep,
            TotalSteps = e.TotalSteps,
            Version = e.Version,
            //SignStartAt = e.SignStartAt,

            URL = e.URL,
            //StoragePath = e.StoragePath,
            FileName = e.FileName,

            CreatedBy = e.CreatedBy,
            CreatedOn = e.CreatedOn
        };
        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await docEntities.Select(expression).OrderBy(sort)
           .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
