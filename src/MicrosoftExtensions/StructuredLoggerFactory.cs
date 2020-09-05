using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    /// <summary>
    /// LoggerFactory suitable for logging <see cref="Core.IEvent"/>s.
    /// </summary>
    internal class StructuredLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerFactory _wrappedFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

        public StructuredLoggerFactory(ILoggerFactory wrappedFactory, JsonSerializerOptions jsonSerializerOptions)
        {
            _wrappedFactory = wrappedFactory ?? throw new ArgumentNullException(nameof(wrappedFactory));
            _jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _wrappedFactory.AddProvider(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new StructuredLogger(categoryName, _jsonSerializerOptions, _wrappedFactory.CreateLogger(categoryName), _scopeProvider);
        }

        public void Dispose()
        {
            _wrappedFactory.Dispose();
        }
    }
}
