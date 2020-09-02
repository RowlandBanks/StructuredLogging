using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arbee.StructuredLogging.MicrosoftExtensions.JsonConverters
{
    /// <summary>
    /// Supports writing of the internal class Microsoft.Extensions.Logging.FormattedLogValues.
    /// </summary>
    /// <remarks>
    /// This is used to support logging of the format:
    /// <code>
    /// <![CDATA[
    /// logger.LogInformation("Favourite Fruit: {Fruit}", "Bananas");
    /// ]]></code>
    /// </remarks>
    internal class FormattedLogValuesConverter : JsonConverter<IEnumerable<KeyValuePair<string, object>>>
    {
        private static readonly Type BaseType = typeof(IEnumerable<KeyValuePair<string, object>>);

        public override bool CanConvert(Type typeToConvert)
        {
            return BaseType.IsAssignableFrom(typeToConvert) &&
                   typeToConvert.FullName == "Microsoft.Extensions.Logging.FormattedLogValues";
        }

        public override IEnumerable<KeyValuePair<string, object>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<KeyValuePair<string, object>> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kv in value.Where(Loggable))
            {
                writer.WritePropertyName(kv.Key);
                //writer.WriteString(kv.Key, kv.Value?.ToString());
                JsonSerializer.Serialize(writer, kv.Value, options);
            }
            writer.WriteString("Message", value.ToString());
            writer.WriteEndObject();
        }

        private bool Loggable(KeyValuePair<string, object> arg)
        {
            return arg.Key != "Message" && arg.Key != "{OriginalFormat}";
        }
    }
}
