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
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetActiveRoutesQuery : IRequest<Result<List<ActiveRouteModel>>>
{
    //public GetActiveRoutesQuery() { }
}

internal class GetRouteTitlesQueryHandler : IRequestHandler<GetActiveRoutesQuery, Result<List<ActiveRouteModel>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetRouteTitlesQueryHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<Result<List<ActiveRouteModel>>> Handle(GetActiveRoutesQuery _, CancellationToken cancellationToken)
    {
        Expression<Func<Route, ActiveRouteModel>> expression = e => new ActiveRouteModel
        {
            Id = e.Id,
            Number = e.Number,
            Count = null,

            Code = e.Code,
            Short = e.Short,
            Name = e.Name,

            Description = e.Description,
            ParseFileName = e.ParseFileName,
            Mask = e.Parses
                    .Where(p => p.PatternType == ParsePatterns.Mask)
                    .Select(p => p.Pattern)
                    .FirstOrDefault()
        };

        var routeTitles = _unitOfWork.Repository<Route>().Entities
            .Where(r => r.IsActive)
            .Select(expression)
            .ToList();

        return await Result<List<ActiveRouteModel>>.SuccessAsync(routeTitles);
    }
}
