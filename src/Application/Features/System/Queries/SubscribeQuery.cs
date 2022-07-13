using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Queries
{
    public class SubscribeQuery : IRequest<Result<Subscribe>>
    {
        public string UserId { get; set; }
        public SubscribeQuery(string userId) { UserId = userId; }
    }

    internal class SubscribeQueryHandler : IRequestHandler<SubscribeQuery, Result<Subscribe>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        public SubscribeQueryHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<Result<Subscribe>> Handle(SubscribeQuery request, CancellationToken cancellationToken)
        {
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == request.UserId);

            if (subscribe is null)
            {
                subscribe = new Subscribe() { UserId = request.UserId };
                await _unitOfWork.Repository<Subscribe>().AddAsync(subscribe);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.SubscribeCacheKey);
            }

            return  await Result<Subscribe>.SuccessAsync(subscribe);
        }
    }
}
