using System.Text.Json;
using DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Analytics;

namespace DrikWeb.PrivacyFriendlyAnalytics.Tests;

public sealed class AnalyticsServiceTests
{
    [Fact]
    public void SanitizeProperties_RemovesUnknownProperties()
    {
        IReadOnlyDictionary<string, object?> input = new Dictionary<string, object?>
        {
            ["feature"] = "dashboard",
            ["privateValue"] = "must-not-be-stored"
        };

        var result = AnalyticsService.SanitizeProperties(input);

        Assert.Single(result);
        Assert.Equal("dashboard", result["feature"]);
        Assert.False(result.ContainsKey("privateValue"));
    }

    [Fact]
    public void SanitizeProperties_TruncatesLongStrings()
    {
        IReadOnlyDictionary<string, object?> input = new Dictionary<string, object?>
        {
            ["variant"] = new string('x', 500)
        };

        var result = AnalyticsService.SanitizeProperties(input);

        Assert.Equal(100, Assert.IsType<string>(result["variant"]).Length);
    }

    [Fact]
    public void SanitizeProperties_RejectsNestedJsonObjects()
    {
        using var document = JsonDocument.Parse("{\"secret\":\"value\"}");
        IReadOnlyDictionary<string, object?> input = new Dictionary<string, object?>
        {
            ["feature"] = document.RootElement.Clone()
        };

        var result = AnalyticsService.SanitizeProperties(input);

        Assert.Empty(result);
    }
}
