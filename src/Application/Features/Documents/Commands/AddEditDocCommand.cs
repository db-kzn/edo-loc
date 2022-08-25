using EDO_FOMS.Application.Requests;
using EDO_FOMS.Application.Serialization.JsonConverters;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using Microsoft.Extensions.Localization;
using EDO_FOMS.Domain.Entities.Doc;

namespace EDO_FOMS.Application.Features.Documents.Commands;

public partial class AddEditDocCommand : IRequest<Result<int>>
{
    public int Id { get; set; } = 0;                                           // 0 - Новый документ
    public int? ParentId { get; set; }                                         // Родительский документ

    public string EmplId { get; set; } = string.Empty;                         // Инициатор подписания
    public int EmplOrgId { get; set; } = 0;                                    // Организация инициатора

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
    //public int StageNumber { get; set; } = 0;                                  // StageNumber => Agreement.Step - первоначальное имя столбца (не меняется)
    public int? StepId { get; set; }                                           // Идентификатор процесса маршрута, к которому относится участник
    public ActTypes Act { get; set; } = ActTypes.Undefined;                    // Тип процесса: подписание, согласование или рецензирование
    public bool IsAdditional { get; set; } = false;                            // Дополнительный участник, не основной

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
        command.Stage = (command.CurrentStep == 0) ? DocStages.Draft : DocStages.InProgress;

        return (command.Id == 0)
            ? await AddDocAsync(command, cancellationToken)
            : await EditDocAsync(command, cancellationToken);
    }

    private async Task<Result<int>> AddDocAsync(AddEditDocCommand command, CancellationToken cancellationToken)
    {
        var doc = _mapper.Map<Document>(command);
        doc.Version = 1;
        doc.StoragePath = "";

        if (command.UploadRequest != null) { Upload(ref doc, command.UploadRequest); }

        var members = command.Members;


        await _unitOfWork.Repository<Document>().AddAsync(doc);
        await _unitOfWork.Commit(cancellationToken);
        //await _userService.MailsToUsersAsync(mails);

        return await Result<int>.SuccessAsync(doc.Id, _localizer["Document Saved"]);
    }

    private async Task<Result<int>> EditDocAsync(AddEditDocCommand command, CancellationToken cancellationToken)
    {
        return null;
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
