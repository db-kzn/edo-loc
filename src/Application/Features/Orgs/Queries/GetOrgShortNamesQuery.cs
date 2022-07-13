using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class GetOrgShortNamesQuery : IRequest<Result<Dictionary<int, string>>>
{
    public int[] Ids { get; set; }

    public GetOrgShortNamesQuery(int[] ids)
    {
        Ids = ids;
    }
}

internal class GetOrgShortNamesQueryHandler : IRequestHandler<GetOrgShortNamesQuery, Result<Dictionary<int, string>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetOrgShortNamesQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Dictionary<int, string>>> Handle(GetOrgShortNamesQuery query, CancellationToken cancellationToken)
    {
        var orgs = _unitOfWork.Repository<Organization>().Entities
            .Where(o => query.Ids.Contains(o.Id))
            .Select(o => new {o.Id, o.ShortName})
            .ToList();

        Dictionary<int, string> names = new();
        orgs.ForEach(o => names.Add(o.Id, o.ShortName));

        return await Result<Dictionary<int, string>>.SuccessAsync(names);
    }
}
