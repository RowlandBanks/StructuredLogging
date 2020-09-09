using System;
using Arbee.StructuredLogging.MicrosoftExtensions.Extensions;
using Arbee.StructuredLogging.SampleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

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
                //.UseSerilog((context, serviceProvider, configure) =>
                //{
                //    configure
                //        .WriteTo.Console();
                //})
                .ConfigureLogging(builder =>
                {
                    builder
                        //.ClearProviders()
                        //.AddSerilog()
                        .AddStructuredLogging();
                })
                .Build();

            host.Run();
        }
    }
}
