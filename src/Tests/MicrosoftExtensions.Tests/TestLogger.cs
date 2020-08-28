using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    internal class TestLogger : ILogger
    {
        private readonly IList<string> _messages = new List<string>();

        public IEnumerable<string> Messages => _messages;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _messages.Add(formatter(state, exception));
        }
    }
}
