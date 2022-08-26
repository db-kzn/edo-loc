using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteFileParseModel
    {
        public int RouteId { get; set; }                                          // Внешний индекс
        public ParsePatterns PatternType { get; set; } = ParsePatterns.Undefined; // Тип шаблона разбора

        public string Pattern { get; set; } = string.Empty;                       // Значение паттерна для Regex
        public ValueTypes ValueType { get; set; } = ValueTypes.String;            // Тип значения результата

        public RouteFileParseModel() { }
        public RouteFileParseModel(RouteFileParse p)
        {
            RouteId = p.RouteId;
            PatternType = p.PatternType;

            Pattern = p.Pattern;
            ValueType = p.ValueType;
        }
    }
}
