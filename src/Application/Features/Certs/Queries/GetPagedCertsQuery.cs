using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Requests.Admin;
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

namespace EDO_FOMS.Application.Features.Certs.Queries;

public class GetPagedCertsQuery : IRequest<PaginatedResult<CertsResponse>>
{
    public GetPagedCertsRequest Request { get; }
    public GetPagedCertsQuery(GetPagedCertsRequest request)
    {
        Request = request;
    }
}

internal class GetPagedCertsQueryHandler : IRequestHandler<GetPagedCertsQuery, PaginatedResult<CertsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    //private readonly ICurrentUserService _currentUserService;

    public GetPagedCertsQueryHandler(
        IUnitOfWork<int> unitOfWork
        //ICurrentUserService currentUserService
        )
    {
        _unitOfWork = unitOfWork;
        //_currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<CertsResponse>> Handle(GetPagedCertsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        Expression<Func<Certificate, CertsResponse>> expression = e => new CertsResponse
        {
            Id = e.Id,

            Thumbprint = e.Thumbprint,
            UserId = e.UserId,
            Snils = e.Snils,

            IsActive = e.IsActive,
            SignAllowed = e.SignAllowed,
            IssuerInn = e.IssuerInn,

            SerialNumber = e.SerialNumber,
            Algorithm = e.Algorithm,
            Version = e.Version,

            FromDate = e.FromDate,
            TillDate = e.TillDate,
            CreatedOn = e.CreatedOn
        };

        var certSpec = new CertSpecification(request);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await _unitOfWork.Repository<Certificate>().Entities
            .Specify(certSpec)
            .Select(expression)
            .OrderBy(sort)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
