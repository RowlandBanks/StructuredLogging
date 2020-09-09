using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Arbee.StructuredLogging.SampleApp
{
    internal class LoggingService : BackgroundService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.Log(LogLevel.Information, new
                    {
                        Message = "User Registered"
                    });

                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                }
            });
        }
    }
}
