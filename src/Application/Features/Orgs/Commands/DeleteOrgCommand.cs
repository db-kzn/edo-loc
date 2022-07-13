using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Commands
{
    public class DeleteOrgCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrgCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteOrganizationCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteOrganizationCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteOrganizationCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteOrgCommand command, CancellationToken cancellationToken)
        {
            var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(command.Id);
            if (org != null)
            {
                await _unitOfWork.Repository<Organization>().DeleteAsync(org);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Organization Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Organization Not Found!"]);
            }
        }
    }
}
