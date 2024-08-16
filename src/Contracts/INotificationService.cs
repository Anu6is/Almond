namespace Almond.Contracts;

public interface INotificationService
{
    /// <summary>
    /// Publishes a collection of emails asynchronously.
    /// </summary>
    /// <param name="emails">An array of <see cref="Email"/> objects to be published.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of successfully published <see cref="Email"/> objects.
    /// </returns>
    ValueTask<IEnumerable<Email>> PublishAsync(Email[] emails);
}
