using Almond.Contracts;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Almond;

internal sealed class CashewFormatter(IConfiguration configuration) : INotificationFormatter
{
    private readonly Regex InfoExtractor = new(
        configuration[Configuration.AmountRegex] +
        configuration[Configuration.TitleRegex] +
        configuration[Configuration.AccountRegex],
        RegexOptions.Compiled |
        RegexOptions.Singleline);

    public AppLinkProperties? ExtractAppLinkProperties(string text)
    {
        var accounts = configuration.GetSection(Configuration.AccountMap)
                                    .GetChildren()
                                    .ToDictionary(x => x.Key, x => x.Value);

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
