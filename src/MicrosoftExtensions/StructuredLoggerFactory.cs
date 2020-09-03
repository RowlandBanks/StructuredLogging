using System;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    /// <summary>
    /// LoggerFactory suitable for logging <see cref="Core.IEvent"/>s.
    /// </summary>
    internal class StructuredLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerFactory _wrappedFactory;
        private readonly IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

        public StructuredLoggerFactory(ILoggerFactory wrappedFactory)
        {
            _wrappedFactory = wrappedFactory ?? throw new ArgumentNullException(nameof(wrappedFactory));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _wrappedFactory.AddProvider(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new StructuredLogger(categoryName, _wrappedFactory.CreateLogger(categoryName), _scopeProvider);
        }

        public void Dispose()
        {
            _wrappedFactory.Dispose();
        }
    }
}
