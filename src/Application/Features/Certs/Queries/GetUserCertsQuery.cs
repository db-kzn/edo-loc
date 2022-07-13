using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Certs.Queries
{
    public class GetUserCertsQuery : IRequest<Result<List<GetUserCertsResponse>>>
    {
        public string UserId { get; set; }

        public GetUserCertsQuery(string userId)
        {
            UserId = userId;
        }
    }

    internal class GetUserCertsQueryHandler : IRequestHandler<GetUserCertsQuery, Result<List<GetUserCertsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;
        //private readonly ICurrentUserService _currentUserService;

        public GetUserCertsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache) //, ICurrentUserService currentUserService
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            //_currentUserService = currentUserService;
        }

        public async Task<Result<List<GetUserCertsResponse>>> Handle(GetUserCertsQuery request, CancellationToken cancellationToken)
        {
            List<Certificate> userCerts = _unitOfWork.Repository<Certificate>().Entities.Where(c => c.UserId == request.UserId).ToList();
            //var certList = await _cache.GetOrAddAsync(AppConstants.Cache.GetAllCertsCacheKey, getUserCerts);
            var mappedCerts = _mapper.Map<List<GetUserCertsResponse>>(userCerts);
            return await Result<List<GetUserCertsResponse>>.SuccessAsync(mappedCerts);
        }
    }
}
