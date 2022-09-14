using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Queries;

public class GetMailParamsQuery : IRequest<Result<MailConfiguration>>
{
}

internal class GetMailParamsQueryHandler : IRequestHandler<GetMailParamsQuery, Result<MailConfiguration>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    public GetMailParamsQueryHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<Result<MailConfiguration>> Handle(GetMailParamsQuery _, CancellationToken cancellationToken)
    {
        var mail = _unitOfWork.Repository<ParamGroup>()
                              .Entities.Include(g => g.Params)
                              .FirstOrDefault(g => g.Name == "MailServer");

        if (mail is null) { return await Result<MailConfiguration>.FailAsync(); }

        var mailPort = mail.Params.FirstOrDefault(p => p.Property == "Port")?.Value ?? string.Empty;

        var config = new MailConfiguration()
        {
            From = mail.Params.FirstOrDefault(p => p.Property == "From")?.Value ?? string.Empty,
            DisplayName = mail.Params.FirstOrDefault(p => p.Property == "DisplayName")?.Value ?? string.Empty,

            Host = mail.Params.FirstOrDefault(p => p.Property == "Host")?.Value ?? string.Empty,
            Port = 0,

            UserName = mail.Params.FirstOrDefault(p => p.Property == "UserName")?.Value ?? string.Empty,
            Password = mail.Params.FirstOrDefault(p => p.Property == "Password")?.Value ?? string.Empty,

            MailPattern = mail.Params.FirstOrDefault(p => p.Property == "MailPattern")?.Value ?? string.Empty
        };

        if (int.TryParse(mailPort, out int port)) { config.Port = port; }

        return await Result<MailConfiguration>.SuccessAsync(config);
    }
}
