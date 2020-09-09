using System;
using Arbee.StructuredLogging.MicrosoftExtensions.Extensions;
using Arbee.StructuredLogging.SampleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
