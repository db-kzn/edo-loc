using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.System.Queries
{
    public class NavCountsQuery : IRequest<Result<NavCountsModel>>
    {
        //public AgreementStates State { get; set; }
        public string UserId { get; set; }

        public NavCountsQuery(string userId)
        {
            //State = state;
            UserId = userId;
        }
    }

    internal class NavCountsQueryHandler : IRequestHandler<NavCountsQuery, Result<NavCountsModel>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        //private readonly ICurrentUserService _currentUserService;
        //private readonly string _emplId;

        public NavCountsQueryHandler(
            IUnitOfWork<int> unitOfWork
            //ICurrentUserService currentUserService
            )
        {
            _unitOfWork = unitOfWork;
            //_currentUserService = currentUserService;
            //_emplId = _currentUserService.UserId;
        }

        public async Task<Result<NavCountsModel>> Handle(NavCountsQuery request, CancellationToken cancellationToken)
        {
            var progress = _unitOfWork.Repository<Agreement>().Entities
                .Where(a =>
                    a.EmplId == request.UserId && a.Document.Stage == DocStages.InProgress &&
                    a.State != AgreementStates.Control && a.State != AgreementStates.Expecting &&
                    a.Opened == null)
                .Count();

            var docs = _unitOfWork.Repository<Document>().Entities
                .Where(d =>
                    (d.Stage == DocStages.Draft || d.Stage == DocStages.InProgress || d.Stage == DocStages.Rejected || d.Stage == DocStages.Agreed) &&
                    d.EmplId == request.UserId && d.HasChanges)
                .Count();

            var archs = _unitOfWork.Repository<Document>().Entities
                .Where(d => d.Stage == DocStages.Archive && d.EmplId == request.UserId && d.HasChanges)
                .Count();

            var counts = new NavCountsModel()
            {
                Progress = progress,
                Docs = docs,
                Archive = archs
            };

            return await Result<NavCountsModel>.SuccessAsync(counts);
        }
    }
}
