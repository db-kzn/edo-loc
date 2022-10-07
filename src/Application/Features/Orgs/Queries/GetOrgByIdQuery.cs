using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class GetOrgByIdQuery : IRequest<Result<GetOrgByIdResponse>>
{
    public int Id { get; set; }
}

internal class GetByIdOrganizationQueryHandler : IRequestHandler<GetOrgByIdQuery, Result<GetOrgByIdResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetByIdOrganizationQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetOrgByIdResponse>> Handle(GetOrgByIdQuery query, CancellationToken cancellationToken)
    {
        var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(query.Id);
        var mappedOrg = _mapper.Map<GetOrgByIdResponse>(org);
        return await Result<GetOrgByIdResponse>.SuccessAsync(mappedOrg);
    }
}
