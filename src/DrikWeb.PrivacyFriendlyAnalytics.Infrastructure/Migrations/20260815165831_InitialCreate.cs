using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrikWeb.PrivacyFriendlyAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyticsEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PropertiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_EventName",
                table: "AnalyticsEvents",
                column: "EventName");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_OccurredAtUtc",
                table: "AnalyticsEvents",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_SessionId",
                table: "AnalyticsEvents",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsEvents");
        }
    }
}
