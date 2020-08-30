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
using System.IO;
using Microsoft.Extensions.Logging.Console;
using System.Linq;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    /// <summary>
    /// Tests to demonstrate that logging to the inbuilt <see cref="ConsoleLogger"/>
    /// is formatted correctly.
    /// </summary>
    public class ConsoleLoggerTests
    {
        private readonly ITestOutputHelper _output;

        public ConsoleLoggerTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void LogsAnonymousEvent()
        {
            // Arrange
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                Console.SetOut(writer);
                var loggerProvider = new TestLoggerProvider();
                var services = new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder
                            .AddConsole(o => o.Format = ConsoleLoggerFormat.Systemd)
                            .AddStructuredLogging();
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
                services.Dispose();
            }

            string message = GetRawJsonFromConsoleOutput(sb).Single();
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
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                Console.SetOut(writer);
                // Assert
                var loggerProvider = new TestLoggerProvider();
                var services = new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder
                            .AddConsole(o => o.Format = ConsoleLoggerFormat.Systemd)
                            .AddStructuredLogging();
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
                services.Dispose();
            }

            // Gather the output
            string message = GetRawJsonFromConsoleOutput(sb).Single();
            _output.WriteLine(message);
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

            // Assert
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                Console.SetOut(writer);
                var loggerProvider = new TestLoggerProvider();
                var services = new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder
                            .AddConsole(o => o.Format = ConsoleLoggerFormat.Systemd)
                            .AddStructuredLogging();
                    })
                    .BuildServiceProvider();

                var logger = services.GetRequiredService<ILogger<LoggerExtensionsTests>>();

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
                    services.Dispose();
                }
            }

            // Gather the output
            string message = GetRawJsonFromConsoleOutput(sb).Single();
            _output.WriteLine(message);
            var json = JObject.Parse(message);

            // Assert
            // Prove that the scoped logging, and the in-scope logging works.
            json.Value<string>("Application").Equals("my-service");
            json["State"].Value<string>("Name").Equals("Grace Hopper");
        }

        private static IEnumerable<string> GetRawJsonFromConsoleOutput(StringBuilder sb)
        {
            // Gather the output
            return sb.ToString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { ']' }, 2)[1].Trim());
        }
    }
}
