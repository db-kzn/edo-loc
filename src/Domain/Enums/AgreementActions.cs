namespace EDO_FOMS.Domain.Enums
{
    public enum AgreementActions : int
    {
        Undefined = 0, // Не определено
        ToRun = 1,     // Запустить

        ToVerify = 2,  // Проверить - доп.согласовантом
        ToApprove = 3, // Согласовать
        ToSign = 4,    // Подписать

        ToRefuse = 5, // Забраковать - доп.согласовантом
        ToReject = 6,  // Отклонить - согласовантом или подписантом
        AddMembers = 8 // Добавить согласовантов
    }
}
