using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Arbee.StructuredLogging.MicrosoftExtensions.Extensions;
using Newtonsoft.Json;
using System.Text.Json;
using Arbee.StructuredLogging.Core;
using Newtonsoft.Json.Linq;
using FluentAssertions;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    public class LoggerExtensionsTests
    {
        [Fact]
        public void LogsAnonymousEvent()
        {
            // Assert
            var loggerProvider = new TestLoggerProvider();
            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder
                        .ClearProviders()
                        .AddProvider(loggerProvider);
                })
                .BuildServiceProvider();

            var logger = services.GetRequiredService<ILogger<LoggerExtensionsTests>>();

            // An anonymous object that represents some log state.
            var logState = new
            {
                Name = "Grace Hopper"
            };

            // Act
            logger.Log(LogLevel.Information, logState);

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message =  Assert.Single(testLogger.Messages);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the key logging fields are set correctly.
            json.Value<DateTime>("Timestamp")
                .Should()
                .BeWithin(TimeSpan.FromSeconds(10))
                .Before(DateTime.UtcNow);
            json.Value<string>("Id").Equals(1);
            json.Value<string>("Name").Equals("LoggingEvent");
            json.Value<string>("Level").Equals("Information");
            json["State"].Value<string>("Name").Equals("Grace Hopper");
        }
    }
}
