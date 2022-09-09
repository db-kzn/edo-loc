using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class GetOrgIdByCodeQuery : IRequest<Result<int>>
{
    public string Code { get; set; }
}

internal class GetIdByCodeQueryHandler : IRequestHandler<GetOrgIdByCodeQuery, Result<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetIdByCodeQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(GetOrgIdByCodeQuery query, CancellationToken cancellationToken)
    {
        var company = await _unitOfWork.Repository<Company>().Entities.FirstOrDefaultAsync(c => c.Code == query.Code, cancellationToken);
        if (company is null) { return Result<int>.Success(-1); }

        var org = await _unitOfWork.Repository<Organization>().Entities.FirstOrDefaultAsync(o => o.Inn == company.Inn, cancellationToken);

        return (org is null) ? Result<int>.Success(0) : Result<int>.Success(org.Id);
    }
}
