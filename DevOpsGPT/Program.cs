using CommonLib.Logging;
using DevOpsGPT.Services;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile("secret.settings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureOpenApi()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddCommonLogging();

        ConfigureAppServices(services, configuration);
    })

    .Build();

host.Run();

static void ConfigureAppServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<DevOpsConfig>(_ => {
        return configuration.GetRequiredSection("DevOps").Get<DevOpsConfig>();
    });

    services.AddSingleton<DevOpsConnectorService>();

    services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
    {
        var options = new OpenApiConfigurationOptions()
        {
            Info = new OpenApiInfo()
            {
                Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                Title = $"{DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle()}",
                Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription()
            },
            Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
            OpenApiVersion = OpenApiVersionType.V3,
            IncludeRequestingHostName = DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
            ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
            ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced()
        };

        return options;
    });
}