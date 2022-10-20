using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Responses.Agreements;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Queries;

public class AgreementsProgressQuery : IRequest<Result<List<AgreementsProgressResponse>>>
{
    public int Id { get; set; }
    public int? AgreementId { get; set; }

    public AgreementsProgressQuery(int docId, int? argId)
    {
        Id = docId;
        AgreementId = argId;
    }
}

internal class AgreementsProgressQueryHandler : IRequestHandler<AgreementsProgressQuery, Result<List<AgreementsProgressResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<AgreementsProgressQueryHandler> _localizer;

    //private readonly IMapper _mapper;
    //private readonly IAppCache _cache;

    public AgreementsProgressQueryHandler(
        IUnitOfWork<int> unitOfWork,
        IUserService userService,
        IStringLocalizer<AgreementsProgressQueryHandler> localizer

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

    public async Task<Result<List<AgreementsProgressResponse>>> Handle(AgreementsProgressQuery request, CancellationToken cancellationToken)
    {
        var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(request.Id);
        if (doc == null) { return await Result<List<AgreementsProgressResponse>>.FailAsync(_localizer["Document Not Found"]); }

        bool commitRequired = false;

        if (doc.HasChanges)
        {
            doc.HasChanges = false;
            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            commitRequired = true;
        }

        if (request.AgreementId != null)
        {
            var a = _unitOfWork.Repository<Agreement>().Entities.FirstOrDefault(a => a.Document == doc && a.Id == request.AgreementId);
            if (a != null && a.Opened == null)
            {
                a.Opened = DateTime.Now;
                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                commitRequired = true;
            }
        }

        var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();

        List<AgreementsProgressResponse> docAgreements = new();

        agreements.ForEach((a) => {
            var employee = _userService.GetEmployeeAsync(a.EmplId).Result;
            var org = _unitOfWork.Repository<Organization>().Entities.FirstOrDefault(o => o.Id == employee.OrgId);

            docAgreements.Add(new()
            {
                AgreementId = a.Id,
                DocumentId = a.DocumentId,
                DocTypeId = doc.TypeId,

                EmplId = a.EmplId,
                Surname = employee.Surname,
                GivenName = employee.GivenName,

                UserOrgId = employee.OrgId,
                UserOrgType = org.Type,
                UserOrgShortName = org.ShortName,
                UserOrgInn = employee.InnLe,

                Step = a.StageNumber,
                State = a.State,
                Action = a.Action,
                IsCanceled = a.IsCanceled,

                CreatedOn = a.CreatedOn,
                Received = a.Received,
                Opened = a.Opened,
                Answered = a.Answered,

                Remark = a.Remark,
                SignURL = a.SignURL
            });
        });

        if (commitRequired) { await _unitOfWork.Commit(cancellationToken); }

        return await Result<List<AgreementsProgressResponse>>.SuccessAsync(docAgreements);
    }
}
