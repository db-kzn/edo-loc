namespace EDO_FOMS.Domain.Enums
{
    public enum ParsePatterns : int
    {
        Undefined = 0,     // Паттерн не определен
        Sample = 1,        // Исходный образец
        Mask = 2,          // Маска файла
        Accept = 3,        // Принимаемые типы файлов

        DocTitle = 4,      // Наименование документа
        DocNumber = 5,     // Номер документа
        DocDate = 6,       // Дата документа
        DocNotes = 7,      // Заметки к документу

        CodeMO = 8,        // Код МО
        CodeSMO = 9,       // Код СМО
        CodeFund = 10,     // Код Фонда
        CodeMEO = 11,      // Код Военкомата
        CodeTreasury = 12  // Код Казначейства
    }
}
