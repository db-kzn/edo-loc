using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Queries
{
    public class GetAllOrgsQuery : IRequest<Result<List<OrgsResponse>>>
    {
        public GetAllOrgsQuery()
        {
        }
    }

    internal class GetAllOrgsQueryHandler : IRequestHandler<GetAllOrgsQuery, Result<List<OrgsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;
        //private readonly ICurrentUserService _currentUserService;

        public GetAllOrgsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache) //, ICurrentUserService currentUserService
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            //_currentUserService = currentUserService;
        }

        public async Task<Result<List<OrgsResponse>>> Handle(GetAllOrgsQuery request, CancellationToken cancellationToken)
        {
            Task<List<Organization>> getAllOrgs() => _unitOfWork.Repository<Organization>().GetAllAsync();
            var orgList = await _cache.GetOrAddAsync(AppConstants.Cache.GetAllOrgsCacheKey, getAllOrgs);
            var mappedOrgs = _mapper.Map<List<OrgsResponse>>(orgList);
            return await Result<List<OrgsResponse>>.SuccessAsync(mappedOrgs);
        }
    }
}
