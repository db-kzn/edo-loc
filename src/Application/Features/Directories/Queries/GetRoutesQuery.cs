﻿using EDO_FOMS.Application.Extensions;
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

public class GetRoutesQuery : IRequest<PaginatedResult<RoutesResponse>>
{
    public GetPagedRoutesRequest Request { get; }
    public GetRoutesQuery(GetPagedRoutesRequest request)
    {
        Request = request;
    }
}

internal class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, PaginatedResult<RoutesResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetRoutesQueryHandler(
        IUnitOfWork<int> unitOfWork
        )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<RoutesResponse>> Handle(GetRoutesQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        Expression<Func<Route, RoutesResponse>> expression = e => new RoutesResponse
        {
            Id = e.Id,
            Number = e.Number,
            Name = e.Name,
            Description = e.Description,

            ForUserRole = e.ForUserRole,

            IsPackage = e.IsPackage,
            CalcHash = e.CalcHash,
            AttachedSign = e.AttachedSign,
            DisplayedSign = e.DisplayedSign,

            IsActive = e.IsActive,
            AllowRevocation = e.AllowRevocation,
            UseVersioning = e.UseVersioning,
            HasDetails = e.HasDetails
        };

        var routeSpec = new RouteFilterSpecification(request.SearchString, request.MatchCase);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await _unitOfWork.Repository<Route>().Entities
           .Specify(routeSpec)
           .Select(expression)
           .OrderBy(sort)
           .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}