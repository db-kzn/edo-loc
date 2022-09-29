namespace EDO_FOMS.Domain.Enums
{
    public enum DocIcons : int
    {
        Undefined = 0,   // Не определен
        Description = 1, // Договор
        NoteAdd = 2,     // Доп.соглашение

        Receipt = 3,     // Акт МЭК
        FactCheck = 4,   // Заключение по результатам МЭК
        TableChart = 5,   // Реестр заключений по результатам МЭК

        CalendarToday = 6,
        ContactPage = 7,
        Newspaper = 8,

        EventRepeat = 9,
        HelpCenter = 10,
        AssignmentLate = 11,

        Difference = 12,
        AssignmentTurnedIn = 13
    }
}
