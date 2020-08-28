using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public static void Log<T>(this ILogger logger, LogLevel level, T @event)
        {
            var anonymousEvent = new AnonymousEvent<T>(@event, level.ToString());

            logger.Log(anonymousEvent);
        }

        public static void Log<T>(this ILogger logger, IEvent<T> @event)
        {
            logger.Log(LogLevel.Information, new EventId(1), @event, null, (state, exception) => JsonSerializer.Serialize(state));
        }
    }
}
