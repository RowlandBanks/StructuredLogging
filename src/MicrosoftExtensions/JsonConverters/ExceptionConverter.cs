using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arbee.StructuredLogging.MicrosoftExtensions.JsonConverters
{
    internal class ExceptionConverter : JsonConverter<Exception>
    {
        private static readonly Type ExceptionType = typeof(Exception);

        public override bool CanConvert(Type typeToConvert)
        {
            return ExceptionType.IsAssignableFrom(typeToConvert);
        }

        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var type = value.GetType();
            writer.WriteString("Name", type.Name);
            writer.WriteString("FullName", type.FullName);
            writer.WriteString("Message", value.Message);
            writer.WriteString("Source", value.Source);
            writer.WriteString("StackTrace", value.StackTrace);
            writer.WriteEndObject();
        }
    }
}
