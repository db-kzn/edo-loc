namespace EDO_FOMS.Domain.Enums
{
    public enum ActTypes : int // Тип процесса
    {
        Undefined = 0,   // Не определено
        Initiation = 1,  // Инициация исполнителем

        Review = 2,      // Рецензирование
        Agreement = 3,   // Согласование
        Signing = 4,     // Подписание

        Executing = 5    // Исполнитель
    }
}
