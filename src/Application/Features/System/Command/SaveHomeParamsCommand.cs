using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.System;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Command;

public class SaveHomeParamsCommand : IRequest<Result<bool>>
{
    public HomeConfiguration _config;
    public SaveHomeParamsCommand(HomeConfiguration config) { _config = config; }
}

internal class SaveHomeParamsCommandHandler : IRequestHandler<SaveHomeParamsCommand, Result<bool>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<SaveHomeParamsCommandHandler> _localizer;

    public SaveHomeParamsCommandHandler(
        IStringLocalizer<SaveHomeParamsCommandHandler> localizer,
        IUnitOfWork<int> unitOfWork
        )
    {
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(SaveHomeParamsCommand command, CancellationToken cancellationToken)
    {
        var config = command._config;

        var home = _unitOfWork.Repository<ParamGroup>()
                              .Entities.Include(g => g.Params)
                              .FirstOrDefault(g => g.Name == "HomePage");

        return (home is null)
            ? await CreateConfig(config, cancellationToken)
            : await UpdateConfig(home, config, cancellationToken);
    }

    private async Task<Result<bool>> CreateConfig(HomeConfiguration config, CancellationToken cancellationToken)
    {
        var mail = new ParamGroup()
        {
            Name = "HomePage",
            Version = 1,
            Params = new()
                {
                    new() { Property = "Title", Value = config.Title },
                    new() { Property = "Description", Value = config.Description },

                    new() { Property = "DocSupportPhone", Value = config.DocSupportPhone },
                    new() { Property = "DocSupportEmail", Value = config.DocSupportEmail },

                    new() { Property = "TechSupportPhone", Value = config.TechSupportPhone },
                    new() { Property = "TechSupportEmail", Value = config.TechSupportEmail }
                }
        };

        await _unitOfWork.Repository<ParamGroup>().AddAsync(mail);
        await _unitOfWork.Commit(cancellationToken);

        return await Result<bool>.SuccessAsync(true, _localizer["Home Page Configuration Created"]);
    }

    private async Task<Result<bool>> UpdateConfig(ParamGroup home, HomeConfiguration config, CancellationToken cancellationToken)
    {
        home.Params.ForEach(p =>
        {
            p.Value = p.Property switch
            {
                "Title" => config.Title,
                "Description" => config.Description,

                "DocSupportPhone" => config.DocSupportPhone,
                "DocSupportEmail" => config.DocSupportEmail,

                "TechSupportPhone" => config.TechSupportPhone,
                "TechSupportEmail" => config.TechSupportEmail,

                _ => string.Empty
            };
        });

        await _unitOfWork.Repository<ParamGroup>().UpdateAsync(home);
        await _unitOfWork.Commit(cancellationToken);

        return await Result<bool>.SuccessAsync(true, _localizer["Home Page Configuration Updated"]);
    }
}
