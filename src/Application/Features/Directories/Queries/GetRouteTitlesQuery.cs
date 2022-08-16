using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using EDO_FOMS.Domain.Entities.Dir;
using System.Linq;
using System.Linq.Expressions;
using System;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetRouteTitlesQuery : IRequest<Result<List<RouteTitleModel>>>
{
    public GetRouteTitlesQuery()
    {
    }
}

internal class GetRouteTitlesQueryHandler : IRequestHandler<GetRouteTitlesQuery, Result<List<RouteTitleModel>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetRouteTitlesQueryHandler(
        IUnitOfWork<int> unitOfWork
        )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<RouteTitleModel>>> Handle(GetRouteTitlesQuery _, CancellationToken cancellationToken)
    {
        Expression<Func<Route, RouteTitleModel>> expression = e => new RouteTitleModel
        {
            Id = e.Id,
            Number = e.Number,
            Name = e.Name,
            Description = e.Description
        };

        var routeTitles = _unitOfWork.Repository<Route>().Entities.Select(expression).ToList();

        return await Result<List<RouteTitleModel>>.SuccessAsync(routeTitles);
    }
}
