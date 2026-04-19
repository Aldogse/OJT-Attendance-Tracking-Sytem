using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class newsetofseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "LogDate",
                value: new DateOnly(2026, 4, 20));

            migrationBuilder.UpdateData(
                table: "StudentLogs",
                keyColumn: "LogId",
                keyValue: 2,
                column: "LogDate",
                value: new DateOnly(2026, 4, 20));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "LogDate",
                value: new DateOnly(2026, 4, 18));

            migrationBuilder.UpdateData(
                table: "StudentLogs",
                keyColumn: "LogId",
                keyValue: 2,
                column: "LogDate",
                value: new DateOnly(2026, 4, 18));
        }
    }
}
