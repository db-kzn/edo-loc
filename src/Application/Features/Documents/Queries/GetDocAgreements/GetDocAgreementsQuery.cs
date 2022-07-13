using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries.GetDocAgreements
{
    public class GetDocAgreementsQuery : IRequest<Result<List<GetDocAgreementsResponse>>>
    {
        public int Id { get; set; }

        public GetDocAgreementsQuery(int docId)
        {
            Id = docId;
        }
    }

    internal class GetDocAgreementsQueryHandler : IRequestHandler<GetDocAgreementsQuery, Result<List<GetDocAgreementsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<GetDocAgreementsQueryHandler> _localizer;

        //private readonly IMapper _mapper;
        //private readonly IAppCache _cache;

        public GetDocAgreementsQueryHandler(
            IUnitOfWork<int> unitOfWork,
            IUserService userService,
            IStringLocalizer<GetDocAgreementsQueryHandler> localizer

            //IMapper mapper,
            //IAppCache cache
            )
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _localizer = localizer;

            //_mapper = mapper;
            //_cache = cache;
        }

        public async Task<Result<List<GetDocAgreementsResponse>>> Handle(GetDocAgreementsQuery request, CancellationToken cancellationToken)
        {
            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(request.Id);
            if (doc == null) { return await Result<List<GetDocAgreementsResponse>>.FailAsync(_localizer["Document Not Found"]); }

            var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();

            List<GetDocAgreementsResponse> docAgreements = new();

            agreements.ForEach((a) => {
                var employee = _userService.GetEmployeeAsync(a.EmplId).Result;
                var org = _unitOfWork.Repository<Organization>().Entities.FirstOrDefault(o => o.Id == employee.OrgId);

                docAgreements.Add(new()
                {
                    Step = a.Step,
                    EmplId = a.EmplId,

                    Surname = employee.Surname,
                    GivenName = employee.GivenName,

                    OrgId = employee.OrgId,
                    OrgInn = employee.InnLe,

                    OrgShortName = org.ShortName,
                    OrgType = org.Type
                });
            });

            return await Result<List<GetDocAgreementsResponse>>.SuccessAsync(docAgreements);
        }
    }
}
