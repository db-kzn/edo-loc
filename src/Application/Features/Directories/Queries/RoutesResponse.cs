using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Features.Directories.Queries
{
    public class RoutesResponse
    {
        public int Id { get; set; }
        public int Number { get; set; }                                          // - Порядковый номер маршрута, для сортировки
        public string Code { get; set; }

        public string Short { get; set; }
        public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
        public string Description { get; set; } = string.Empty;                  // + Описание маршрута

        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту

        public bool IsActive { get; set; } = true;                               // + Используемый маршрут
        public bool DateIsToday { get; set; } = false;                           // + Дата документа - устанивить сегодня
        public bool NameOfFile { get; set; } = false;                            // + Наименование документа из имени файла
        public bool ParseFileName { get; set; } = false;                         // + Разбор имени файла

        public bool AllowRevocation { get; set; } = true;                        // + Возможность отзывать документ с маршрута
        public bool ProtectedMode { get; set; } = false;                         // + Карточка документа не редактируется
        public bool ShowNotes { get; set; } = false;                             // + Отобразить примечание/заметки
        public bool UseVersioning { get; set; } = false;                         // + Используется версионность

        public bool IsPackage { get; set; } = false;                             // - Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                              // - Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                          // - Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                         // - Отображаемая подпись руководителя

        public bool HasDetails { get; set; } = false;                            // - Отображать параметры этапов
    }
}
