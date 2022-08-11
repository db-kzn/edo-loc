using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Client.Models
{
    public class RouteModel
    {
        public int Id { get; set; }

        public int Number { get; set; }                                          // - Порядковый номер маршрута, для сортировки
        public string Name { get; set; } = string.Empty;                         // + Наименование маршрута
        public string Description { get; set; } = string.Empty;                  // + Описание маршрута

        public UserBaseRoles ForUserRole { get; set; } = UserBaseRoles.Employee; // + Минимальная роль пользователя имеющая доступ к маршруту

        public bool IsPackage { get; set; } = false;                             // + Является пакетом документов, а не единичным файлом
        public bool CalcHash { get; set; } = false;                              // + Рассчитывать хэш документа
        public bool AttachedSign { get; set; } = false;                          // + Прикрепленная подпись руководителя
        public bool DisplayedSign { get; set; } = false;                         // + Отображаемая подпись руководителя

        public bool IsActive { get; set; } = true;                               // - Используемый маршрут
        public bool AllowRevocation { get; set; } = true;                        // - Возможность отзывать документ с маршрута
        public bool UseVersioning { get; set; } = false;                         // - Используется версионность
        public bool HasDetails { get; set; } = false;                            // - Отображать параметры этапов
    }
}
