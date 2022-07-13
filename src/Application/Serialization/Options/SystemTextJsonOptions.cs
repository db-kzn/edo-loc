using System.Text.Json;
using EDO_FOMS.Application.Interfaces.Serialization.Options;

namespace EDO_FOMS.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}