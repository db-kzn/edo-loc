using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.System;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Commands
{
    public partial class ChangeDocStageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public DocStages Stage { get; set; }
    }

    internal class ChangeDocStageHandler : IRequestHandler<ChangeDocStageCommand, Result<int>>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<ChangeDocStageHandler> _localizer;

        private readonly List<MailToUser> _mails;

        public ChangeDocStageHandler(
            IUserService userService,
            IUnitOfWork<int> unitOfWork,
            IUploadService uploadService,
            IStringLocalizer<ChangeDocStageHandler> localizer
            )
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
            _localizer = localizer;

            _mails = new();
        }

        public async Task<Result<int>> Handle(ChangeDocStageCommand command, CancellationToken cancellationToken)
        {
            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(command.Id);
            if (doc == null) { return await Result<int>.FailAsync(_localizer["Document Not Found"]); }

            if (doc.HasChanges) { doc.HasChanges = false; }

            if (!((doc.Stage == DocStages.Draft && command.Stage == DocStages.InProgress) || // Черновик - запустить
                ((doc.Stage == DocStages.InProgress || doc.Stage == DocStages.Rejected) && command.Stage == DocStages.Draft) || // На подписании - прервать, Отклонено - перезапустить
                ((doc.Stage == DocStages.Draft || doc.Stage == DocStages.InProgress || doc.Stage == DocStages.Rejected) && command.Stage == DocStages.Deleted))) // Черновик, на подписании, отклонено - Удалить
            {
                return await Result<int>.FailAsync(_localizer["The command is not correct"]);
            }

            var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();

            if (command.Stage == DocStages.Draft)           { await DocToNewDraftAsync(doc, agreements); }
            else if (command.Stage == DocStages.InProgress) { await SendDocAlongRouteAsync(doc, agreements); }
            else if (command.Stage == DocStages.Deleted)    { await SeеDocStageDeletedAsync(doc, agreements); }

            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            await _unitOfWork.Commit(cancellationToken);
            await _userService.MailsToUsersAsync(_mails);

            return await Result<int>.SuccessAsync(doc.Id, _localizer["Document Stage Updated"]);
        }

        private async Task SendDocAlongRouteAsync(Document doc, List<Agreement> agreements)
        {
            doc.Stage = DocStages.InProgress;

            // Если на первом шаге нет согласовантов, то перейти ко второму
            var count = 0;
            agreements.ForEach((a) => { if (a.StageNumber == 1) { count++; } });
            doc.CurrentStep = (count != 0) ? 1 : 2;

            // Определить статус для контактов текущего шага - входящие, последующие - ожидание
            //agreements.ForEach(async a =>
            foreach (var a in agreements)
            {
                a.State = (a.Action == ActTypes.Executing) ? AgreementStates.Control :
                          (a.StageNumber == doc.CurrentStep) ? AgreementStates.Incoming : AgreementStates.Expecting;

                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);

                if (a.State == AgreementStates.Incoming)
                {
                    var emplId = a.EmplId;
                    var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.AgreementIncoming); // 1

                    if (subscribe is not null)
                    {
                        var mail = new MailToUser()
                        {
                            UserId = emplId,
                            Theme = _localizer["New document received"],
                            Text = $"{_localizer["Dear user!"]}<br/><br/>{_localizer["You have received a new document for signing (approval) {0} of {1:d} {2}", doc.Number, doc.Date, doc.Title]}"
                        };

                        _mails.Add(mail);
                    }
                }
            };
        }
        private async Task DocToNewDraftAsync(Document doc, List<Agreement> agreements)
        {
            _uploadService.DeleteFolder(doc.StoragePath);

            doc.StoragePath = "";
            doc.URL = "";
            doc.Stage = DocStages.Canceled; // Текущий документ в отмененные

            var newDoc = new Document()
            {
                EmplId = doc.EmplId,
                EmplOrgId = doc.EmplOrgId,
                ExecutorId = doc.ExecutorId,

                ParentId = doc.ParentId,
                PreviousId = doc.Id,

                RouteId = doc.RouteId,
                Stage = DocStages.Draft,
                HasChanges = false,

                TypeId = doc.TypeId,
                Number = doc.Number,
                Date = doc.Date,

                Title = doc.Title,
                Description = doc.Description,
                IsPublic = doc.IsPublic,

                CurrentStep = 0,
                TotalSteps = doc.TotalSteps,
                Version = doc.Version + 1,

                URL = "",
                StoragePath = "",
                FileName = "",

                Agreements = new()
            };

            //agreements.ForEach(async a =>
            foreach(var a in agreements)
            {
                if (a.ParentId is null)
                {
                    // Сбросить согласования из состояния входящие (или полученные, или открытые) в удален
                    if (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)
                    {
                        a.State = AgreementStates.Deleted;
                        await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                    }
                    else if (a.State == AgreementStates.Expecting)
                    {
                        a.State = AgreementStates.Undefined;
                        await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                    }

                    // Сделать копию контактов для новой версии проекта
                    newDoc.Agreements.Add(new()
                    {
                        Document = newDoc,
                        RouteStepId = a.RouteStepId,
                        StageNumber = a.StageNumber,

                        OmsCode = a.OmsCode,
                        OrgInn = a.OrgInn,
                        OrgId = a.OrgId,
                        EmplId = a.EmplId,

                        IsRequired = a.IsRequired,
                        IsCanceled = false,
                        IsAdditional = a.IsAdditional,

                        Action = a.Action,
                        State = AgreementStates.Undefined,

                        Received = null,
                        Opened = null,
                        Answered = null,

                        Remark = string.Empty,
                        SignURL = string.Empty,
                        CertId = null
                    });
                }
            };

            await _unitOfWork.Repository<Document>().AddAsync(newDoc);
        }
        private async Task SeеDocStageDeletedAsync(Document doc, List<Agreement> agreements)
        {
            _uploadService.DeleteFolder(doc.StoragePath);

            doc.StoragePath = "";
            doc.URL = "";
            doc.CurrentStep = 0;
            doc.Stage = DocStages.Deleted;

            // Сбросить согласования из состояния входящие в удален, а ожидание в неопределен
            //agreements.ForEach(async a =>
            foreach (var a in agreements)
            {
                if (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)
                {
                    a.State = AgreementStates.Deleted;
                }
                else if (a.State == AgreementStates.Expecting)
                {
                    a.State = AgreementStates.Undefined;
                }
                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
            };
        }
    }
}
