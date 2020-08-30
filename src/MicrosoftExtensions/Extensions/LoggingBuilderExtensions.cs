using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arbee.StructuredLogging.MicrosoftExtensions.Extensions
{
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Enables structured logging for this <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns><c>builder</c></returns>
        public static ILoggingBuilder AddStructuredLogging(this ILoggingBuilder builder)
        {
            var descriptor = builder.Services.Single(s => s.ServiceType == typeof(ILoggerFactory));

            builder.Services.AddScoped<ILoggerFactory>(provider =>
                new StructuredLoggerFactory(GetInstance<ILoggerFactory>(provider, descriptor)));
            return builder;
        }

        // Method loosely based on Scrutor, MIT licensed: https://github.com/khellang/Scrutor/blob/68787e28376c640589100f974a5b759444d955b3/src/Scrutor/ServiceCollectionExtensions.Decoration.cs#L319
        // See https://stackoverflow.com/a/55601564/15393 for more information.
        private static T GetInstance<T>(IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return (T)descriptor.ImplementationInstance;
            }

            if (descriptor.ImplementationType != null)
            {
                if (descriptor.ImplementationType == typeof(LoggerFactory))
                {
                    // There is a bug (https://github.com/dotnet/aspnetcore/issues/2871) in Activator.CreateUtilities
                    // whereby the first constructor seen is chosen (typically the shortest).
                    // This works the opposite of ServiceProvider, which will choose the longest for which it has parameters.
                    // Therefore, we force it to choose the longest in this scenario.
                    var providers = provider.GetRequiredService<IEnumerable<ILoggerProvider>>();
                    var filterOptions = provider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();

                    return (T)ActivatorUtilities.CreateInstance(
                        provider,
                        descriptor.ImplementationType,
                        new object[] { providers, filterOptions });
                }
                else
                {
                    return (T)ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType);
                }
            }

            if (descriptor.ImplementationFactory != null)
            {
                return (T)descriptor.ImplementationFactory(provider);
            }

            throw new InvalidOperationException($"Could not create instance for {descriptor.ServiceType}");
        }
    }
}
