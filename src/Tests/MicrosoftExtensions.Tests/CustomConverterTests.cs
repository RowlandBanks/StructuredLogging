using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Arbee.StructuredLogging.MicrosoftExtensions.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Tests
{
    public class CustomConverterTests
    {
        [Fact]
        public void UserDefinedConverters()
        {
            // Background: Prove that the user can supply customer JsonConverters
            // Arrange
            var loggerProvider = new TestLoggerProvider();
            var services = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder
                        .ClearProviders()
                        .AddProvider(loggerProvider)
                        .AddStructuredLogging(structuredLoggingBuilder =>
                        {
                            structuredLoggingBuilder
                                .WithJsonConverter(converterBuilder =>
                                {
                                    converterBuilder.ConfigureOptions(jsonConverterOptions =>
                                    {
                                        jsonConverterOptions.Converters.Add(new StringReverseConverter());
                                    });
                                });
                        });
                })
                .BuildServiceProvider();

            var logger = services.GetRequiredService<ILogger<CustomConverterTests>>();

            // An anonymous object that represents some log state.
            var logState = new
            {
                Name = "Grace Hopper"
            };

            // Act
            logger.Log(LogLevel.Information, logState);

            // Gather the output
            var message = loggerProvider[typeof(CustomConverterTests).FullName].Messages.Single();
            var json = JObject.Parse(message);

            // Assert
            // Prove that the converter was used to reverse the string.
            json["State"].Value<string>("Name").Should().Be("reppoH ecarG");
        }

        public class StringReverseConverter : JsonConverter<string>
        {
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(new string(value.Reverse().ToArray()));
            }
        }
    }
}
