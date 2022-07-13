namespace EDO_FOMS.Domain.Enums
{
    public enum UserBaseRoles : int
    {
        // System
        Undefined = 0, // Роль не определена
        Admin = 1,     // Aдминистратор ЭДО
        Chief = 2,     // Руководитель
        Manager = 3,   // Управляющий управляет сотрудниками
        Employee = 4,  // Сотрудник с правом подписи
        User = 5       // Пользователь без права подписи
    }
}
