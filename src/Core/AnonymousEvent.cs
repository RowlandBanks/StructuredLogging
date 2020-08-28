using System;

namespace Arbee.StructuredLogging.Core
{
    /// <summary>
    /// An anonymous logging event, to permit simple logging scenarios
    /// with anonymous state objects and similar.
    /// </summary>
    /// <typeparam name="T">The type of the state</typeparam>
    public class AnonymousEvent<T> : IEvent<T>
    {
        public AnonymousEvent(T state, string level)
        {
            State = state;
            Level = level;
        }

        public int Id { get; } = 1;

        public string Name { get; } = "LogEvent";

        public string Level { get; }

        public T State { get; }

        public string Message => "NOT IMPLEMENTED";

        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    }
}
