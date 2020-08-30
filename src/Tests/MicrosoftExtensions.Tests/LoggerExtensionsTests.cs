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
using Xunit.Abstractions;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    public class LoggerExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public LoggerExtensionsTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void LogsAnonymousEvent()
        {
            // Arrange
            var loggerProvider = GetLogger(out var logger);

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
            _output.WriteLine(message);
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

        [Fact]
        public void LogsCallingMethod()
        {
            // Arrange
            var loggerProvider = GetLogger(out var logger);

            // An anonymous object that represents some log state.
            var logState = new
            {
                Name = "Grace Hopper"
            };

            // Act
            logger.Log(LogLevel.Information, logState);

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(JObject.Parse(message).ToString());
            var json = JObject.Parse(message)["Caller"];

            // Assert
            // Prove that the key logging fields are set correctly.
            json.Value<string>("Member").Equals(nameof(LogsCallingMethod));
            json.Value<string>("File").Equals("LoggerExtensionsTests");
            // We can't prove the exact line number as it will vary based on
            // compilation options, so we just prove it's greater than 1.
            json.Value<int>("Line").Should().BeGreaterThan(1);
        }

        [Fact]
        public void LogsScope()
        {
            // Background: Proves that scope variables are logged.

            // Arrange
           var loggerProvider = GetLogger(out var logger);

            using (logger.BeginScope(new
            {
                Application = "my-service"
            }))
            {
                // An anonymous object that represents some log state.
                var logState = new
                {
                    Name = "Grace Hopper"
                };

                // Act
                logger.Log(LogLevel.Information, logState);
            }

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the scoped logging, and the in-scope logging works.
            json.Value<string>("Application").Equals("my-service");
            json["State"].Value<string>("Name").Equals("Grace Hopper");
        }

        private static TestLoggerProvider GetLogger(out ILogger<LoggerExtensionsTests> logger)
        {
            var loggerProvider = new TestLoggerProvider();
            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder
                        .ClearProviders()
                        .AddProvider(loggerProvider)
                        .AddStructuredLogging();
                })
                .BuildServiceProvider();

            logger = services.GetRequiredService<ILogger<LoggerExtensionsTests>>();

            return loggerProvider;
        }
    }
}
