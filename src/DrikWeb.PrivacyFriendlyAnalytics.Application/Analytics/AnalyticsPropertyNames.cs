namespace DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;

public static class AnalyticsPropertyNames
{
    public const string Feature = "feature";
    public const string FeatureCount = "featureCount";
    public const string Variant = "variant";
    public const string Feedback = "feedback";

    public static readonly HashSet<string> Allowed =
    [
        Feature,
        FeatureCount,
        Variant,
        Feedback
    ];
}
