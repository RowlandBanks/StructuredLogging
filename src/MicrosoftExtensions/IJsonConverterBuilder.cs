using System;
using System.Text.Json;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    public interface IJsonConverterBuilder
    {
        void ConfigureOptions(Action<JsonSerializerOptions> options);
    }
}
