namespace Almond.Contracts;

public interface IEmailService
{
    /// <summary>
    /// Initialized the <see cref="IEmailService"/> implementation
    /// </summary>
    /// <returns><c>true</c> if the initialization succeeds, otherwise <c>false</c>.</returns>
    ValueTask<bool> InitializeAsync();

    /// <summary>
    /// Retrieve email messages
    /// </summary>
    /// <returns>A collection of <see cref="Email"/>s</returns>
    ValueTask<Email[]> FetchEmailsAsync();

    /// <summary>
    /// Mark an email as read
    /// </summary>
    /// <param name="emailId">ID of the <see cref="Email"/> to marks as unread</param>
    ValueTask MarkAsReadAsync(string emailId);
}
