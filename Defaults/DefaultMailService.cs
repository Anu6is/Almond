using Almond.Contracts;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Almond;

internal class DefaultMailService(IConfiguration configuration, ILogger<DefaultMailService> logger) : IEmailService
{
    private const string UserId = "me";
    private GmailService? _mailService;

    public async ValueTask<bool> InitializeAsync()
    {
        var refreshToken = configuration[Configuration.EmailToken];
        var credentials = await GetGoogleCredentialsAsync();

        if (string.IsNullOrWhiteSpace(refreshToken)) return false;

        await credentials.RefreshTokenAsync(CancellationToken.None);

        InitializeMailService(credentials);

        return true;
    }

    public async ValueTask<Email[]> FetchEmailsAsync()
    {
        EnsureMailServiceInitialized();

        var messages = await FetchAllMessagesAsync();
        var emails = await FetchMessageDetailsAsync(messages);

        return [.. emails];
    }

    public async ValueTask MarkAsReadAsync(string emailId)
    {
        EnsureMailServiceInitialized();

        var request = _mailService!.Users.Messages.Modify(new ModifyMessageRequest() { RemoveLabelIds = ["UNREAD"] }, UserId, emailId);

        await request.ExecuteAsync();

        return;
    }

    private async Task<UserCredential> GetGoogleCredentialsAsync()
    {
        return await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = configuration[Configuration.EmailClientId],
                ClientSecret = configuration[Configuration.EmailClientSecret]
            },
            [GmailService.Scope.GmailModify],
            "user",
            CancellationToken.None,
            new ConfigurationDataStore(configuration));
    }

    private void InitializeMailService(UserCredential credentials)
    {
        _mailService = new GmailService(new BaseClientService.Initializer
        {
            ApplicationName = Configuration.ApplicationName,
            HttpClientInitializer = credentials
        });
    }

    private void EnsureMailServiceInitialized()
    {
        if (_mailService is null)
            throw new InvalidOperationException($"{nameof(DefaultMailService)} must first be initialized.");
    }

    private async Task<List<Message>> FetchAllMessagesAsync()
    {
        var messages = new List<Message>();
        var request = _mailService!.Users.Messages.List(UserId).Configure(r =>
        {
            r.Q = configuration[Configuration.EmailQuery];
            r.IncludeSpamTrash = false;
        });

        do
        {
            try
            {
                var response = await request.ExecuteAsync();
                messages.AddRange(response.Messages);
                request.PageToken = response.NextPageToken;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching messages");
                break;
            }
        } while (!string.IsNullOrEmpty(request.PageToken));

        return messages;
    }

    private async Task<List<Email>> FetchMessageDetailsAsync(List<Message> messages)
    {
        var emails = new List<Email>();

        foreach (var message in messages)
        {
            try
            {
                var emailInfoRequest = _mailService!.Users.Messages.Get("me", message.Id);

                var emailInfoResponse = await emailInfoRequest.ExecuteAsync();

                //var body = DecodeRFC2822Base64Url(emailInfoResponse.Payload.Body.Data);

                emails.Add(new Email(message.Id, emailInfoResponse.Snippet));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching email details for message ID {Id}", message.Id);
            }
        }

        return emails;
    }

    private static string DecodeRFC2822Base64Url(string encodedString)
    {
        string base64 = encodedString.Replace('-', '+').Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] bytes = Convert.FromBase64String(base64);

        string decodedString = Encoding.UTF8.GetString(bytes);

        return decodedString;
    }
}