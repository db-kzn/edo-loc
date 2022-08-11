using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using System.Linq;
using EDO_FOMS.Application.Models;
using System.Text.Json.Serialization;
using EDO_FOMS.Application.Serialization.JsonConverters;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;

namespace EDO_FOMS.Application.Features.Documents.Commands.AddEdit
{
    public partial class AddEditDocumentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; } = 0; // 0 - Новый документ
        public string EmplId { get; set; } // Инициатор подписания
        public int EmplOrgId { get; set; } // Организация инициатора
        public int? DocParentId { get; set; } // Родительский документ

        public int TypeId { get; set; } = 1; // Тип документа: 1 - Договор, 2 - Приложение ...
        //Type
        [JsonConverter(typeof(TrimmingJsonConverter))]
        public string Number { get; set; } = ""; // Номер
        public DateTime? Date { get; set; } = DateTime.Today; // Дата

        [JsonConverter(typeof(TrimmingJsonConverter))]
        public string Title { get; set; } = ""; // Наименование
        [JsonConverter(typeof(TrimmingJsonConverter))]
        public string Description { get; set; } = ""; // Описание
        public bool IsPublic { get; set; } = false; // Публичный документ - виден всем пользователям

        public int RouteId { get; set; } = 1; // Пока только один в системе
        //public int StatusIx { get; set; } = 0;// Статус: 0 - черновик
        public DocStages Stage { get; set; } // Статус документа:
        public int CurrentStep { get; set; } // Текущий этап подписания
        public int TotalSteps { get; set; } // Всего этапов подписания
        // Version

        public string URL { get; set; } // Ссылка для загрузки
        public UploadRequest UploadRequest { get; set; } // Сервис загруки файла
        // StoragePath
        // FileName

        // Участники согласования и подписания
        public List<AgreementContact> Contacts { get; set; }
        //public List<string> FundIds { get; set; }
        //public List<string> SmoIds { get; set; }
        //public List<string> MoIds { get; set; }
        //public List<string> HeadIds { get; set; }
    }

    public class AgreementContact
    {
        public int Step { get; set; }
        public string EmplId { get; set; }
        public int OrgId { get; set; }
        public AgreementActions Action { get; set; }
    }

    internal class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditDocumentCommandHandler> _localizer;

        public AddEditDocumentCommandHandler(
            IMapper mapper,
            IUserService userService,
            IUnitOfWork<int> unitOfWork,
            IUploadService uploadService,
            IStringLocalizer<AddEditDocumentCommandHandler> localizer
            )
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDocumentCommand command, CancellationToken cancellationToken)
        {
            //var uploadRequest = command.UploadRequest;
            //if (uploadRequest != null) { uploadRequest.FileName = $"D-{Guid.NewGuid()}{uploadRequest.Extension}"; }

            if (command.CurrentStep == 1)
            {
                command.Stage = DocStages.InProgress;
            }
            else if (command.CurrentStep == 0)
            {
                command.Stage = DocStages.Draft;
            }

            if (command.TypeId == 0) { command.TypeId = 1; }

            return (command.Id == 0)
                ? await AddDocAsync(command, cancellationToken)
                : await EditDocAsync(command, cancellationToken);
        }

        private async Task<Result<int>> AddDocAsync(AddEditDocumentCommand command, CancellationToken cancellationToken)
        {
            var doc = _mapper.Map<Document>(command);
            doc.Version = 1;
            doc.StoragePath = "";

            if (command.UploadRequest != null) {
                Upload(ref doc, command.UploadRequest);
            }

            var contacts = command.Contacts;

            // Если требуется запустить процесс подписания, определить начальный шаг 1 или 2
            if (doc.CurrentStep == 1)
            {
                var count = 0;
                contacts.ForEach((c) => { if (c.Step == 1) { count++; } });
                // Если на первом этапе нет подписантов, перейти на второй
                if (count == 0) { doc.CurrentStep = 2; }
            }

            List<MailToUser> mails = new();

            contacts.ForEach(c => {
                var emplId = c.EmplId;

                var state = (doc.Stage == DocStages.Draft) ? AgreementStates.Undefined :
                    (c.Action == AgreementActions.ToRun) ? AgreementStates.Control :
                    (doc.CurrentStep == c.Step) ? AgreementStates.Incoming : AgreementStates.Expecting;

                doc.Agreements.Add(new()
                {
                    Document = doc,
                    //Attempt = doc.Attempt,

                    EmplId = emplId,
                    OrgId = c.OrgId, //employee.OrgId,

                    Step = c.Step,
                    State = state,

                    Action = c.Action
                    // Remark // Time // URL
                });

                if (state == AgreementStates.Incoming)
                {
                    var subscribe = _unitOfWork.Repository<Subscribe>().Entities
                        .FirstOrDefault(s => s.UserId == emplId && s.Email.AgreementIncoming); // 1

                    if (subscribe is not null)
                    {
                        var mail = new MailToUser()
                        {
                            UserId = emplId,
                            Theme = _localizer["New document received"],
                            Text = $"Уважаемый пользователь! <br/><br/>{_localizer["You have received a new document for signing (approval) {0} of {1:d} {2}", doc.Number, doc.Date, doc.Title]}"
                        };

                        mails.Add(mail);
                        //await _userService.MailToUserAsync(mail);
                    }
                }
            });

            await _unitOfWork.Repository<Document>().AddAsync(doc);
            await _unitOfWork.Commit(cancellationToken);
            await _userService.MailsToUsersAsync(mails);

            return await Result<int>.SuccessAsync(doc.Id, _localizer["Document Saved"]);
        }
        private async Task<Result<int>> EditDocAsync(AddEditDocumentCommand command, CancellationToken cancellationToken)
        {
            Document doc = await _unitOfWork.Repository<Document>().GetByIdAsync(command.Id);
            if (doc == null) { return await Result<int>.FailAsync(_localizer["Document Not Found!"]); }

            doc.TypeId = command.TypeId;
            doc.Number = command.Number;
            doc.Date = command.Date ?? DateTime.Today;

            doc.Title = command.Title;
            doc.Description = command.Description;
            doc.IsPublic = command.IsPublic;

            doc.RouteId = command.RouteId;
            doc.Stage = command.Stage;
            doc.CurrentStep = command.CurrentStep;
            doc.TotalSteps = command.TotalSteps;

            // проверка существующих контактов
            var agreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.Document == doc).ToList();
            //if (agreements == null) { return await Result<int>.FailAsync(_localizer["Agreements Not Found"]); }

            // Если требуется запустить процесс подписания, определить начальный шаг 1 или 2
            if (doc.CurrentStep == 1)
            {
                var count = 0;
                command.Contacts.ForEach((c) => { if (c.Step == 1) { count++; } });
                if (count == 0) { doc.CurrentStep = 2; }
            }

            List<MailToUser> mails = new();

            agreements.ForEach(async a => {
                var emplId = a.EmplId;

                if (!command.Contacts.Exists(c => c.EmplId == emplId && c.Step == a.Step))
                {
                    await _unitOfWork.Repository<Agreement>().DeleteAsync(a);
                }
                else
                {
                    a.State = (doc.Stage == DocStages.Draft) ? AgreementStates.Undefined :
                              (a.Action == AgreementActions.ToRun) ? AgreementStates.Control :
                              (doc.CurrentStep == a.Step) ? AgreementStates.Incoming : AgreementStates.Expecting;

                    await _unitOfWork.Repository<Agreement>().UpdateAsync(a);

                    if (a.State == AgreementStates.Incoming)
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

                            mails.Add(mail);
                            //await _userService.MailToUserAsync(mail);
                        }
                    }
                }
            });

            // добавление новых контактов
            command.Contacts.ForEach(async c => {
                var isPresent = agreements.Exists(a => c.EmplId == a.EmplId && c.Step == a.Step);

                if (!isPresent)
                {
                    var employee = await _userService.GetEmployeeAsync(c.EmplId);

                    doc.Agreements.Add(new()
                    {
                        Document = doc,
                        //Attempt = doc.Attempt,

                        EmplId = c.EmplId,
                        OrgId = employee.OrgId,

                        Step = c.Step,
                        State = (doc.Stage == DocStages.Draft) ? AgreementStates.Undefined :
                                (c.Action == AgreementActions.ToRun) ? AgreementStates.Control :
                                (doc.CurrentStep == c.Step) ? AgreementStates.Incoming : AgreementStates.Expecting,

                        Action = c.Action
                        // Remark // Time // URL
                    });
                }
            });

            if (command.UploadRequest != null) { Upload(ref doc, command.UploadRequest); }

            await _unitOfWork.Repository<Document>().UpdateAsync(doc);
            await _unitOfWork.Commit(cancellationToken);

            await _userService.MailsToUsersAsync(mails);

            return await Result<int>.SuccessAsync(doc.Id, _localizer["Document Updated"]);
        }
        private void Upload(ref Document d, UploadRequest request)
        {
            if (request.Data != null)
            {
                var uploadResult = _uploadService.UploadDoc(request, d.Version, d.StoragePath);

                d.URL = uploadResult?.URL ?? "";
                d.StoragePath = uploadResult?.StoragePath ?? "";
                d.FileName = uploadResult?.FileName ?? "";
            }
        }
    }
}