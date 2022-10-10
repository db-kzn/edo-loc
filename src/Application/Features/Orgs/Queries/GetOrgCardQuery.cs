using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Responses.Orgs;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries;

public class GetOrgCardQuery : IRequest<Result<OrgCardResponse>>
{
    public int Id { get; set; }

    //public GetOrgCardQuery() { }
    public GetOrgCardQuery(int id) { Id = id; }
}

internal class GetOrgCardQueryHandler : IRequestHandler<GetOrgCardQuery, Result<OrgCardResponse>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetOrgCardQueryHandler(
        IUserService userService,
        IUnitOfWork<int> unitOfWork
        )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrgCardResponse>> Handle(GetOrgCardQuery query, CancellationToken cancellationToken)
    {
        var org = await _unitOfWork.Repository<Organization>().Entities.FirstOrDefaultAsync(o => o.Id == query.Id, cancellationToken: cancellationToken);

        if (org == null) { return await Result<OrgCardResponse>.FailAsync(); }

        var card = new OrgCardResponse()
        {
            Id = org.Id,

            Inn = org.Inn,
            Code = org.OmsCode,
            Name = org.Name,
            ShortName = org.ShortName,
            
            IsPublic = org.IsPublic,
            Type = org.Type,
            State = org.State,
            
            Phone = org.Phone,
            Email = org.Email,
            CreatedOn = org.CreatedOn
        };

        var result = await _userService.GetAllByOrgIdAsync(query.Id);
        if (result.Succeeded) card.Employees = result.Data;

        return await Result<OrgCardResponse>.SuccessAsync(card);
    }
}
