namespace DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
public sealed record AnalyticsDashboardSummary(int TotalEvents,int SessionCount,int WorkflowStartedCount,int WorkflowCompletedCount,decimal CompletionRate,decimal PositiveFeedbackRate,IReadOnlyList<AnalyticsEventCount> EventCounts,IReadOnlyList<RecentAnalyticsEvent> RecentEvents);
public sealed record AnalyticsEventCount(string EventName,int Count);
public sealed record RecentAnalyticsEvent(long Id,string EventName,DateTime OccurredAtUtc,Guid SessionId,string? PagePath);
