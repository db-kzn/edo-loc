using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class AddEditRouteCommand : IRequest<Result<int>>
    {
        public List<int> DocTypeIds { get; set; } = new();                       // + Типы документов для которых предназначен маршрут
        public List<OrgTypes> ForOrgTypes { get; set; } = new();                 // + Типы организаций которые могут использовать маршрут

        public List<RouteStageCommand> Stages { get; set; } = new();             // + Стадии текущего маршрута
        public List<RouteStepCommand> Steps { get; set; } = new();               // + Процессы в этапе + Участники процесса
        public List<RouteFileParseCommand> Parses { get; set; } = new();         // + Правила разбора имени файла

        public List<RoutePacketFile> Files { get; set; } = null;                 // - Типы файлов из пакета

        public int Id { get; set; }                                              // - Идентификатор маршрута
        public int Number { get; set; }                                          // - "Ценность" маршрута, для сортировки
        public string Code { get; set; } = string.Empty;                         // - Код маршрута

        public string Short { get; set; } = string.Empty;                        // + Наименование маршрута
        public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
        public string Description { get; set; } = string.Empty;                  // + Описание маршрута

        public string ExecutorId { get; set; } = string.Empty;
        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
        public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

        public bool IsActive { get; set; } = true;                               // + Используемый маршрут
        public bool DateIsToday { get; set; } = true;                            // + Дата документа - устанивить сегодня
        public bool NameOfFile { get; set; } = true;                             // + Наименование документа из имени файла
        public bool ParseFileName { get; set; } = false;                         // + Разбор имени файла

        public bool AllowRevocation { get; set; } = true;                        // + Возможность отзывать документ с маршрута
        public bool ProtectedMode { get; set; } = false;                         // + Карточка документа не редактируется
        public bool ShowNotes { get; set; } = false;                             // + Отобразить примечание/заметки
        public bool UseVersioning { get; set; } = false;                         // - Используется версионность

        public bool IsPackage { get; set; } = false;                             // - Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                              // - Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                          // - Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                         // - Отображаемая подпись руководителя

        public bool HasDetails { get; set; } = true;                             // - Отображать параметры этапов
    }
    public class RouteStageCommand
    {
        public int Id { get; set; }
        public int RouteId { get; set; }                   // - Внешний индекс
        public int Number { get; set; }                    // + Номер этапа в цепочке маршрута

        public Color Color { get; set; }                   // + Цвет формы
        public string Name { get; set; }                   // + Наименование этапа
        public string Description { get; set; }            // - Описание этапа

        public ActTypes ActType { get; set; }              // - Тип этапа: неопределенный, подписание, согласование или рецензирование
        public bool InSeries { get; set; } = false;        // - Последовательное прохождение
        public bool AllRequred { get; set; } = true;       // - Если параллельно, то требуются все

        public bool DenyRevocation { get; set; }           // - Возможность отзывать документ с маршрута
        public TimeSpan Validity { get; set; }             // - Срок на прохождение этапа
    }
    public class RouteStepCommand
    {
        public int RouteId { get; set; }                                         // - Внешний индекс
        public int Id { get; set; }                                              // - RouteStepId
        // IsDeleted - на клиенте не используется, только на сервере

        public int StageNumber { get; set; }                                     // + Номер этапа
        public int Number { get; set; }                                          // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing;                // + Тип шага: подписание, согласование или рецензирование
        public MemberGroups MemberGroup { get; set; }                            // + Группа участников

        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;              // + Тип организации, может быть не определен
        public int? OrgId { get; set; } = null;                                  // + Организация участник

        public bool IsKeyMember { get; set; } = false;                           // + Является ключевым участником
        public bool Requred { get; set; } = true;                                // + Обязательный шаг

        public bool SomeParticipants { get; set; } = true;                       // - Несколько участников
        public bool AllRequred { get; set; } = true;                             // + Если несколько, то условие завершения: все или любой

        public int AutoSearch { get; set; } = 0;                                 // + Автопоиск - количество записей
        public bool HasAgreement { get; set; } = false;                          // + Содержит согласование
        public bool HasReview { get; set; } = false;                             // + Содержит рецензирование

        public string Description { get; set; } = string.Empty;                  //   Описание
        public List<RouteStepMemberCommand> Members { get; set; } = new();       // + Список участников
    }
    public class RouteStepMemberCommand
    {
        public int RouteStepId { get; set; }                    // Идентификатор процесса (шага)
        public ActTypes Act { get; set; } = ActTypes.Undefined; // Тип действия
        public bool IsAdditional { get; set; } = false;         // Дополнительный, не основной
        public string UserId { get; set; } = string.Empty;      // Участник
    }
    public class RouteFileParseCommand
    {
        public ParsePatterns PatternType { get; set; } = ParsePatterns.Undefined; // Тип шаблона разбора
        public string Pattern { get; set; } = string.Empty;                       // Значение паттерна для Regex
        public ValueTypes ValueType { get; set; } = ValueTypes.String;            // Тип значения результата

        public RouteFileParseCommand() { }
        public RouteFileParseCommand(
            ParsePatterns patternType,
            string pattern,
            ValueTypes valueType
            )
        {
            PatternType = patternType;
            Pattern = pattern;
            ValueType = valueType;
        }
    }

    internal class AddEditRouteCommandHandler : IRequestHandler<AddEditRouteCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditRouteCommandHandler> _localizer;

        public AddEditRouteCommandHandler(
            IMapper mapper,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditRouteCommandHandler> localizer
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditRouteCommand command, CancellationToken cancellationToken)
        {
            return (command.Id == 0)
                ? await AddRouteAsync(command, cancellationToken)
                : await EditRouteAsync(command, cancellationToken);
        }

        private async Task<Result<int>> AddRouteAsync(AddEditRouteCommand command, CancellationToken cancellationToken)
        {
            var route = _mapper.Map<Route>(command);

            command.DocTypeIds.ForEach(id =>
                route.RouteDocTypes.Add(new RouteDocType() { Route = route, DocumentTypeId = id })
            );

            command.ForOrgTypes.ForEach(orgType =>
                route.ForOrgTypes.Add(new RouteOrgType() { Route = route, OrgType = orgType })
            );

            command.Stages.ForEach(stage =>
                route.Stages.Add(new RouteStage()
                {
                    Route = route,
                    Number = stage.Number,

                    Color = stage.Color,
                    Name = stage.Name,
                    Description = stage.Description,

                    ActType = stage.ActType,
                    InSeries = stage.InSeries,
                    AllRequred = stage.AllRequred,

                    DenyRevocation = stage.DenyRevocation,
                    Validity = stage.Validity
                })
            );

            command.Steps.ForEach(s =>
            {
                var step = new RouteStep()
                {
                    Route = route,
                    IsDeleted = false,

                    StageNumber = s.StageNumber,
                    Number = s.Number,

                    ActType = s.ActType,
                    MemberGroup = s.MemberGroup,

                    OrgType = s.OrgType,
                    OrgId = s.OrgId,

                    IsKeyMember = s.IsKeyMember,
                    Requred = s.Requred,

                    SomeParticipants = s.SomeParticipants,
                    AllRequred = s.AllRequred,

                    AutoSearch = s.AutoSearch,
                    HasAgreement = s.HasAgreement,
                    HasReview = s.HasReview,

                    Description = s.Description,
                    Members = new()
                };

                step.Members = s.Members.Select(m => NewMember(step, m)).ToList();

                route.Steps.Add(step);
            });

            command.Parses.ForEach(p =>
                route.Parses.Add(new RouteFileParse() // route, p.PatternType, p.Pattern, p.ValueType
                {
                    Route = route,
                    PatternType = p.PatternType,
                    Pattern = p.Pattern,
                    ValueType = p.ValueType
                }));

            await _unitOfWork.Repository<Route>().AddAsync(route);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<int>.SuccessAsync(route?.Id ?? 0, _localizer["Route Saved"]);
        }

        private async Task<Result<int>> EditRouteAsync(AddEditRouteCommand command, CancellationToken cancellationToken)
        {
            var routes = _unitOfWork.Repository<Route>().Entities
                .Include(r => r.RouteDocTypes).Include(r => r.ForOrgTypes).Include(r => r.Parses)
                .Include(r => r.Stages).Include(r => r.Steps).ThenInclude(s => s.Members);

            var route = await routes.FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken: cancellationToken);
            if (route is null) { return await Result<int>.FailAsync(_localizer["Route Not Found!"]); }

            route.Number = command.Number;
            route.Code = command.Code;

            route.Short = command.Short;
            route.Name = command.Name;
            route.Description = command.Description;

            route.ExecutorId = command.ExecutorId;
            route.ForUserRole = command.ForUserRole;
            route.EndAction = command.EndAction;

            route.IsActive = command.IsActive;
            route.DateIsToday = command.DateIsToday;
            route.NameOfFile = command.NameOfFile;
            route.ParseFileName = command.ParseFileName;

            route.AllowRevocation = command.AllowRevocation;
            route.ProtectedMode = command.ProtectedMode;
            route.ShowNotes = command.ShowNotes;
            route.UseVersioning = command.UseVersioning;

            route.IsPackage = command.IsPackage;
            route.CalcHash = command.CalcHash;
            route.AttachedSign = command.AttachedSign;
            route.DisplayedSign = command.DisplayedSign;

            route.HasDetails = command.HasDetails;

            route.RouteDocTypes.RemoveAll(r => !command.DocTypeIds.Exists(id => id == r.DocumentTypeId));
            command.DocTypeIds.ForEach(id =>
            {
                if (!route.RouteDocTypes.Exists(r => r.DocumentTypeId == id))
                {
                    route.RouteDocTypes.Add(new RouteDocType() { Route = route, DocumentTypeId = id });
                }
            });

            route.ForOrgTypes.RemoveAll(r => !command.ForOrgTypes.Exists(t => t == r.OrgType));
            command.ForOrgTypes.ForEach(t =>
            {
                if (!route.ForOrgTypes.Exists(r => r.OrgType == t))
                {
                    route.ForOrgTypes.Add(new RouteOrgType() { Route = route, OrgType = t });
                }
            });

            route.Stages.RemoveAll(r => !command.Stages.Exists(c => c.Id == r.Id));
            command.Stages.ForEach(c =>
            {
                if (c.Id == 0)
                {
                    route.Stages.Add(new RouteStage()
                    {
                        Route = route,
                        Number = c.Number,

                        Color = c.Color,
                        Name = c.Name,
                        Description = c.Description,

                        ActType = c.ActType,
                        InSeries = c.InSeries,
                        AllRequred = c.AllRequred,

                        DenyRevocation = c.DenyRevocation,
                        Validity = c.Validity
                    });
                }
                else
                {
                    var s = route.Stages.FirstOrDefault(r => r.Id == c.Id);
                    if (s != null)
                    {
                        s.Number = c.Number;

                        s.Color = c.Color;
                        s.Name = c.Name;
                        s.Description = c.Description;

                        s.ActType = c.ActType;
                        s.InSeries = c.InSeries;
                        s.AllRequred = c.AllRequred;

                        s.DenyRevocation = c.DenyRevocation;
                        s.Validity = c.Validity;
                    }
                }
            });

            route.Steps.ForEach(r =>
            {
                if (!command.Steps.Exists(c => c.Id == r.Id)) { r.IsDeleted = true; }
            });
            command.Steps.ForEach(c =>
            {
                if (c.Id == 0)
                {
                    var step = new RouteStep()
                    {
                        Route = route,
                        IsDeleted = false,

                        StageNumber = c.StageNumber,
                        Number = c.Number,

                        ActType = c.ActType,
                        MemberGroup = c.MemberGroup,

                        OrgType = c.OrgType,
                        OrgId = c.OrgId,

                        IsKeyMember = c.IsKeyMember,
                        Requred = c.Requred,

                        SomeParticipants = c.SomeParticipants,
                        AllRequred = c.AllRequred,

                        AutoSearch = c.AutoSearch,
                        HasAgreement = c.HasAgreement,
                        HasReview = c.HasReview,

                        Description = c.Description,
                        Members = new(),
                    };

                    step.Members = c.Members.Select(m => NewMember(step, m)).ToList();

                    route.Steps.Add(step);
                }
                else
                {
                    var r = route.Steps.FirstOrDefault(f => f.Id == c.Id);
                    if (r != null)
                    {
                        r.IsDeleted = false;
                        r.StageNumber = c.StageNumber;
                        r.Number = c.Number;

                        r.ActType = c.ActType;
                        r.MemberGroup = c.MemberGroup;

                        r.OrgType = c.OrgType;
                        r.OrgId = c.OrgId;

                        r.IsKeyMember = c.IsKeyMember;
                        r.Requred = c.Requred;

                        r.SomeParticipants = c.SomeParticipants;
                        r.AllRequred = c.AllRequred;

                        r.AutoSearch = c.AutoSearch;
                        r.HasAgreement = c.HasAgreement;
                        r.HasReview = c.HasReview;

                        r.Description = c.Description;
                    }

                    r.Members.RemoveAll(mr => !c.Members.Exists(mc =>
                        (mc.UserId == mr.UserId && mc.IsAdditional == mr.IsAdditional && mc.Act == mr.Act)));

                    c.Members.ForEach(mc =>
                    {
                        if (!r.Members.Exists(mr => (mc.UserId == mr.UserId && mc.IsAdditional == mr.IsAdditional && mc.Act == mr.Act)))
                        {
                            r.Members.Add(NewMember(r, mc));
                        }
                    });
                }
            });

            route.Parses.RemoveAll(r => !command.Parses.Exists(c => c.PatternType == r.PatternType));
            command.Parses.ForEach(c =>
            {
                var parse = route.Parses.FirstOrDefault(r => r.PatternType == c.PatternType);

                if (parse is null)
                {
                    route.Parses.Add(new(route, c.PatternType, c.Pattern, c.ValueType));
                }
                else
                {
                    parse.PatternType = c.PatternType;
                    parse.Pattern = c.Pattern;
                    parse.ValueType = c.ValueType;
                }
            });

            await _unitOfWork.Repository<Route>().UpdateAsync(route);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<int>.SuccessAsync(route?.Id ?? 0, _localizer["Route Updated"]);
        }

        private static RouteStepMember NewMember(RouteStep s, RouteStepMemberCommand m)
        {
            return new()
            {
                Step = s,
                Act = m.Act,
                IsAdditional = m.IsAdditional,
                UserId = m.UserId
            };
        }
    }
}
