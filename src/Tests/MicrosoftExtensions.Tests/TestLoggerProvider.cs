using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    internal class TestLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, TestLogger> _loggers =
            new ConcurrentDictionary<string, TestLogger>();

        private IExternalScopeProvider _scopeProvider;

        /// <summary>
        /// Provides access to the generated loggers, for verifying tests.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestLogger this[string category] =>
            _loggers.TryGetValue(category, out var logger) ?
                logger :
                throw new Exception($"Logger '{category}' was never instantiated");

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, InstantiateLogger);
        }

        private TestLogger InstantiateLogger(string categoryName)
        {
            return new TestLogger
            {
                ScopeProvider = _scopeProvider
            };
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void Dispose()
        {
        }
    }
}
