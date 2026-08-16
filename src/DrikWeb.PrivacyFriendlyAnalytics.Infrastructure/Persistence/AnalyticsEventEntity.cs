namespace DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Persistence;
public sealed class AnalyticsEventEntity
{
    public long Id { get; set; }
    public required string EventName { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid SessionId { get; set; }
    public string? PagePath { get; set; }
    public string? PropertiesJson { get; set; }
}
