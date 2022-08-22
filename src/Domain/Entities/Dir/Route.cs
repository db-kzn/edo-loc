using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Domain.Entities.Dir;

public class Route : AuditableEntity<int>
{
    public virtual List<DocumentType> DocTypes { get; set; } = new();        // + Типы документов для которых предназначен маршрут
    public virtual List<RouteDocType> RouteDocTypes { get; set; } = new();   // + Промежуточная таблица * <=> *
    public virtual List<RouteOrgType> ForOrgTypes { get; set; } = new();     // + Типы организаций которые могут использовать маршрут

    public virtual List<RouteStage> Stages { get; set; } = new();            // + Стадии текущего маршрута
    public virtual List<RouteStep> Steps { get; set; } = new();              // + Процессы этапы

    // Id
    public int Number { get; set; }                                          // - Порядковый номер маршрута, для сортировки
    public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
    public string Description { get; set; } = string.Empty;                  // + Описание маршрута

    public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
    public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

    public bool IsPackage { get; set; } = false;                             // + Является пакетом документов, а не единичным файлом
    public bool CalcHash { get; set; } = false;                              // + Рассчитывать хэш документа
    public bool AttachedSign { get; set; } = false;                          // + Прикрепленная подпись руководителя
    public bool DisplayedSign { get; set; } = false;                         // + Отображаемая подпись руководителя

    public bool IsActive { get; set; } = true;                               // - Используемый маршрут
    public bool AllowRevocation { get; set; } = true;                        // - Возможность отзывать документ с маршрута
    public bool UseVersioning { get; set; } = false;                         // - Используется версионность
    public bool HasDetails { get; set; } = false;                            // - Отображать параметры этапов
}
