using DevOpsGPT.Application;
using DevOpsGPT.Services;
using Microsoft.Azure.Functions.Worker;
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
        var env = hostingContext.HostingEnvironment;

        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile("secret.settings.json", optional: true, reloadOnChange: true);

        // Additional configuration sources as needed
    })
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        ConfigureAppServices(services);
    })

    .Build();

host.Run();

static void ConfigureAppServices(IServiceCollection services)
{
    services.AddSingleton<DevOpsConnectorService>(x =>
    {
        var config = x.GetRequiredService<IConfiguration>();

        var token = config[Const.SETTING_DEVOPS_TOKEN];
        var orgName = config[Const.SETTING_DEVOPS_ORG];
        var project = config[Const.SETTING_DEVOPS_PROJECT];

        return new DevOpsConnectorService(token, project, orgName);
    });

    services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
    {
        var options = new OpenApiConfigurationOptions()
        {
            Info = new OpenApiInfo()
            {
                Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                Title = $"{DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle()}",
                Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription(),
                TermsOfService = new Uri("https://andresantacroce.com"),
                Contact = new OpenApiContact()
                {
                    Name = "Andre Santacroce",
                    Email = "info@andresantacroce.com",
                    Url = new Uri("https://andresantacroce.com"),
                }
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