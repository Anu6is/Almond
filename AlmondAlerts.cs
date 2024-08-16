using Almond.Contracts;
using Microsoft.Extensions.Logging;

namespace Almond;

internal sealed class AlmondAlerts(IEmailService emailService, INotificationService notificationService, ILogger<AlmondAlerts> logger)
{
    internal async ValueTask ExecuteAsync()
    {
        if (!await emailService.InitializeAsync()) return;

        var emails = await emailService.FetchEmailsAsync();

        logger.LogInformation("Retrieved {count} alert(s)", emails.Length);

        if (emails.Length == 0) return;

        var published = await notificationService.PublishAsync(emails);

        logger.LogInformation("Published {count} alert(s)", published.Count());

        foreach (var email in published)
            await emailService.MarkAsReadAsync(email.Id);

        return;
    }
}