using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Entities.System;
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
            var agreement = await _unitOfWork.Repository<Agreement>().GetByIdAsync(command.Id); // not null

            if (agreement.State != AgreementStates.Received)
            {
                return await Result<int>.FailAsync(_localizer["Сhange of agreement status is not allowed"]);
            }

            agreement.State = command.State;
            agreement.Remark = command.Remark;
            agreement.Answered = DateTime.Now;
            agreement.Opened ??= agreement.Answered;

            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(agreement.DocumentId);
            var member = await _userService.GetEmployeeAsync(agreement.EmplId);
            var memberOrg = await _unitOfWork.Repository<Organization>().GetByIdAsync(member.OrgId);

            var memberName = (member == null) ? string.Empty : member.GivenName;
            var memberOrgName = string.IsNullOrWhiteSpace(memberOrg.ShortName) ? memberOrg.Name : memberOrg.ShortName;

            if (command.State == AgreementStates.Approved || command.State == AgreementStates.Signed)
            {
                AddMail(command.State, doc, memberOrgName, memberName);                      // Отправка письм Инициатору о Согласовании/Подписании участником
                agreement.CertId = FindEmployeeCertId(agreement.EmplId, command.Thumbprint); // Поиск идентификатора используемого сертификата
                CancelNotAnsweredRewiers(agreement.Id);                                      // Отмена не отвеченных доп. Согласований/Рецензирований

                var lastAgrs = FindMainNotAnsweredAgreements(agreement);

                if (lastAgrs.Count == 0) // Не ответивших участников нет. Перейти на следующий Этап или завершить
                {
                    NextStepOrComplete(doc);
                }
                else // Остались не ответившие участники. Требуется проверить условия завершения
                {
                    var routeStep = _unitOfWork.Repository<RouteStep>().Entities.FirstOrDefault(rs => rs.Id == agreement.RouteStepId);
                    if (routeStep is null)
                    {
                        // ERROR !!! Процесс не найден -> Отозвать документ, записать ошибку
                    }
                    else if (routeStep.SomeParticipants) // Не требуются все участники
                    {
                        if (routeStep.AllRequred) // По одному участнику в каждой организации
                        {
                            var canceledCount = 0;

                            lastAgrs.ForEach(async a =>
                            {
                                if (a.OrgId == agreement.OrgId)
                                {
                                    canceledCount++;
                                    //a.IsCanceled = true;
                                    a.State = AgreementStates.Deleted;
                                    await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                                }
                            });

                            if (canceledCount == lastAgrs.Count)
                            {
                                NextStepOrComplete(doc);
                            }
                        }
                        else
                        {
                            lastAgrs.ForEach(async a =>
                            {
                                //a.IsCanceled = true;
                                a.State = AgreementStates.Deleted;
                                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                            });

                            NextStepOrComplete(doc);
                        }
                    }
                }
            }
            else if (command.State == AgreementStates.Rejected)
            {
                DocumentRejection(doc, memberOrgName, memberName, command.Remark);
            }
            //else if (command.State == AgreementStates.Refused || command.State == AgreementStates.Verifed) {}

            await _unitOfWork.Repository<Agreement>().UpdateAsync(agreement);
            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            await _unitOfWork.Commit(cancellationToken);

            await _userService.MailsToUsersAsync(_mails);

            return await Result<int>.SuccessAsync(agreement.Id, _localizer["Agreement Updated"]);
        }

        private void CancelNotAnsweredRewiers(int agreementId)
        {
            var myAgrs = _unitOfWork.Repository<Agreement>().Entities
                .Where(a =>
                    a.ParentId == agreementId &&
                        (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)
                    )
                .ToList();

            myAgrs.ForEach(async a =>
            {
                a.State = AgreementStates.Deleted;
                await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
            });
        }
        private int? FindEmployeeCertId(string emplId, string thumbprint)
        {
            var cert = _unitOfWork.Repository<Certificate>().Entities
                .FirstOrDefault(c => c.UserId == emplId && c.Thumbprint == thumbprint);

            return (cert is not null) ? cert.Id : null;
        }
        private List<Agreement> FindMainNotAnsweredAgreements(Agreement agreement)
        {   // Согласования основных, не ответевших участники в данном документе и процесс
            return _unitOfWork.Repository<Agreement>().Entities
                .Where(a =>
                    a.DocumentId == agreement.DocumentId && a.StageNumber == agreement.StageNumber && a.Id != agreement.Id && !a.IsCanceled &&
                    (a.Action == ActTypes.Agreement || a.Action == ActTypes.Signing) &&
                    (a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened)
                ).ToList();
        }
        private void NextStepOrComplete(Document doc)
        {
            if (doc.CurrentStep < doc.TotalSteps) // Переход на следующий этап -> отправка сообщений участникам
            {
                doc.CurrentStep++;

                var agreements = _unitOfWork.Repository<Agreement>().Entities
                    .Where(a => a.DocumentId == doc.Id && a.StageNumber == doc.CurrentStep)
                    .ToList();

                agreements.ForEach(async a =>
                {
                    a.State = AgreementStates.Incoming;
                    await _unitOfWork.Repository<Agreement>().UpdateAsync(a);

                    AddMailAboutReceived(doc, a.EmplId);
                });
            }
            else // Завершение -> отправка уведомлений о завершении подписание и создание архива
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
        private void DocumentRejection(Document doc, string memberOrgName, string memberName, string remark)
        {
            doc.Stage = DocStages.Rejected;
            doc.HasChanges = true;

            AddMailAboutRefused(doc, memberOrgName, memberName, remark);

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

        private void AddMail(AgreementStates state, Document doc, string memberOrgName, string memberName)
        {
            if (state == AgreementStates.Signed)
            {
                AddMailAboutSigned(doc, memberOrgName, memberName);
            }
            else if (state == AgreementStates.Approved)
            {
                AddMailAboutApproved(doc, memberOrgName, memberName);
            }
        }
        private void AddMailAboutSigned(Document doc, string memberOrgName, string memberName)
        {
            var emplId = doc.EmplId;
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.DocumentSigned); // 3.2

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
            var emplId = doc.EmplId;
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.DocumentApproved); // 3.1

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
            var emplId = doc.EmplId;
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.DocumentAgreed); // 4

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
            var emplId = doc.EmplId;
            var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.DocumentRejected); // 2

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
