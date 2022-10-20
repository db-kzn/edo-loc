using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries
{
    public class GetDocParticipantsQuery : IRequest<Result<List<DocParticipantResponse>>>
    {
        public int Id { get; set; }

        public GetDocParticipantsQuery(int docId)
        {
            Id = docId;
        }
    }

    internal class GetDocParticipantsQueryHandler : IRequestHandler<GetDocParticipantsQuery, Result<List<DocParticipantResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<GetDocParticipantsQueryHandler> _localizer;

        //private readonly IMapper _mapper;
        //private readonly IAppCache _cache;

        public GetDocParticipantsQueryHandler(
            IUnitOfWork<int> unitOfWork,
            IUserService userService,
            IStringLocalizer<GetDocParticipantsQueryHandler> localizer

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

        public async Task<Result<List<DocParticipantResponse>>> Handle(GetDocParticipantsQuery request, CancellationToken cancellationToken)
        {
            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(request.Id);
            if (doc == null) { return await Result<List<DocParticipantResponse>>.FailAsync(_localizer["Document Not Found"]); }

            var participants = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();

            List<DocParticipantResponse> docParticipants = new();

            participants.ForEach((a) =>
            {
                var employee = _userService.GetEmployeeAsync(a.EmplId).Result;
                var org = _unitOfWork.Repository<Organization>().Entities.FirstOrDefault(o => o.Id == employee.OrgId);

                docParticipants.Add(new()
                {
                    Step = a.StageNumber,
                    EmplId = a.EmplId,

                    Surname = employee.Surname,
                    GivenName = employee.GivenName,

                    OrgId = employee.OrgId,
                    OrgInn = employee.InnLe,

                    OrgShortName = org.ShortName,
                    OrgType = org.Type
                });
            });

            return await Result<List<DocParticipantResponse>>.SuccessAsync(docParticipants);
        }
    }
}
