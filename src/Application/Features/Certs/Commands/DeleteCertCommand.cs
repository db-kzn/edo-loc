using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Certs.Commands
{
    public class DeleteCertCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public DeleteCertCommand(int id) { Id = id; }
    }

    internal class DeleteCertCommandHandler : IRequestHandler<DeleteCertCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteCertCommandHandler> _localizer;

        public DeleteCertCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteCertCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteCertCommand command, CancellationToken cancellationToken)
        {
            var cert = await _unitOfWork.Repository<Certificate>().GetByIdAsync(command.Id);

            if (cert == null) { return await Result<int>.FailAsync(_localizer["Cert Not Found"]); }

            await _unitOfWork.Repository<Certificate>().DeleteAsync(cert);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllCertsCacheKey);

            return await Result<int>.SuccessAsync(cert.Id, _localizer["Cert Deleted"]);
        }
    }
}
