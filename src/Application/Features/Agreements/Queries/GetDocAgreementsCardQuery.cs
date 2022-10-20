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

public class GetDocAgreementsCardQuery : IRequest<Result<DocAgreementsCardResponse>>
{
    public int Id { get; set; }
    public int? AgreementId { get; set; }

    public GetDocAgreementsCardQuery(int docId, int? argId)
    {
        Id = docId;
        AgreementId = argId;
    }
}

internal class GetDocAgreementsCardQueryHandler : IRequestHandler<GetDocAgreementsCardQuery, Result<DocAgreementsCardResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<GetDocAgreementsCardQueryHandler> _localizer;

    public GetDocAgreementsCardQueryHandler(
        IUnitOfWork<int> unitOfWork,
        IUserService userService,
        IStringLocalizer<GetDocAgreementsCardQueryHandler> localizer
        )
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _localizer = localizer;
    }

    public async Task<Result<DocAgreementsCardResponse>> Handle(GetDocAgreementsCardQuery request, CancellationToken cancellationToken)
    {
        var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(request.Id);
        if (doc is null) { return await Result<DocAgreementsCardResponse>.FailAsync(_localizer["Document Not Found"]); }

        bool commitRequired = false;
        if (doc.HasChanges)
        {
            doc.HasChanges = false;
            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            commitRequired = true;
        }
        if (request.AgreementId is not null)
        {
            var a = _unitOfWork.Repository<Agreement>().Entities.FirstOrDefault(a => a.Document == doc && a.Id == request.AgreementId);
            if (a != null && a.Opened == null)
            {
                a.Opened = DateTime.Now;
                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                commitRequired = true;
            }
        }

        DocAgreementsCardResponse docAgrsCard = new()
        {
            Agreements = new(),

            DocId = doc.Id,
            RouteId = doc.RouteId,

            EmplId = doc.EmplId,
            EmplOrgId = doc.EmplOrgId,
            ExecutorId = doc.ExecutorId,

            Number = doc.Number,
            Date = doc.Date,
            Title = doc.Title,
            Description = doc.Description
        };

        var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();

        agreements.ForEach((a) =>
        {
            var employee = _userService.GetEmployeeAsync(a.EmplId).Result;
            var org = _unitOfWork.Repository<Organization>().Entities.FirstOrDefault(o => o.Id == employee.OrgId);
            var cert = _unitOfWork.Repository<Certificate>().Entities.FirstOrDefault(o => o.Id == a.CertId);

            docAgrsCard.Agreements.Add(new()
            {
                DocumentId = a.DocumentId,
                AgreementId = a.Id,
                RouteStepId = a.RouteStepId,
                StageNumber = a.StageNumber,
                IsAdditional = a.IsAdditional,

                EmplOrgId = org.Id,
                OrgType = org.Type,
                OmsCode = org.OmsCode,
                OrgInn = org.Inn,
                OrgShort = org.ShortName,

                EmplId = a.EmplId,
                EmplTitle = employee.Title,
                EmplSurname = employee.Surname,
                EmplGivenName = employee.GivenName,

                CertThumbprint = cert?.Thumbprint ?? string.Empty,
                CertFromDate = cert?.FromDate,
                CertTillDate = cert?.TillDate,
                CertAlgorithm = cert?.Algorithm ?? string.Empty,

                Step = a.StageNumber,
                Action = a.Action,
                State = a.State,
                Answered = a.Answered,

                Remark = a.Remark,
                SignURL = a.SignURL
            });
        });

        if (commitRequired) { await _unitOfWork.Commit(cancellationToken); }

        return await Result<DocAgreementsCardResponse>.SuccessAsync(docAgrsCard);
    }
}
