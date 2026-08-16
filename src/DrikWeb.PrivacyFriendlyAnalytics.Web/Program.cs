using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
using DrikWeb.PrivacyFriendlyAnalytics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddAnalyticsInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("analytics-ingestion", limiterOptions =>
    {
        limiterOptions.PermitLimit = 60;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();

app.MapRazorPages();

app.MapPost("/api/analytics/events", async (
    TrackAnalyticsEventRequest request,
    IAnalyticsService analyticsService,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    try
    {
        await analyticsService.TrackAsync(request, cancellationToken);
        return Results.NoContent();
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
    catch (Exception exception)
    {
        // Analytics must not expose implementation details to callers.
        // The product workflow should treat telemetry as best-effort.
        logger.LogError(exception, "Unable to persist analytics event {EventName}.", request.EventName);
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
})
.RequireRateLimiting("analytics-ingestion");

app.Run();

public partial class Program;
