using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Domain.Entities.Dir;

public class Route : AuditableEntity<int>
{
    public string Name { get; set; }                   // + Наименование маршрута
    public string Description { get; set; }            // + Описание маршрута

    public DocumentType[] DocTypes { get; set; }       // + Типы документов для которых предназначен маршрут
    public OrgTypes[] ForOrgTypes { get; set; }        // + Типы организаций которые могут использовать маршрут
    public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
    public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

    public bool IsPackage { get; set; }                // + Является пакетом документов, а не единичным файлом
    public bool CalcHash { get; set; }                 // + Рассчитывать хэш документа
    public bool AttachedSign { get; set; }             // + Прикрепленная подпись руководителя
    public bool DisplayedSign { get; set; }            // + Отображаемая подпись руководителя

    public bool IsActive { get; set; }                 // - Используемый маршрут
    public bool UseVersioning { get; set; }            // - Используется версионность
    public bool AllowRevocation { get; set; }          // - Возможность отзывать документ с маршрута
    public RouteStage[] Stages { get; set; }    // + Стадии текущего маршрута
}
