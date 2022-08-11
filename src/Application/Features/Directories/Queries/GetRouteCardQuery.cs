using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Application.Features.Agreements.Queries;
using System.Collections.Generic;
using static MudBlazor.FilterOperator;
using System.Linq;
using System.Linq.Expressions;
using System;
using EDO_FOMS.Application.Models.Dir;
using static MudBlazor.Colors;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetRouteCardQuery : IRequest<Result<RouteCardResponse>>
{
    public int Id { get; }
    public GetRouteCardQuery() { }
    public GetRouteCardQuery(int id)
    {
        Id = id;
    }
}

internal class GetRouteCardQueryHandler : IRequestHandler<GetRouteCardQuery, Result<RouteCardResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetRouteCardQueryHandler(
        IUnitOfWork<int> unitOfWork
        )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RouteCardResponse>> Handle(GetRouteCardQuery request, CancellationToken cancellationToken)
    {
        var routes = _unitOfWork.Repository<Route>().Entities.Include(r => r.Stages).Include(r => r.Steps).Include(r => r.RouteDocTypes).Include(r => r.ForOrgTypes);
        var route = await routes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);

        var card = new RouteCardResponse
        {
            DocTypeIds = route.RouteDocTypes.Select(dt => (int)dt.DocumentTypeId).ToList(),
            ForOrgTypes = route.ForOrgTypes.Select(ot => ot.OrgType).ToList(),
            Stages = route.Stages.Select(s => new RouteStageModel(s)).ToList(),
            Steps = route.Steps.Select(s => new RouteStageStepModel(s)).ToList(),

            Id = route.Id,
            Number = route.Number,
            Name = route.Name,
            Description = route.Description,

            ForUserRole = route.ForUserRole,
            EndAction = route.EndAction,

            IsPackage = route.IsPackage,
            CalcHash = route.CalcHash,
            AttachedSign = route.AttachedSign,
            DisplayedSign = route.DisplayedSign,

            IsActive = route.IsActive,
            AllowRevocation = route.AllowRevocation,
            UseVersioning = route.UseVersioning,
            HasDetails = route.HasDetails
        };

        return await Result<RouteCardResponse>.SuccessAsync(card);
    }
}
