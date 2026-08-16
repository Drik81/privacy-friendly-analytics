namespace DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
public static class AnalyticsEventNames
{
    public const string DemoOpened = "demo_opened";
    public const string WorkflowStarted = "workflow_started";
    public const string WorkflowCompleted = "workflow_completed";
    public const string FeatureUsed = "feature_used";
    public const string FeedbackSubmitted = "feedback_submitted";
    public static readonly HashSet<string> Allowed = [DemoOpened, WorkflowStarted, WorkflowCompleted, FeatureUsed, FeedbackSubmitted];
}
