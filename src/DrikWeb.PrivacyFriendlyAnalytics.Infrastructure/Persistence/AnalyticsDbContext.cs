using Microsoft.EntityFrameworkCore;
namespace DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Persistence;
public sealed class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    public DbSet<AnalyticsEventEntity> AnalyticsEvents => Set<AnalyticsEventEntity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity=modelBuilder.Entity<AnalyticsEventEntity>();
        entity.ToTable("AnalyticsEvents"); entity.HasKey(x=>x.Id);
        entity.Property(x=>x.EventName).HasMaxLength(100).IsRequired();
        entity.Property(x=>x.PagePath).HasMaxLength(500);
        entity.Property(x=>x.PropertiesJson).HasColumnType("nvarchar(max)");
        entity.HasIndex(x=>x.OccurredAtUtc); entity.HasIndex(x=>x.EventName); entity.HasIndex(x=>x.SessionId);
    }
}
