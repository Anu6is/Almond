namespace Almond.Contracts;

public record Email(string Id, string Body);
public record Notification(string Type, string Title, string Body, string Url, string Email);
public record AppLinkProperties(string Amount, string Title, string Account);