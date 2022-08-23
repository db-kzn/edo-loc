using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Domain.Enums;
using System.Collections.Generic;

namespace EDO_FOMS.Client.Models
{
    public class RouteCardModel
    {
        public List<int> DocTypeIds { get; set; } = new();                       // + Типы документов для которых предназначен маршрут
        public List<OrgTypes> ForOrgTypes { get; set; } = new();                 // + Типы организаций которые могут использовать маршрут

        public List<RouteStageModel> Stages { get; set; } = new();               // + Стадии текущего маршрута
        public List<RouteStepModel> Steps { get; set; } = new();                 // + Процессы в этапе + Участники процесса

        public List<RouteFileParse> Parses { get; set; } = new();                // - Правила разбора имени файла

        public int Id { get; set; }                                              // - Идентификатор маршрута
        public int Number { get; set; }                                          // - Ценность маршрута, для сортировки
        public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
        public string Description { get; set; } = string.Empty;                  // + Описание маршрута

        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту
        public EndActions EndAction { get; set; } = EndActions.ToArchive;        // + Действие по завершению маршрута

        public bool IsPackage { get; set; } = false;                             // + Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                              // + Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                          // + Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                         // + Отображаемая подпись руководителя

        public bool IsActive { get; set; } = true;                               // + Используемый маршрут
        public bool ReadOnly { get; set; } = false;                              // + Карточка документа не редактируется
        public bool NameOfFile { get; set; } = true;                             // + Наименование документа из имени файла
        public bool DateIsToday { get; set; } = true;                            // + Дата документа - устанивить сегодня

        public bool AllowRevocation { get; set; } = true;                        // + Возможность отзывать документ с маршрута
        public bool ParseFileName { get; set; } = false;                         // + Разбор имени файла
        public bool UseVersioning { get; set; } = false;                         // - Используется версионность
        public bool HasDetails { get; set; } = false;                            // - Отображать параметры этапов
    }
}
