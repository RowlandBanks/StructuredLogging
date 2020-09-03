using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Arbee.StructuredLogging.MicrosoftExtensions.JsonConverters;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions
{
    internal class StructuredLogger : ILogger
    {
        private readonly string _category;
        private readonly ILogger _wrappedLogger;
        private readonly IExternalScopeProvider _scopeProvider;

        private readonly JsonSerializerOptions _options;

        public StructuredLogger(
            string category,
            ILogger wrappedLogger,
            IExternalScopeProvider scopeProvider)
        {
            _category = category;
            _wrappedLogger = wrappedLogger ?? throw new ArgumentNullException(nameof(wrappedLogger));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));

            _options = new JsonSerializerOptions();
            _options.Converters.Add(new FormattedLogValuesConverter());
            _options.Converters.Add(new ExceptionConverter());
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // This instance takes care of handling scope for the wrapped logger.
            // This is to ensure that loggers like the ConsoleLogger will be able to log
            // the correct event JSON - by default the console logger will log "<scope1> => <scope2> => <message>",
            // which is no good.
            return _scopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _wrappedLogger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // We provide a formatter to create the log message, INCLUDING the scope.
            // We ignore the supplied formatter, and create our own in order to
            // ensure that output is always JSON - we are doing structured logging,
            // so a hierarchical KVP strucuture such as JSON is the only sensible format
            // to use.
            //
            // Do as much evaluation in the Serialize callback as possible,
            // to avoid doing any work if logging isn't enabled for this log level.
            // We use a lazy to ensure that we evaluate the log event only once,
            // regardless of how many actual loggers there are.
            var lazyJson = new Lazy<string>(() =>
            {
                var json = "{}";
                _scopeProvider.ForEachScope((scopeObject, state) =>
                {
                    json = JsonMerge.Merge(json, JsonSerializer.Serialize(scopeObject));
                }, state);

                if (exception != null)
                {
                    json = JsonMerge.Merge(json, JsonSerializer.Serialize(new { Exception = exception }, _options));
                }

                json = JsonMerge.Merge(json, JsonSerializer.Serialize(new { Category = _category}, _options));

                return JsonMerge.Merge(json, JsonSerializer.Serialize(state, _options));
            });

            string Serialize(TState theState, Exception e)
            {
                return lazyJson.Value;
            };

            _wrappedLogger.Log(logLevel, eventId, state, exception, Serialize);
        }
    }
}
