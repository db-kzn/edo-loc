namespace EDO_FOMS.Domain.Enums
{
    public enum MemberGroups : int
    {
        Undefined = 0,          // Группа участников не определена
        OnlyHead = 1,           // Только руководитель
        HeadAndAccountant = 2,  // Руководитель и главный бухгалтер
        OrgAdmin = 3,           // Администратор организации
        OnlyEmployees = 4,      // Только сотрудники, не руководство
        ExtExpert = 5           // Внешние специалисты
    }
}
