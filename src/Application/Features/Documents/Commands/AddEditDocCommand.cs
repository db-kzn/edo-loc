using EDO_FOMS.Application.Requests;
using EDO_FOMS.Application.Serialization.JsonConverters;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using Microsoft.Extensions.Localization;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Application.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EDO_FOMS.Application.Features.Documents.Commands;

public partial class AddEditDocCommand : IRequest<Result<int>>
{
    public int Id { get; set; } = 0;                                           // 0 - Новый документ
    public int? ParentId { get; set; }                                         // Родительский документ

    public string EmplId { get; set; } = string.Empty;                         // Инициатор подписания
    public int EmplOrgId { get; set; } = 0;                                    // Организация инициатора
    public string ExecutorId { get; set; } = string.Empty;                     // Исполнитель

    public int TypeId { get; set; } = 1;                                       // Тип документа: 1 - Договор, 2 - Приложение 
    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Number { get; set; } = string.Empty;                         // Номер
    public DateTime? Date { get; set; } = null;                                // Дата

    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Title { get; set; } = string.Empty;                          // Наименование
    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Description { get; set; } = string.Empty;                    // Описание
    public bool IsPublic { get; set; } = false;                                // Публичный документ - виден всем сотрудникам Организации

    public int RouteId { get; set; } = 0;                                      // Маршрут документа. 0 - не определен
    public DocStages Stage { get; set; } = DocStages.Undefined;                // Статус документа:
    public int CurrentStep { get; set; } = 0;                                  // Текущий этап подписания
    public int TotalSteps { get; set; } = 0;                                   // Всего этапов в маршруте

    public string URL { get; set; } = string.Empty;                            // Ссылка для загрузки
    public UploadRequest UploadRequest { get; set; }                           // Сервис загруки файла

    public List<ActMember> Members { get; set; }                               // Участники подписания
}

public class ActMember
{
    public int? RouteStepId { get; set; }                                      // Идентификатор процесса маршрута, к которому относится участник
    public int StageNumber { get; set; } = 0;                                  // StageNumber => Порядковый номер этапа маршрута

    public ActTypes Act { get; set; } = ActTypes.Undefined;                    // Тип процесса: подписание, согласование или рецензирование
    public bool IsAdditional { get; set; } = false;                            // Дополнительный участник, не основной

    public string OmsCode { get; set; } = string.Empty;                        // Код Организации в НСИ
    public string OrgInn { get; set; } = string.Empty;                         // ИНН Организации, если не зарегистрирована в системе
    public int? OrgId { get; set; } = null;                                    // ИД зарегистрированной Организации
    public string EmplId { get; set; } = string.Empty;                         // ИД сотрудника, если определен
}

internal class AddEditDocCommandHandler : IRequestHandler<AddEditDocCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUploadService _uploadService;
    private readonly IStringLocalizer<AddEditDocCommandHandler> _localizer;

    public AddEditDocCommandHandler(
        IMapper mapper,
        IUserService userService,
        IUnitOfWork<int> unitOfWork,
        IUploadService uploadService,
        IStringLocalizer<AddEditDocCommandHandler> localizer
        )
    {
        _mapper = mapper;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _uploadService = uploadService;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(AddEditDocCommand command, CancellationToken cancellationToken)
    {
        // Документ либо в статусе черновика, либо только что отправлен по маршруту
        command.Stage = (command.CurrentStep == 0) ? DocStages.Draft : DocStages.InProgress;

        return (command.Id == 0)
            ? await AddDocAsync(command, cancellationToken)
            : await EditDocAsync(command, cancellationToken);
    }

    private async Task<Result<int>> AddDocAsync(AddEditDocCommand command, CancellationToken cancellationToken)
    {

        //var doc = _mapper.Map<Document>(command);
        //doc.Version = 1;
        //doc.StoragePath = "";

        var doc = new Document()
        {
            //Agreements = new(),
            //PacketFiles = new(),

            EmplId = command.EmplId,
            EmplOrgId = command.EmplOrgId,
            ExecutorId = command.ExecutorId,

            ParentId = null,
            PreviousId = null,

            RouteId = command.RouteId,
            Stage = command.Stage,
            HasChanges = true,

            TypeId = command.TypeId,
            Number = command.Number,
            Date = command.Date,

            Title = command.Title,
            Description = command.Description,
            IsPublic = false,

            CurrentStep = command.CurrentStep,
            TotalSteps = command.TotalSteps,

            URL = command.URL,
            StoragePath = string.Empty,
            Version = 1
        };

        if (command.UploadRequest != null) { Upload(ref doc, command.UploadRequest); }

        // Определение первого активного этапа маршрута
        if (command.Members is not null && command.CurrentStep > 0)
        {
            var activeCounts = new int[doc.TotalSteps + 1];

            // Поиск активных обязательных участников по всем этапам
            command.Members.ForEach(m =>
            {
                if ((m.StageNumber != 0 && m.StageNumber < doc.TotalSteps) && !(m.IsAdditional && m.Act != ActTypes.Review))
                {
                    activeCounts[m.StageNumber - 1]++;
                }
            });

            var firstStage = Array.FindIndex(activeCounts, count => count > 0);
            doc.CurrentStep = firstStage + 1;
        }

        List<MailToUser> mails = new();

        command.Members.ForEach(m =>
        {
            var state = (doc.Stage == DocStages.Draft) ? AgreementStates.Undefined :
                        (m.Act == ActTypes.Initiation) ? AgreementStates.Control :
                        (doc.CurrentStep == m.StageNumber) ? AgreementStates.Incoming : AgreementStates.Expecting;

            // Использовалось в предыдущей модели данных
            //var action = (m.Act == ActTypes.Initiation) ? AgreementActions.ToRun :
            //             (m.Act == ActTypes.Signing) ? AgreementActions.ToSign :
            //             (m.Act == ActTypes.Agreement) ? AgreementActions.ToApprove :
            //             (m.Act == ActTypes.Review) ? AgreementActions.ToReview : AgreementActions.Undefined;

            doc.Agreements.Add(new()
            {
                Document = doc,
                RouteStepId = m.RouteStepId,
                StageNumber = m.StageNumber,

                IsRequired = !(m.IsAdditional && m.Act != ActTypes.Review),
                IsAdditional = m.IsAdditional,
                Action = m.Act, //action,
                State = state,

                OmsCode = m.OmsCode,
                OrgInn = m.OrgInn,
                OrgId = m.OrgId,
                EmplId = m.EmplId
            });

            //if (state == AgreementStates.Incoming)
            //{
            //    var subscribe = _unitOfWork.Repository<Subscribe>().Entities
            //        .FirstOrDefault(s => s.UserId == m.EmplId && s.Email.AgreementIncoming); // 1

            //    if (subscribe is not null)
            //    {
            //         var mail = new MailToUser()
            //        {
            //            UserId = m.EmplId,
            //            Theme = _localizer["New document received"],
            //            Text = $"Уважаемый пользователь! <br/><br/>{_localizer["You have received a new document for signing (approval) {0} of {1:d} {2}", doc.Number, doc.Date, doc.Title]}"
            //        };

            //        mails.Add(mail);
            //    }
            //}
        });

        await _unitOfWork.Repository<Document>().AddAsync(doc);
        await _unitOfWork.Commit(cancellationToken);
        //await _userService.MailsToUsersAsync(mails);

        return await Result<int>.SuccessAsync(doc.Id, _localizer["Document Saved"]);
    }

    private async Task<Result<int>> EditDocAsync(AddEditDocCommand command, CancellationToken cancellationToken)
    {
        var docs = _unitOfWork.Repository<Document>().Entities.Include(d => d.Agreements);
        var doc = await docs.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken: cancellationToken);
        if (doc is null) { return await Result<int>.FailAsync(_localizer["Document Not Found!"]); }

        doc.EmplId = command.EmplId;
        doc.EmplOrgId = command.EmplOrgId;
        doc.ExecutorId = command.ExecutorId;

        doc.TypeId = command.TypeId;
        doc.Number = command.Number;
        doc.Date = command.Date;
        doc.Title = command.Title;

        doc.Stage = command.Stage;
        doc.Description = command.Description;
        doc.IsPublic = command.IsPublic;

        doc.RouteId = command.RouteId;
        doc.Stage = command.Stage;
        doc.CurrentStep = command.CurrentStep;
        doc.TotalSteps = command.TotalSteps;

        // Определение первого активного этапа маршрута
        if (command.Members is not null && command.CurrentStep > 0)
        {
            var activeCounts = new int[doc.TotalSteps + 1];

            // Поиск активных обязательных участников по всем этапам
            command.Members.ForEach(m =>
            {
                if ((m.StageNumber != 0 && m.StageNumber < doc.TotalSteps) && !(m.IsAdditional && m.Act != ActTypes.Review))
                {
                    activeCounts[m.StageNumber - 1]++;
                }
            });

            var firstStage = Array.FindIndex(activeCounts, count => count > 0);
            doc.CurrentStep = firstStage + 1;
        }

        List<MailToUser> mails = new();

        // Удалить согласования, которые отсутствуют в новом варианте
        doc.Agreements.RemoveAll(a => !command.Members.Exists(m => m.EmplId == a.EmplId && m.RouteStepId == a.RouteStepId));

        // Добавить новые согласования, для новых участников, для которых еще не созданы согласования или обновить уже созданные
        command.Members.ForEach(m =>
        {
            var state = (doc.Stage == DocStages.Draft) ? AgreementStates.Undefined :
                        (m.Act == ActTypes.Initiation) ? AgreementStates.Control :
                        (doc.CurrentStep == m.StageNumber) ? AgreementStates.Incoming : AgreementStates.Expecting;

            var agr = doc.Agreements.FirstOrDefault(a => a.EmplId == m.EmplId && a.RouteStepId == m.RouteStepId);

            if (agr is null)
            {
                doc.Agreements.Add(new()
                {
                    Document = doc,
                    RouteStepId = m.RouteStepId,
                    StageNumber = m.StageNumber, // Привязан к RouteStepId

                    IsRequired = !(m.IsAdditional && m.Act != ActTypes.Review),
                    IsAdditional = m.IsAdditional,
                    Action = m.Act, //action,
                    State = state,

                    OmsCode = m.OmsCode,
                    OrgInn = m.OrgInn,
                    OrgId = m.OrgId,
                    EmplId = m.EmplId
                });
            }
            else
            {
                agr.StageNumber = m.StageNumber; // Привязан к RouteStepId

                agr.IsRequired = !(m.IsAdditional && m.Act != ActTypes.Review);
                agr.IsAdditional = m.IsAdditional;
                agr.Action = m.Act; //action,
                agr.State = state;

                agr.OmsCode = m.OmsCode;
                agr.OrgInn = m.OrgInn;
                agr.OrgId = m.OrgId;
                agr.EmplId = m.EmplId;
            }

            //if (state == AgreementStates.Incoming)
            //{
            //    var subscribe = _unitOfWork.Repository<Subscribe>().Entities
            //        .FirstOrDefault(s => s.UserId == m.EmplId && s.Email.AgreementIncoming); // 1

            //    if (subscribe is not null)
            //    {
            //         var mail = new MailToUser()
            //        {
            //            UserId = m.EmplId,
            //            Theme = _localizer["New document received"],
            //            Text = $"Уважаемый пользователь! <br/><br/>{_localizer["You have received a new document for signing (approval) {0} of {1:d} {2}", doc.Number, doc.Date, doc.Title]}"
            //        };

            //        mails.Add(mail);
            //    }
            //}
        });

        await _unitOfWork.Repository<Document>().UpdateAsync(doc);
        await _unitOfWork.Commit(cancellationToken);
        //await _userService.MailsToUsersAsync(mails);

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
