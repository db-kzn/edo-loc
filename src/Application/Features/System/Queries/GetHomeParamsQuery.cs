using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.System;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Queries;

public class GetHomeParamsQuery : IRequest<Result<HomeConfiguration>>
{
}

internal class GetHomeParamsQueryHandler : IRequestHandler<GetHomeParamsQuery, Result<HomeConfiguration>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    public GetHomeParamsQueryHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<Result<HomeConfiguration>> Handle(GetHomeParamsQuery _, CancellationToken cancellationToken)
    {
        var home = _unitOfWork.Repository<ParamGroup>()
                              .Entities.Include(g => g.Params)
                              .FirstOrDefault(g => g.Name == "HomePage");

        if (home is null) { return await Result<HomeConfiguration>.FailAsync(); }

        var config = new HomeConfiguration()
        {
            Title = home.Params.FirstOrDefault(p => p.Property == "Title")?.Value ?? string.Empty,
            Description = home.Params.FirstOrDefault(p => p.Property == "Description")?.Value ?? string.Empty,

            DocSupportPhone = home.Params.FirstOrDefault(p => p.Property == "DocSupportPhone")?.Value ?? string.Empty,
            DocSupportEmail = home.Params.FirstOrDefault(p => p.Property == "DocSupportEmail")?.Value ?? string.Empty,

            TechSupportPhone = home.Params.FirstOrDefault(p => p.Property == "TechSupportPhone")?.Value ?? string.Empty,
            TechSupportEmail = home.Params.FirstOrDefault(p => p.Property == "TechSupportEmail")?.Value ?? string.Empty
        };

        return await Result<HomeConfiguration>.SuccessAsync(config);
    }
}
