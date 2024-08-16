using Almond.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Almond;

internal class DefaultNotificationService(HttpClient httpClient,
                                          INotificationFormatter formatter,
                                          IConfiguration configuration,
                                          ILogger<DefaultNotificationService> logger) : INotificationService
{
    private const string PushUrl = "/v2/pushes";
    private JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public const string BaseUrl = "https://api.pushbullet.com";

    public async ValueTask<IEnumerable<Email>> PublishAsync(Email[] emails)
    {
        if (emails is null || emails.Length == 0) return [];

        var accessToken = configuration[Configuration.NotificationToken];

        if (string.IsNullOrWhiteSpace(accessToken)) return [];

        httpClient.DefaultRequestHeaders.Add("Access-Token", accessToken);

        var tasks = emails.Select(PublishNotificationAsync);
        var results = await Task.WhenAll(tasks);

        return results.Where(r => r is not null)
                      .Select(x => x!)
                      .ToArray();
    }

    private async Task<Email?> PublishNotificationAsync(Email email)
    {
        try
        {
            var properties = formatter.ExtractAppLinkProperties(email.Body);

            if (properties is null) return null;

            var notification = CreateNotification(properties);
            var success = await SendNotificationAsync(notification);

            return success ? email : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish notification for email: {EmailId}", email.Id);

            return null;
        }
    }

    private Notification CreateNotification(AppLinkProperties properties) => new(
        "link",
        "New Credit Card Transaction",
        $"Purchase made at {properties.Title}",
        formatter.CreateAppLink(properties),
        configuration[Configuration.NotificationDestination]!
    );

    private async Task<bool> SendNotificationAsync(Notification notification)
    {
        var json = JsonSerializer.Serialize(notification, _serializeOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, PushUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
}
