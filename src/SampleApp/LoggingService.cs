using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniGuids;

namespace Arbee.StructuredLogging.SampleApp
{
    internal class LoggingService : BackgroundService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly Faker<UserModel> _testUsers;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testUsers = new Faker<UserModel>()
                .RuleFor(m => m.FirstName, f => f.Name.FirstName())
                .RuleFor(m => m.LastName, f => f.Name.LastName())
                .RuleFor(m => m.DateOfBirth, f => f.Date.Past(80));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var nextUser = _testUsers.Generate();
                    _logger.Log(LogLevel.Information, new
                    {
                        Action = "User Registered",
                        User = nextUser,
                        TransactionId = MiniGuid.NewGuid()
                    });

                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                }
            });
        }
    }
}
