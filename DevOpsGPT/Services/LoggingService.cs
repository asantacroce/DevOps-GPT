﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace CommonLib.Logging
{
    public static class LoggingService
    {
        public static IServiceCollection AddCommonLogging(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddApplicationInsights();
            });

            services.Configure<LoggerFilterOptions>(options =>
            {
                // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
                // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
                LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                    == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

                if (toRemove is not null)
                {
                    options.Rules.Remove(toRemove);
                }
            });

            return services;
        }
    }
}