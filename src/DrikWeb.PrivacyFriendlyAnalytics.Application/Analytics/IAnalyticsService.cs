namespace DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
public interface IAnalyticsService
{
    Task TrackAsync(TrackAnalyticsEventRequest request, CancellationToken cancellationToken = default);
    Task<AnalyticsDashboardSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
