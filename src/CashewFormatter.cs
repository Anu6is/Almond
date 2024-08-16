using Almond.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Almond;

internal sealed class CashewFormatter(IConfiguration configuration, ILogger<CashewFormatter> logger) : INotificationFormatter
{
    private readonly Regex InfoExtractor = new(
        configuration[Configuration.AmountRegex] +
        configuration[Configuration.TitleRegex] +
        configuration[Configuration.AccountRegex],
        RegexOptions.Compiled |
        RegexOptions.Singleline);

    public AppLinkProperties? ExtractAppLinkProperties(string text)
    {
        var accountMapJson = configuration[Configuration.AccountMap];
        var accounts = !string.IsNullOrWhiteSpace(accountMapJson) ? JsonSerializer.Deserialize<Dictionary<string, string>>(accountMapJson)! : [];

        if (accounts.Count == 0)
            logger.LogInformation("No account mappings configured");

        Match match = InfoExtractor.Match(text);

        if (match.Success)
        {
            var account = match.Groups[3].Value;

            account = accounts.TryGetValue(account, out var accountName) ? accountName : account;

            return new AppLinkProperties(match.Groups[1].Value, match.Groups[2].Value, account!);
        }

        return null;

    }
}
