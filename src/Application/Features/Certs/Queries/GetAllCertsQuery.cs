using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Certs.Queries
{
    public class GetAllCertsQuery : IRequest<Result<List<CertsResponse>>>
    {
        //public GetAllCertsQuery() {}
    }

    internal class GetAllCertsQueryHandler : IRequestHandler<GetAllCertsQuery, Result<List<CertsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;
        //private readonly ICurrentUserService _currentUserService;

        public GetAllCertsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache) //, ICurrentUserService currentUserService
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            //_currentUserService = currentUserService;
        }

        public async Task<Result<List<CertsResponse>>> Handle(GetAllCertsQuery request, CancellationToken cancellationToken)
        {
            Task<List<Certificate>> getAllCerts() => _unitOfWork.Repository<Certificate>().GetAllAsync();
            var certList = await _cache.GetOrAddAsync(AppConstants.Cache.GetAllCertsCacheKey, getAllCerts);
            var mappedCerts = _mapper.Map<List<CertsResponse>>(certList);
            return await Result<List<CertsResponse>>.SuccessAsync(mappedCerts);
        }
    }
}
