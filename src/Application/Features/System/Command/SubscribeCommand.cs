using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.System;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Command
{
    public class SubscribeCommand : IRequest<Result<int>>
    {
        public Subscribe Subscribe { get; set; }
        public SubscribeCommand(Subscribe subscribe) { Subscribe = subscribe; }
    }

    internal class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        public SubscribeCommandHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<Result<int>> Handle(SubscribeCommand request, CancellationToken cancellationToken)
        {
            var subscribe = request.Subscribe;

            if (subscribe.Id == 0)
            {
                await _unitOfWork.Repository<Subscribe>().AddAsync(subscribe);
            }
            else
            {
                var s = await _unitOfWork.Repository<Subscribe>().GetByIdAsync(subscribe.Id);

                s.Email.AgreementIncoming = subscribe.Email.AgreementIncoming;
                s.Email.DocumentRejected =  subscribe.Email.DocumentRejected;
                s.Email.DocumentApproved =  subscribe.Email.DocumentApproved;
                s.Email.DocumentSigned =    subscribe.Email.DocumentApproved;
                s.Email.DocumentAgreed =    subscribe.Email.DocumentAgreed;

                await _unitOfWork.Repository<Subscribe>().UpdateAsync(s);
            }

            await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.SubscribeCacheKey);

            return await Result<int>.SuccessAsync(subscribe.Id);
        }
    }
}
