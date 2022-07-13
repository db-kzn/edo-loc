﻿namespace EDO_FOMS.Domain.Enums
{
    public enum DocStages : int
    {
        Error = -2,     // 9. Ошибка обработки
        Undefined = -1, // -. Не определен
        AllActive = 0,  // -. Фильтр активных, без отмененных и удаленных

        Draft = 1,      // 7. Черновик
        InProgress = 2, // 8. Отправленный - На подписании

        Rejected = 3,   // 10. Отклоненный / отозванный документ
        Agreed = 4,     // 11. Согласованный и подписанный всеми
        Archive = 5,    // 11+ Перемещен в архив

        Canceled = 6,   // 12. Отмененный процесс согласования и подписания
        Deleted = 7     // 12+ В корзине, помечен как удаленный
    }
}
