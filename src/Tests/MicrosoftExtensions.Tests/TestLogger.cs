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
            // Build up a JSON object based on the scoped state.
            var logEvent = new JObject();
            ScopeProvider?.ForEachScope((scope, state) =>
            {
                AddState(logEvent, scope);
            }, state);

            AddState(logEvent, state);
            _messages.Add(logEvent.ToString());
        }

        private void AddState(JObject logEvent, object scope)
        {
            logEvent.Merge(JObject.FromObject(scope));
        }
    }
}
