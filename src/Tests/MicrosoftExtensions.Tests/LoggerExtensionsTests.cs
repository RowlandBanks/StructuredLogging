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
using System.Linq;
using System.IO;

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

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the key logging fields are set correctly.
            json.Value<DateTime>("Timestamp")
                .Should()
                .BeWithin(TimeSpan.FromSeconds(10))
                .Before(DateTime.UtcNow);
            json.Value<int>("Id").Should().Be(1);
            json.Value<string>("Name").Should().Be("LogEvent");
            json.Value<string>("Level").Should().Be("Information");
            json["State"].Value<string>("Name").Should().Be("Grace Hopper");
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
            json.Value<string>("Member").Should().Be(nameof(LogsCallingMethod));
            json.Value<string>("File").Should().EndWith($"{Path.DirectorySeparatorChar}LoggerExtensionsTests.cs");
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
            json.Value<string>("Application").Should().Be("my-service");
            json["State"].Value<string>("Name").Should().Be("Grace Hopper");
        }

        [Fact]
        public void LogInformation()
        {
            // Background: Proves that the LogInformation method logs correctly.

            // Arrange
            var loggerProvider = GetLogger(out var logger);

            // Act
            logger.LogInformation("Favourite fruit: {FavouriteFruit}", "Bananas");

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the scoped logging, and the in-scope logging works.
            json.Value<string>("FavouriteFruit").Should().Be("Bananas");
            json.Value<string>("Message").Should().Be("Favourite fruit: Bananas");
            json.Values().Count().Should().Be(2);
        }

        [Fact]
        public void LogInformation_LogsComplexClasses()
        {
            // Background: Proves that the LogInformation method logs correctly.

            // Arrange
            var loggerProvider = GetLogger(out var logger);

            // Act
            logger.LogInformation("Favourite fruit: {Fruit}", new
            {
                Color = "Yellow",
                Name = "Banana"
            });

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the scoped logging, and the in-scope logging works.
            json["Fruit"].Value<string>("Color").Should().Be("Yellow");
            json["Fruit"].Value<string>("Name").Should().Be("Banana");
            json.Value<string>("Message").Should().Be("Favourite fruit: { Color = Yellow, Name = Banana }");
            json.Values().Count().Should().Be(2);
        }

        [Fact]
        public void LogInformation_DoesNotLogMessage()
        {
            // Background: Proves that the LogInformation method will not
            //             log a field called Message (as that is the reserved
            //             name for the formatted message).

            // Arrange
            var loggerProvider = GetLogger(out var logger);

            // Act
            logger.LogInformation("MyMessage: {Message}", "some_message");

            // Gather the output
            var testLogger = loggerProvider[typeof(LoggerExtensionsTests).FullName];

            var message = Assert.Single(testLogger.Messages);
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the scoped logging, and the in-scope logging works.
            json.Value<string>("Message").Should().Be("MyMessage: some_message");
            json.Values().Count().Should().Be(1);
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
