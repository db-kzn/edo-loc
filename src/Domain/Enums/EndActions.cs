﻿namespace EDO_FOMS.Domain.Enums
{
    public enum EndActions : int
    {
        Undefined = 0,    // Действие не определено

        SignedByAll = 1,  // Подписано всеми - нет доп.действий
        ToArchive = 2,    // Отправить в архив (заархивировать папку документа в один файл)

        ToPublish = 3,    // Опубликовать документ для участников подписания
        ForPickUp = 4     // Сформировать пакеты из групп документов, для массовой выгрузки
    }
}
