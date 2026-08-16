using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DrikWeb.PrivacyFriendlyAnalytics.Application.Analytics;
using DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Analytics;
using DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Persistence;
namespace DrikWeb.PrivacyFriendlyAnalytics.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddAnalyticsInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        var cs=configuration.GetConnectionString("Analytics")??throw new InvalidOperationException("Connection string 'Analytics' was not found.");
        services.AddDbContext<AnalyticsDbContext>(o=>o.UseSqlServer(cs));
        services.AddScoped<IAnalyticsService,AnalyticsService>();
        return services;
    }
}
