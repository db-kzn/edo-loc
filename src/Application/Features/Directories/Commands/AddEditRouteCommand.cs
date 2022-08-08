using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class AddEditRouteCommand : IRequest<Result<int>>
    {
        public List<RouteStageModel> Stages { get; set; }            // + Стадии текущего маршрута
        public List<RouteStageStepModel> Steps { get; set; }         // + Процессы этапы

        public int Id { get; set; }                             // - Идентификатор маршрута
        public int Number { get; set; }                         // - Ценность маршрута, для сортировки
        public string Name { get; set; } = string.Empty;        // + Наименование маршрута
        public string Description { get; set; } = string.Empty; // + Описание маршрута

        public List<DocumentType> DocTypes { get; set; }        // + Типы документов для которых предназначен маршрут
        public List<OrgTypes> ForOrgTypes { get; set; }         // + Типы организаций которые могут использовать маршрут
        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
        public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

        public bool IsPackage { get; set; } = false;            // + Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;             // + Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;         // + Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;        // + Отображаемая подпись руководителя

        public bool IsActive { get; set; } = true;              // - Используемый маршрут
        public bool AllowRevocation { get; set; } = true;       // - Возможность отзывать документ с маршрута
        public bool UseVersioning { get; set; } = false;        // - Используется версионность
    }

    public class RouteStageModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }                   // - Внешний индекс
        public virtual Route Route { get; set; }

        public int Number { get; set; }                    // + Номер этапа в цепочке маршрута

        public MudBlazor.Color Color { get; set; }         // + Цвет формы
        public string Name { get; set; }                   // + Наименование этапа
        public string Description { get; set; }            // - Описание этапа

        public ActTypes ActType { get; set; }              // - Тип этапа: неопределенный, подписание, согласование или рецензирование
        public bool InSeries { get; set; } = false;        // - Последовательное прохождение
        public bool AllRequred { get; set; } = true;       // - Если параллельно, то требуются все

        public bool DenyRevocation { get; set; }           // - Возможность отзывать документ с маршрута
        public TimeSpan Validity { get; set; }             // - Срок на прохождение этапа

        //public List<RouteStageStep> Steps { get; set; }    // + Процессы этапы
    }

    public class RouteStageStepModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }                          // - Внешний индекс
        public virtual Route Route { get; set; }

        public int StageNumber { get; set; }                      // + Номер этапа
        public int Number { get; set; }                           // - Номер процесса в этапе, для сортировки последовательности

        public ActTypes ActType { get; set; } = ActTypes.Signing; // + Тип шага: подписание, согласование или рецензирование
        public OrgTypes OrgType { get; set; }                     // + Тип организации, может быть не определен
        public bool OnlyHead { get; set; }                        // + Требуется руководитель

        public bool Requred { get; set; } = true;                 // + Обязательный шаг
        public bool SomeParticipants { get; set; } = true;        // - Несколько участников
        public bool AllRequred { get; set; } = true;              // + Если несколько, то условие завершения: все или любой

        public bool HasAgreement { get; set; } = false;           // + Содержит согласование
        public bool HasReview { get; set; } = false;              // + Содержит рецензирование
    }
}
