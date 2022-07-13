using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EDO_FOMS.Application.Serialization.JsonConverters
{
    /// <summary>
    /// Trim spaces
    /// </summary>
    public class TrimmingJsonConverter : JsonConverter<string>
    {
        /// <summary>
        /// Trim the input string
        /// </summary>
        /// <param name="reader">reader</param>
        /// <param name="typeToConvert">Object type</param>
        /// <param name="options">Existing Value</param>
        /// <returns></returns>
        public override string Read(
          ref Utf8JsonReader reader,
          Type typeToConvert,
          JsonSerializerOptions options) => reader.GetString()?.Trim();

        /// <summary>
        /// Trim the output string
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="value">value</param>
        /// <param name="options">serializer</param>
        public override void Write(
            Utf8JsonWriter writer,
            string value,
            JsonSerializerOptions options) => writer.WriteStringValue(value?.Trim());
    }
}
