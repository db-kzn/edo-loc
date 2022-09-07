using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Commands
{
    public class AgreementAnswerCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public AgreementStates State { get; set; }
        public string Remark { get; set; }
        public List<string> Members { get; set; }
        //public DateTime Time { get; set; } = DateTime.Now;
        public string Thumbprint { get; set; }

        public string URL { get; set; } // Ссылка для загрузки
        public UploadRequest UploadRequest { get; set; } // Сервис загруки файла
    }

    internal class AgreementAnswerCommandHandler : IRequestHandler<AgreementAnswerCommand, Result<int>>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AgreementAnswerCommandHandler> _localizer;

        private readonly List<MailToUser> _mails;

        public AgreementAnswerCommandHandler(
            IUserService userService,
            IUnitOfWork<int> unitOfWork,
            IUploadService uploadService,
            IStringLocalizer<AgreementAnswerCommandHandler> localizer
            )
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
            _localizer = localizer;

            _mails = new();
        }

        public async Task<Result<int>> Handle(AgreementAnswerCommand command, CancellationToken cancellationToken)
        {
            // Согласование для которого принимается ответ
            var agreement = await _unitOfWork.Repository<Agreement>().GetByIdAsync(command.Id);
            if (agreement == null) { return await Result<int>.FailAsync(_localizer["Agreement not found"]); }
            if (agreement.State != AgreementStates.Received) { return await Result<int>.FailAsync(_localizer["Сhange of agreement status is not allowed"]); }

            agreement.State = command.State;
            agreement.Remark = command.Remark;
            agreement.Answered = DateTime.Now;
            if (agreement.Opened == null) { agreement.Opened = agreement.Answered; }

            // (URL != null) => Upload Sign
            // (Members.Count > 0) => Add Members

            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(agreement.DocumentId);
            if (doc == null) { return await Result<int>.FailAsync(_localizer["Document not found"]); }

            var member = await _userService.GetEmployeeAsync(agreement.EmplId);
            var memberName = (member == null) ? string.Empty : member.GivenName;
            var memberOrg = await _unitOfWork.Repository<Organization>().GetByIdAsync(member.OrgId);
            var memberOrgName = string.IsNullOrWhiteSpace(memberOrg.ShortName) ? memberOrg.Name : memberOrg.ShortName;

            // Agreement.Step - Check
            if (command.State == AgreementStates.Approved || command.State == AgreementStates.Signed) // Для подписания или согласования
            {
                if (command.State == AgreementStates.Signed)
                {
                    AddMailAboutSigned(doc, memberOrgName, memberName);
                }
                else if (command.State == AgreementStates.Approved)
                {
                    AddMailAboutApproved(doc, memberOrgName, memberName);
                }

                // Отменить не ответивших доп.согласовантов |> Рецензентов
                var myAgrs = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.ParentId == agreement.Id && (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)).ToList();

                myAgrs.ForEach(async a =>
                {
                    a.State = AgreementStates.Deleted;
                    await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                });

                var cert = _unitOfWork.Repository<Certificate>().Entities.FirstOrDefault(c => c.UserId == agreement.EmplId && c.Thumbprint == command.Thumbprint);

                if (cert != null) { agreement.CertId = cert.Id; }

                var members = _unitOfWork.Repository<Agreement>().Entities.Where(a =>
                    a.DocumentId == agreement.DocumentId && a.StageNumber == agreement.StageNumber &&                                   // Документ и Шаг
                    (a.Action == ActTypes.Agreement || a.Action == ActTypes.Signing) &&                                                 // Основные согласованты и подписанты
                    (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)   // Только для не ответивших участников
                    ).ToList();

                // If Approve || Signed => Last Sign => Next Step || Finish
                if (members.Count == 1 && members[0].Id == command.Id) // Переход на следующий шаг
                {
                    if (doc.CurrentStep < doc.TotalSteps)
                    {
                        doc.CurrentStep++;
                        var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.DocumentId == doc.Id && a.StageNumber == doc.CurrentStep).ToList();

                        agreements.ForEach(async a =>
                        {
                            a.State = AgreementStates.Incoming;
                            await _unitOfWork.Repository<Agreement>().UpdateAsync(a);

                            AddMailAboutReceived(doc, a.EmplId);
                        });
                    }
                    else
                    {
                        doc.Stage = DocStages.Agreed;
                        doc.HasChanges = true;

                        AddMailAboutSignedByAll(doc);

                        var result = _uploadService.ArchiveDoc(doc.StoragePath, doc.FileName);
                        if (result != null)
                        {
                            doc.Stage = DocStages.Archive;

                            doc.URL = result.URL;
                            doc.StoragePath = result.StoragePath;
                            doc.FileName = result.FileName;
                        }
                    }
                }
            }
            else if (command.State == AgreementStates.Rejected)
            {
                doc.Stage = DocStages.Rejected;
                doc.HasChanges = true;

                AddMailAboutRefused(doc, memberOrgName, memberName, command.Remark);

                // Clear Agreements State Incoming
                var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.DocumentId == doc.Id).ToList();

                agreements.ForEach(async a =>
                {
                    if (a.State == AgreementStates.Incoming)
                    {
                        a.State = AgreementStates.Deleted;
                        await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                    }
                    else if (a.State == AgreementStates.Expecting)
                    {
                        a.State = AgreementStates.Undefined;
                        await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                    }
                });
            }
            //else if (command.State == AgreementStates.Refused || command.State == AgreementStates.Verifed) {}

            await _unitOfWork.Repository<Agreement>().UpdateAsync(agreement);
            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            await _unitOfWork.Commit(cancellationToken);

            await _userService.MailsToUsersAsync(_mails);

            return await Result<int>.SuccessAsync(agreement.Id, _localizer["Agreement Updated"]);
        }

        private void AddMailAboutSigned(Document doc, string memberOrgName, string memberName)
        {
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == doc.EmplId && s.Email.DocumentSigned); // 3.2

            if (subscribe is not null)
            {
                var mail = new MailToUser()
                {
                    UserId = doc.EmplId,
                    Theme = _localizer["The document you sent for signing №. {0} from {1:d} {2} signed by one of the participants.", doc.Number, doc.Date, doc.Title],
                    Text = $"{_localizer["Dear user!"]}<br/><br/>{_localizer["In the EDO system of the Ministry of Health, the document you sent for signing is № {0} from {1:d} {2} signed by one of the participants: {3}, {4}", doc.Number, doc.Date, doc.Title, memberOrgName, memberName]}"
                };

                _mails.Add(mail);
            }
        }
        private void AddMailAboutApproved(Document doc, string memberOrgName, string memberName)
        {
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == doc.EmplId && s.Email.DocumentApproved); // 3.1

            if (subscribe is not null)
            {
                var mail = new MailToUser()
                {
                    UserId = doc.EmplId,
                    Theme = _localizer["The document you sent for signing №. {0} from {1:d} {2} approved by one of the participants.", doc.Number, doc.Date, doc.Title],
                    Text = $"{_localizer["Dear user!"]}<br/><br/>{_localizer["In the EDO system of the Ministry of Health, the document you sent for signing is № {0} from {1:d} {2} approved by one of the participants: {3}, {4}", doc.Number, doc.Date, doc.Title, memberOrgName, memberName]}"
                };

                _mails.Add(mail);
            }
        }
        private void AddMailAboutReceived(Document doc, string emplId)
        {
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
        private void AddMailAboutSignedByAll(Document doc)
        {
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == doc.EmplId && s.Email.DocumentAgreed); // 4

            if (subscribe is not null) // ?.Email.DocumentAgreed == true
            {
                var mail = new MailToUser()
                {
                    UserId = doc.EmplId,
                    Theme = _localizer["The document you sent for signing № {0} from {1:d} {2} signed by all parties.", doc.Number, doc.Date, doc.Title],
                    Text = $"{_localizer["Dear user!"]}<br/><br/>{_localizer["In the EDO system of the Ministry of Health, the document you sent for signing is № {0} from {1:d} {2} signed by all parties.", doc.Number, doc.Date, doc.Title]}"
                };

                _mails.Add(mail);
            }
        }
        private void AddMailAboutRefused(Document doc, string memberOrgName, string memberName, string remark)
        {
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == doc.EmplId && s.Email.DocumentRejected); // 2

            if (subscribe is not null)
            {
                var mail = new MailToUser()
                {
                    UserId = doc.EmplId,
                    Theme = _localizer["The document you sent for signing № {0} from {1:d} {2} was refused to sign or approve.", doc.Number, doc.Date, doc.Title],
                    Text = _localizer["In the EDO system of the Ministry of Health, the document you sent for signing is № {0} from {1:d} {2} was refused to sign or approve, indicating the following remark. {3}, {4}: {5}", doc.Number, doc.Date, doc.Title, memberOrgName, memberName, remark]
                };

                _mails.Add(mail);
            }
        }
    }
}
