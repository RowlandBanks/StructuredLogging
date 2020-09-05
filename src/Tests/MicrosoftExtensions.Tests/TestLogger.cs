using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    internal class TestLogger : ILogger
    {
        private readonly IList<string> _messages = new List<string>();

        public IEnumerable<string> Messages => _messages;

        public IExternalScopeProvider ScopeProvider { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _messages.Add(formatter(state, exception));
        }

        internal object Single()
        {
            throw new NotImplementedException();
        }
    }
}
