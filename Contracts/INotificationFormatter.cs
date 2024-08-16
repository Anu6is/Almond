namespace Almond.Contracts;

public interface INotificationFormatter
{
    private static readonly FormattableString AddTransactionTemplate = $"https://cashewapp.web.app/addTransaction?amount=-{{0}}&title={{1}}&account={{2}}";

    /// <summary>
    /// Gets the properties required for creating an add transaction app link from the text provided
    /// </summary>
    /// <param name="text">The text from which the properties should be extracted</param>
    /// <returns><see cref="AppLinkProperties"/></returns>
    AppLinkProperties? ExtractAppLinkProperties(string text);


    public string CreateAppLink(AppLinkProperties properties)
    {
        return string.Format(AddTransactionTemplate.ToString(),
                             properties.Amount,
                             Uri.EscapeDataString(properties.Title),
                             Uri.EscapeDataString(properties.Account));
    }
}
