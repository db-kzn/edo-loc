using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Features.Directories.Queries
{
    public class RouteCardResponse
    {
        public List<int> DocTypeIds { get; set; } = new();                       // + Типы документов для которых предназначен маршрут
        public List<OrgTypes> ForOrgTypes { get; set; } = new();                 // + Типы организаций которые могут использовать маршрут

        public List<RouteStageModel> Stages { get; set; } = new();               // + Стадии текущего маршрута
        public List<RouteStepModel> Steps { get; set; } = new();                 // + Процессы (шаги) с Участниками
        public List<RouteFileParseModel> Parses { get; set; } = new();           // - Правила разбора имени файла
        public List<RoutePacketFile> Files { get; set; } = null;                 // - Типы файлов из пакета

        public int Id { get; set; }
        public int Number { get; set; }                                          // - Порядковый номер маршрута, для сортировки
        public string Code { get; set; }                                         //   Уникальное текстовое поле

        public string Short { get; set; } = string.Empty;                        // + Краткое наименование маршрута
        public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
        public string Description { get; set; } = string.Empty;                  // + Описание маршрута

        public string ExecutorId { get; set; } = string.Empty;                   // + Исполнитель по UserId
        public ContactResponse Executor { get; set; } = null;                    // + Контакт исполнителя
        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
        public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

        public bool IsActive { get; set; } = true;                               // + Используемый маршрут
        public bool DateIsToday { get; set; } = true;                            // + Дата документа - устанивить сегодня
        public bool NameOfFile { get; set; } = true;                             // + Наименование документа из имени файла
        public bool ParseFileName { get; set; } = false;                         // + Разбор имени файла

        public bool AllowRevocation { get; set; } = true;                        // + Возможность отзывать документ с маршрута
        public bool ProtectedMode { get; set; } = false;                              // + Карточка документа не редактируется
        public bool ShowNotes { get; set; } = false;                             // + Отобразить примечание/заметки
        public bool UseVersioning { get; set; } = false;                         // - Используется версионность

        public bool IsPackage { get; set; } = false;                             // - Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                              // - Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                          // - Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                         // - Отображаемая подпись руководителя

        public bool HasDetails { get; set; } = true;                             // - Отображать параметры этапов
    }
}
