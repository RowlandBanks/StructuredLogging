using System;
using System.Collections.Generic;
using System.Text;

namespace Arbee.StructuredLogging.Core
{
    /// <summary>
    /// Represnets a logging event.
    /// </summary>
    public interface IEvent<T>
    {
        /// <summary>
        /// The ID of the event.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The log level. It's defined as a string to provide flexibility
        /// when interfacing with arbitrary logging libraries.
        /// </summary>
        string Level { get; }

        /// <summary>
        /// The moment this event was raised.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// The state of the application when this event was raised.
        /// This property gives developers the opportunity to set any state
        /// they would like to capture.
        /// </summary>
        T State { get; }

        string Message { get; }

        Caller Caller { get; set; }
    }
}
