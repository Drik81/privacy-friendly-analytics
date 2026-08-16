using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
using DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Persistence;

namespace DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Analytics;

public sealed class AnalyticsService(AnalyticsDbContext dbContext) : IAnalyticsService
{
    public async Task TrackAsync(
        TrackAnalyticsEventRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var sanitizedProperties = SanitizeProperties(request.Properties);

        var entity = new AnalyticsEventEntity
        {
            EventName = request.EventName,
            OccurredAtUtc = DateTime.UtcNow,
            SessionId = request.SessionId,
            PagePath = NormalizePath(request.PagePath),
            PropertiesJson = sanitizedProperties.Count > 0
                ? JsonSerializer.Serialize(sanitizedProperties)
                : null
        };

        dbContext.AnalyticsEvents.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AnalyticsDashboardSummary> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalEvents = await dbContext.AnalyticsEvents.CountAsync(cancellationToken);

        var sessionCount = await dbContext.AnalyticsEvents
            .Select(x => x.SessionId)
            .Distinct()
            .CountAsync(cancellationToken);

        var started = await dbContext.AnalyticsEvents
            .CountAsync(x => x.EventName == AnalyticsEventNames.WorkflowStarted, cancellationToken);

        var completed = await dbContext.AnalyticsEvents
            .CountAsync(x => x.EventName == AnalyticsEventNames.WorkflowCompleted, cancellationToken);

        var feedbackPayloads = await dbContext.AnalyticsEvents
            .Where(x => x.EventName == AnalyticsEventNames.FeedbackSubmitted && x.PropertiesJson != null)
            .Select(x => x.PropertiesJson!)
            .ToListAsync(cancellationToken);

        var totalFeedback = feedbackPayloads.Count;
        var positiveFeedback = feedbackPayloads.Count(IsPositiveFeedback);

        var eventCountsRaw = await dbContext.AnalyticsEvents
            .GroupBy(x => x.EventName)
            .Select(g => new
            {
                EventName = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        var eventCounts = eventCountsRaw
            .Select(x => new AnalyticsEventCount(x.EventName, x.Count))
            .ToList();

        var recentEvents = await dbContext.AnalyticsEvents
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(25)
            .Select(x => new RecentAnalyticsEvent(
                x.Id, x.EventName, x.OccurredAtUtc, x.SessionId, x.PagePath))
            .ToListAsync(cancellationToken);

        return new AnalyticsDashboardSummary(
            totalEvents,
            sessionCount,
            started,
            completed,
            started == 0 ? 0 : decimal.Round((decimal)completed / started * 100, 1),
            totalFeedback == 0 ? 0 : decimal.Round((decimal)positiveFeedback / totalFeedback * 100, 1),
            eventCounts,
            recentEvents);
    }

    internal static IReadOnlyDictionary<string, object?> SanitizeProperties(
        IReadOnlyDictionary<string, object?>? properties)
    {
        if (properties is null || properties.Count == 0)
            return new Dictionary<string, object?>();

        return properties
            .Take(AnalyticsLimits.MaxPropertyCount)
            .Where(x => AnalyticsPropertyNames.Allowed.Contains(x.Key))
            .Where(x => x.Key.Length <= AnalyticsLimits.MaxPropertyNameLength)
            .Select(x => new KeyValuePair<string, object?>(x.Key, SanitizeValue(x.Value)))
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private static object? SanitizeValue(object? value)
    {
        if (value is JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => LimitString(element.GetString()),
                JsonValueKind.Number when element.TryGetInt32(out var intValue) => intValue,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };
        }

        return value switch
        {
            string text => LimitString(text),
            int number => number,
            bool flag => flag,
            _ => null
        };
    }

    private static string? LimitString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();
        return value.Length <= AnalyticsLimits.MaxStringPropertyLength
            ? value
            : value[..AnalyticsLimits.MaxStringPropertyLength];
    }

    private static void ValidateRequest(TrackAnalyticsEventRequest request)
    {
        if (!AnalyticsEventNames.Allowed.Contains(request.EventName))
            throw new ArgumentException("Unsupported analytics event.", nameof(request));

        if (request.EventName.Length > AnalyticsLimits.MaxEventNameLength)
            throw new ArgumentException("Analytics event name is too long.", nameof(request));

        if (request.SessionId == Guid.Empty)
            throw new ArgumentException("Session identifier is required.", nameof(request));
    }

    private static string? NormalizePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        path = path.Trim();
        return path.Length <= AnalyticsLimits.MaxPagePathLength
            ? path
            : path[..AnalyticsLimits.MaxPagePathLength];
    }

    private static bool IsPositiveFeedback(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty(AnalyticsPropertyNames.Feedback, out var value)
                && value.ValueKind == JsonValueKind.String
                && string.Equals(value.GetString(), "positive", StringComparison.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
