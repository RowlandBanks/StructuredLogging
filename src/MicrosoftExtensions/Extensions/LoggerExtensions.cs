using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Arbee.StructuredLogging.Core;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Extensions
{
    /// <summary>
    /// Extension methods on <see cref="ILogger{T}"/>
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs the <paramref name="event"/>.
        /// </summary>
        /// <param name="logger">The logger to log with.</param>
        /// <param name="event">The event to log.</param>
        public static void Log<T>(
            this ILogger logger,
            LogLevel level, T @event,
            [CallerMemberName] string member = null,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int? line = null)
        {
            var anonymousEvent = new AnonymousEvent<T>(@event, level.ToString());

            logger.Log(anonymousEvent, member, file, line);
        }

        public static void Log<T>(
            this ILogger logger,
            IEvent<T> @event,
            [CallerMemberName] string member = null,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int? line = null)
        {
            @event.Caller ??= new Caller();
            @event.Caller.Member ??= member;
            @event.Caller.File ??= file;
            @event.Caller.Line ??= line;

            string SerializeState(IEvent<T> theEvent, Exception e)
            {
                return JsonSerializer.Serialize(theEvent);
            }

            logger.Log(LogLevel.Information, new EventId(1), @event, null, SerializeState);
        }
    }
}
