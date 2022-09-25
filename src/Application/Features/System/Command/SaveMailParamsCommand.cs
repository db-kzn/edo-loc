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

public class SaveMailParamsCommand : IRequest<Result<bool>>
{
    public MailConfiguration _config;
    public SaveMailParamsCommand(MailConfiguration config) { _config = config; }
}

internal class SaveMailParamsCommandHandler : IRequestHandler<SaveMailParamsCommand, Result<bool>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<SaveMailParamsCommandHandler> _localizer;

    public SaveMailParamsCommandHandler(
        IStringLocalizer<SaveMailParamsCommandHandler> localizer,
        IUnitOfWork<int> unitOfWork
        )
    {
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(SaveMailParamsCommand command, CancellationToken cancellationToken)
    {
        var config = command._config;

        var mail = _unitOfWork.Repository<ParamGroup>()
                              .Entities.Include(g => g.Params)
                              .FirstOrDefault(g => g.Name == "MailServer");

        return (mail is null)
            ? await CreateConfig(config, cancellationToken)
            : await UpdateConfig(mail, config, cancellationToken);
    }

    private async Task<Result<bool>> CreateConfig(MailConfiguration config, CancellationToken cancellationToken)
    {
        var mail = new ParamGroup()
        {
            Name = "MailServer",
            Version = 1,
            Params = new()
                {
                    new() { Property = "From", Value = config.From },
                    new() { Property = "Host", Value = config.Host },
                    new() { Property = "Port", Value = config.Port is null ? string.Empty : config.Port.ToString() },
                    new() { Property = "UserName", Value = config.UserName },
                    new() { Property = "Password", Value = config.Password },
                    new() { Property = "DisplayName", Value = config.DisplayName },
                    new() { Property = "MailPattern", Value = config.MailPattern }
                }
        };

        await _unitOfWork.Repository<ParamGroup>().AddAsync(mail);
        await _unitOfWork.Commit(cancellationToken);

        return await Result<bool>.SuccessAsync(true, _localizer["Mail Configuration Created"]);
    }

    private async Task<Result<bool>> UpdateConfig(ParamGroup mail, MailConfiguration config, CancellationToken cancellationToken)
    {
        mail.Params.ForEach(p =>
        {
            p.Value = p.Property switch
            {
                "From" => config.From,
                "Host" => config.Host,
                "Port" => config.Port is null ? string.Empty : config.Port.ToString(),
                "UserName" => config.UserName,
                "Password" => config.Password,
                "DisplayName" => config.DisplayName,
                "MailPattern" => config.MailPattern,

                _ => string.Empty
            };
        });

        await _unitOfWork.Repository<ParamGroup>().UpdateAsync(mail);
        await _unitOfWork.Commit(cancellationToken);

        return await Result<bool>.SuccessAsync(true, _localizer["Mail Configuration Updated"]);
    }
}
