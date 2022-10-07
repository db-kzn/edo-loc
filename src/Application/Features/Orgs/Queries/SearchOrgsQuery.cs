using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Application.Specifications.Org;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class SearchOrgsQuery : IRequest<PaginatedResult<OrgsResponse>>
{
    public SearchOrgsRequest Request { get; }

    public SearchOrgsQuery(SearchOrgsRequest request)
    {
        Request = request;
    }
}

internal class SearchOrgsQueryHandler : IRequestHandler<SearchOrgsQuery, PaginatedResult<OrgsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    //private readonly ICurrentUserService _currentUserService;

    public SearchOrgsQueryHandler(
        IUnitOfWork<int> unitOfWork
        //ICurrentUserService currentUserService
        )
    {
        _unitOfWork = unitOfWork;
        //_currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<OrgsResponse>> Handle(SearchOrgsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        Expression<Func<Organization, OrgsResponse>> expression = e => new OrgsResponse
        {
            Id = e.Id,
            Inn = e.Inn,
            Code = e.OmsCode,

            Name = e.Name,
            ShortName = e.ShortName,

            IsPublic = e.IsPublic,
            Type = e.Type,
            State = e.State,

            Email = e.Email,
            Phone = e.Phone,
            CreatedOn = e.CreatedOn
        };

        var orgSpec = new OrgSpecification(request);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await _unitOfWork.Repository<Organization>().Entities
            .Specify(orgSpec)
            .Select(expression)
            .OrderBy(sort)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
