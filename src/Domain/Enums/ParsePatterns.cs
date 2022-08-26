namespace EDO_FOMS.Domain.Enums
{
    public enum ParsePatterns : int
    {
        Undefined = 0, // Роль не определена
        Sample = 1,    // Исходный образец
        Mask = 2,      // Маска файла

        DocTitle = 3,  // Наименование документа
        DocNumber = 4, // Номер документа
        DocDate = 5,   // Дата документа
        DocNotes = 6,  // Заметки к документу

        CodeMO = 7,    // Код МО
        CodeSMO = 8,   // Код СМО
        CodeFund = 9   // Код Фонда
    }
}
