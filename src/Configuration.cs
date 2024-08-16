using Almond.Contracts;
using Google.Apis.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Almond;

public static class Configuration
{
    public const string ApplicationName = "Almond";
    public const string EmailQuery = "EMAIL_QUERY";
    public const string EmailToken = "EMAIL_TOKEN";
    public const string EmailClientId = "EMAIL_CLIENT_ID";
    public const string EmailClientSecret = "EMAIL_CLIENT_SECRET";
    public const string NotificationToken = "NOTIFICATION_TOKEN";
    public const string NotificationDestination = "NOTIFICATION_DESTINATION";
    public const string TitleRegex = "REGEX_PATTERN_TITLE";
    public const string AmountRegex = "REGEX_PATTERN_AMOUNT";
    public const string AccountRegex = "REGEX_PATTERN_ACCOUNT";
    public const string AccountMap = "ACCOUNT_MAP";

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        services.AddSingleton<AlmondAlerts>()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<INotificationFormatter, CashewFormatter>()
                .AddLogging(options => 
                {
                    options.ClearProviders();
                    options.AddConsole();
                });

        return services;
    }

    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, DefaultMailService>()
                .AddSingleton<INotificationService, DefaultNotificationService>()
                .AddHttpClient<INotificationService, DefaultNotificationService>(client =>
                {
                    client.BaseAddress = new Uri(DefaultNotificationService.BaseUrl);
                });

        return services;
    }
}
