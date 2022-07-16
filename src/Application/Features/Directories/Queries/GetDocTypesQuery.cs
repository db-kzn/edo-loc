using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Specifications.Dir;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetDocTypesQuery : IRequest<PaginatedResult<DocTypesResponse>>
{
    public GetPagedDocTypesRequest Request { get; set; }
    public GetDocTypesQuery(GetPagedDocTypesRequest request)
    {
        Request = request;
    }
}

internal class GetDocTypesQueryHandler : IRequestHandler<GetDocTypesQuery, PaginatedResult<DocTypesResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetDocTypesQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<DocTypesResponse>> Handle(GetDocTypesQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        Expression<Func<DocumentType, DocTypesResponse>> expression = e => new DocTypesResponse
        {
            Id = e.Id,
            IsActive = e.IsActive,
            Icon = e.Icon,
            Color = e.Color,

            Short = e.Short,
            Label = e.Label,
            Name = e.Name,

            NameEn = e.NameEn,
            Description = e.Description
        };

        var companySpec = new DocTypesFilterSpecification(request.SearchString, request.MatchCase);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await _unitOfWork.Repository<DocumentType>().Entities
           .Specify(companySpec)
           .Select(expression)
           .OrderBy(sort)
           .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
