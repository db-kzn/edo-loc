namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteFileParse // Правила разбора имени файла
    {
        public int Id { get; set; }                        // Ключ
        public int RouteId { get; set; }                   // Внешний индекс к маршруту

        public string Name { get; set; }                   // Наименования правила разбора
    }
}
