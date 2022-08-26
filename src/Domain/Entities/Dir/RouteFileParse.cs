using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteFileParse
    {
        public int RouteId { get; set; }                                          // Внешний индекс
        public Route Route { get; set; }                                          // Навигационное поле
        public ParsePatterns PatternType { get; set; } = ParsePatterns.Undefined; // Тип шаблона разбора

        public string Pattern { get; set; } = string.Empty;                       // Значение паттерна для Regex
        public ValueTypes ValueType { get; set; } = ValueTypes.String;            // Тип значения результата

        public RouteFileParse() { }
        public RouteFileParse(
            Route route,
            ParsePatterns patternType,
            string pattern,
            ValueTypes valueType
            )
        {
            Route = route;
            PatternType = patternType;
            Pattern = pattern;
            ValueType = valueType;
        }
    }
}
