using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class addednewrowandenumformissedtimeoutmodel : Migration
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

            migrationBuilder.DropColumn(
                name: "isApproved",
                table: "MissedTimeouts");

            migrationBuilder.AddColumn<string>(
                name: "ApproverUserId",
                table: "MissedTimeouts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HrProfileProfileId",
                table: "MissedTimeouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "MissedTimeouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MissedTimeouts_HrProfileProfileId",
                table: "MissedTimeouts",
                column: "HrProfileProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts",
                column: "HrProfileProfileId",
                principalTable: "HRProfile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts");

            migrationBuilder.DropIndex(
                name: "IX_MissedTimeouts_HrProfileProfileId",
                table: "MissedTimeouts");

            migrationBuilder.DropColumn(
                name: "ApproverUserId",
                table: "MissedTimeouts");

            migrationBuilder.DropColumn(
                name: "HrProfileProfileId",
                table: "MissedTimeouts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "MissedTimeouts");

            migrationBuilder.AddColumn<bool>(
                name: "isApproved",
                table: "MissedTimeouts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "MissedTimeouts",
                columns: new[] { "Id", "Explanation", "LogDate", "LogId", "ProfileId", "Timeout", "isApproved" },
                values: new object[] { 1, null, new DateOnly(2026, 4, 20), 2, 2, null, false });

            migrationBuilder.InsertData(
                table: "StudentLogs",
                columns: new[] { "LogId", "LogDate", "ProfileId", "Status", "TimeIn", "TimeOut", "TotalHours" },
                values: new object[] { 2, new DateOnly(2026, 4, 20), 2, 1, new TimeSpan(0, 8, 0, 0, 0), null, 0m });
        }
    }
}
