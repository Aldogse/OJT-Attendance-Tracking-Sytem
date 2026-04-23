using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class adjustseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentLogs",
                keyColumn: "LogId",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MissedTimeouts",
                columns: new[] { "Id", "ApproverProfileId", "Explanation", "HrProfileProfileId", "LogDate", "LogId", "ProfileId", "Timeout", "status" },
                values: new object[] { 1, null, null, null, new DateOnly(2026, 4, 20), 2, 2, null, 1 });

            migrationBuilder.InsertData(
                table: "StudentLogs",
                columns: new[] { "LogId", "LogDate", "ProfileId", "Status", "TimeIn", "TimeOut", "TotalHours" },
                values: new object[] { 2, new DateOnly(2026, 4, 21), 2, 1, new TimeSpan(0, 8, 0, 0, 0), null, 0m });
        }
    }
}
