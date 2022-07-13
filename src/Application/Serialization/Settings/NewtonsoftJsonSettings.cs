
using EDO_FOMS.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace EDO_FOMS.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}