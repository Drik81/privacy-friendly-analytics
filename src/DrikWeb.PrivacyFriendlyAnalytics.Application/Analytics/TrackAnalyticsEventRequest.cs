namespace DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
public sealed record TrackAnalyticsEventRequest(string EventName, Guid SessionId, string? PagePath, IReadOnlyDictionary<string, object?>? Properties);
