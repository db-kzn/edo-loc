namespace EDO_FOMS.Domain.Enums
{
    public enum ActTypes : int // Тип процесса
    {
        Initiation = -1, // Инициация исполнителем
        Undefined = 0,   // Не определено

        Signing = 1,     // Подписание
        Agreement = 2,   // Согласование
        Review = 3       // Рецензирование
    }
}
