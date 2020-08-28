﻿using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    internal class TestLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, TestLogger> _loggers =
            new ConcurrentDictionary<string, TestLogger>();

        /// <summary>
        /// Provides access to the generated loggers, for verifying tests.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestLogger this[string category] => _loggers[category];

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, InstantiateLogger);
        }

        private TestLogger InstantiateLogger(string categoryName)
        {
            return new TestLogger();
        }

        public void Dispose()
        {
        }
    }
}