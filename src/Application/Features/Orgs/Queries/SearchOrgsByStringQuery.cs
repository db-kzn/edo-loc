using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class SearchOrgsByStringQuery : IRequest<Result<List<OrgsResponse>>>
{
    public string Search { get; }

    public SearchOrgsByStringQuery(string search)
    {
        Search = search;
    }
}

internal class SearchOrgsByStringQueryHandler : IRequestHandler<SearchOrgsByStringQuery, Result<List<OrgsResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public SearchOrgsByStringQueryHandler(
        IUnitOfWork<int> unitOfWork
        )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<OrgsResponse>>> Handle(SearchOrgsByStringQuery query, CancellationToken cancellationToken)
    {
        var search = query.Search?.ToUpper();

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

        var orgs = _unitOfWork.Repository<Organization>().Entities;

        var foundOrgs = string.IsNullOrWhiteSpace(search) ? orgs : int.TryParse(search, out _)
            ? orgs.Where(o => o.Inn.Contains(search)) // || o.Name.ToUpper().Contains(search) || o.ShortName.ToUpper().Contains(search)
            : orgs.Where(o => o.Name.ToUpper().Contains(search) || o.ShortName.ToUpper().Contains(search));

        var result = await foundOrgs.Take(10)
            .Select(expression)
            .ToListAsync(cancellationToken);

        return await Result<List<OrgsResponse>>.SuccessAsync(result);
    }
}
