﻿namespace EDO_FOMS.Domain.Enums
{
    public enum AgreementStates : int
    {
        Error = -2,     // 6. Ошибка обработки
        AllActive = -1, // -. Фильтр активных, без отмененных, удаленных

        Undefined = 0,  // -. Не определен
        Control = 1,    // -. Контроль прохождения по маршруту
        Expecting = 2,  // -. Ожидание. Согласование назначено, очередь не дошла

        Incoming = 3,   // 1. Входящий - Новый поступивший
        Received = 4,   // 2. Полученный - в работе
        Opened = 5,     // 2. Прочитанный - в работе

        Verifed = 6,    // 3+ Доп.согласовант согласился
        Approved = 7,   // 3. Согласованный
        Signed = 8,     // 3. Подписанный

        Refused = 9,    // 4+ Доп.согласовант отказался
        Rejected = 10,  // 4. Отклоненный
        Canceled = 11,  // 5. Отмененное согласование
                        // 5+ Agreement.IsCanceled для сохранения отмененного статуса

        Deleted = 12,
        DocInArchive = 13
    }
}