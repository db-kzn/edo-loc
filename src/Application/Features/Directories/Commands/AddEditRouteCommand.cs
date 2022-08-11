using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MudBlazor.FilterOperator;

namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class AddEditRouteCommand : IRequest<Result<int>>
    {
        public List<int> DocTypeIds { get; set; } = new();              // + Типы документов для которых предназначен маршрут
        public List<OrgTypes> ForOrgTypes { get; set; } = new();        // + Типы организаций которые могут использовать маршрут
        public List<RouteStageModel> Stages { get; set; } = new();    // + Стадии текущего маршрута
        public List<RouteStageStepModel> Steps { get; set; } = new(); // + Процессы этапы

        public int Id { get; set; }                                     // - Идентификатор маршрута
        public int Number { get; set; }                                 // - Ценность маршрута, для сортировки
        public string Name { get; set; } = string.Empty;                // + Наименование маршрута
        public string Description { get; set; } = string.Empty;         // + Описание маршрута

        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
        public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

        public bool IsPackage { get; set; } = false;                    // + Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                     // + Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                 // + Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                // + Отображаемая подпись руководителя

        public bool IsActive { get; set; } = true;                      // - Используемый маршрут
        public bool AllowRevocation { get; set; } = true;               // - Возможность отзывать документ с маршрута
        public bool UseVersioning { get; set; } = false;                // - Используется версионность
        public bool HasDetails { get; set; } = false;                   // - Отображать параметры этапов
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

            command.Steps.ForEach(step =>
                route.Steps.Add(new RouteStageStep()
                {
                    Route = route,
                    StageNumber = step.StageNumber,
                    Number = step.Number,

                    ActType = step.ActType,
                    OrgType = step.OrgType,
                    OnlyHead = step.OnlyHead,

                    Requred = step.Requred,
                    SomeParticipants = step.SomeParticipants,
                    AllRequred = step.AllRequred,

                    HasAgreement = step.HasAgreement,
                    HasReview = step.HasReview
                })
            );

            await _unitOfWork.Repository<Route>().AddAsync(route);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<int>.SuccessAsync(route.Id, _localizer["Route Saved"]);
        }

        private async Task<Result<int>> EditRouteAsync(AddEditRouteCommand command, CancellationToken cancellationToken)
        {
            var routes = _unitOfWork.Repository<Route>().Entities.Include(r => r.Stages).Include(r => r.Steps).Include(r => r.RouteDocTypes).Include(r => r.ForOrgTypes);
            var route = await routes.FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken: cancellationToken);
            if (route is null) { return await Result<int>.FailAsync(_localizer["Route Not Found!"]); }

            route.Number = command.Number;
            route.Name = command.Name;
            route.Description = command.Description;

            route.ForUserRole = command.ForUserRole;
            route.EndAction = command.EndAction;

            route.IsPackage = command.IsPackage;
            route.CalcHash = command.CalcHash;
            route.AttachedSign = command.AttachedSign;
            route.DisplayedSign = command.DisplayedSign;

            route.IsActive = command.IsActive;
            route.AllowRevocation = command.AllowRevocation;
            route.UseVersioning = command.UseVersioning;
            route.HasDetails = command.HasDetails;

            //List<DocumentType> DocTypes -> List<RouteDocType> RouteDocTypes
            //route.RouteDocTypes.Clear();
            route.RouteDocTypes.RemoveAll(r => !command.DocTypeIds.Exists(id => id == r.DocumentTypeId));
            command.DocTypeIds.ForEach(id =>
            {
                if (!route.RouteDocTypes.Exists(r => r.DocumentTypeId == id))
                {
                    route.RouteDocTypes.Add(new RouteDocType() { Route = route, DocumentTypeId = id });
                }
            });

            //List<RouteOrgType> ForOrgTypes
            //route.ForOrgTypes.Clear();
            route.ForOrgTypes.RemoveAll(r => !command.ForOrgTypes.Exists(t => t == r.OrgType));
            command.ForOrgTypes.ForEach(t =>
            {
                if (!route.ForOrgTypes.Exists(r => r.OrgType == t))
                {
                    route.ForOrgTypes.Add(new RouteOrgType() { Route = route, OrgType = t });
                }
            });

            //List<RouteStage> Stages
            //route.Stages.Clear();
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
                    var s = route.Stages.Find(r => r.Id == c.Id);
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

            //List<RouteStageStep> Steps
            //route.Steps.Clear();
            route.Steps.RemoveAll(r => !command.Steps.Exists(c => c.Id == r.Id));
            command.Steps.ForEach(c =>
            {
                if (c.Id == 0)
                {
                    route.Steps.Add(new RouteStageStep()
                    {
                        Route = route,
                        StageNumber = c.StageNumber,
                        Number = c.Number,

                        ActType = c.ActType,
                        OrgType = c.OrgType,
                        OnlyHead = c.OnlyHead,

                        Requred = c.Requred,
                        SomeParticipants = c.SomeParticipants,
                        AllRequred = c.AllRequred,

                        HasAgreement = c.HasAgreement,
                        HasReview = c.HasReview
                    });
                }
                else
                {
                    var s = route.Steps.Find(r => r.Id == c.Id);
                    if (s != null)
                    {
                        s.StageNumber = c.StageNumber;
                        s.Number = c.Number;

                        s.ActType = c.ActType;
                        s.OrgType = c.OrgType;
                        s.OnlyHead = c.OnlyHead;

                        s.Requred = c.Requred;
                        s.SomeParticipants = c.SomeParticipants;
                        s.AllRequred = c.AllRequred;

                        s.HasAgreement = c.HasAgreement;
                        s.HasReview = c.HasReview;
                    }
                }
            });

            await _unitOfWork.Repository<Route>().UpdateAsync(route);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<int>.SuccessAsync(route.Id, _localizer["Route Updated"]);
        }
    }
}
