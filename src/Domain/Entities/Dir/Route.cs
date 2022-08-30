using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Dir;

public class Route : AuditableEntity<int>
{
    public List<DocumentType> DocTypes { get; set; } = new();                // + Типы документов для которых предназначен маршрут
    public List<RouteDocType> RouteDocTypes { get; set; } = new();           // + Промежуточная таблица * <=> *
    public List<RouteOrgType> ForOrgTypes { get; set; } = new();             // + Типы организаций которые могут использовать маршрут

    public List<RouteStage> Stages { get; set; } = new();                    // + Стадии текущего маршрута
    public List<RouteStep> Steps { get; set; } = new();                      // + Процессы этапы
    public List<RouteFileParse> Parses { get; set; } = new();                // + Правила разбора имени файла
    public List<RoutePacketFile> Files { get; set; } = null;                 // - Типы файлов из пакета

    // Id
    public int Number { get; set; }                                          // - Порядковый номер маршрута, для сортировки
    [MaxLength(10)]
    public string Code { get; set; }                                         //   Уникальное текстовое поле

    [MaxLength(50)]
    public string Short { get; set; } = string.Empty;                        // + Краткое наименование маршрута
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;                  // + Описание маршрута (комментарий)

    public string ExecutorId { get; set; } = string.Empty;                   // + Исполнитель
    public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль инициатора имеющая доступ к маршруту
    public EndActions EndAction { get; set; } = EndActions.SignedByAll;      // + Действие по завершению маршрута

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

    public bool HasDetails { get; set; } = false;                            // - Отображать параметры этапов
}
