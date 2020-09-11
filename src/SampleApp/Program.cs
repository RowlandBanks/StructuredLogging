using System;
using Arbee.StructuredLogging.MicrosoftExtensions.Extensions;
using Arbee.StructuredLogging.SampleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .AddHostedService<LoggingService>();
                })
                .UseSerilog((context, serviceProvider, configure) =>
                {
                    configure
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: "SERILOG {Level:u3}: {Message}{NewLine}");
                })
                .ConfigureLogging(builder =>
                {
                    builder
                        .AddConsole()
                        .AddStructuredLogging();
                })
                .Build();

            host.Run();
        }
    }
}
