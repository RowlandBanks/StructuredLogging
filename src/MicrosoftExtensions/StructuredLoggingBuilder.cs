using System;
using System.Text.Json;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    internal class StructuredLoggingBuilder : IStructuredLoggingBuilder
    {
        private readonly JsonSerializerOptions _options;

        public StructuredLoggingBuilder(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IStructuredLoggingBuilder WithJsonConverter(Action<IJsonConverterBuilder> configure)
        {
            var builder = new JsonConverterBuilder(_options);
            configure(builder);
            return this;
        }
    }
}
