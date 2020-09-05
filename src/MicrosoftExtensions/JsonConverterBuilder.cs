using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    internal class JsonConverterBuilder : IJsonConverterBuilder
    {
        private readonly JsonSerializerOptions _options;

        public JsonConverterBuilder(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public void ConfigureOptions(Action<JsonSerializerOptions> configure)
        {
            configure(_options);
        }
    }
}
