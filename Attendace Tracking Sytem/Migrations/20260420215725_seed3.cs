using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class seed3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts");

            migrationBuilder.AlterColumn<int>(
                name: "HrProfileProfileId",
                table: "MissedTimeouts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "MissedTimeouts",
                columns: new[] { "Id", "ApproverUserId", "Explanation", "HrProfileProfileId", "LogDate", "LogId", "ProfileId", "Timeout", "status" },
                values: new object[] { 1, "40c23957-fd0b-4b75-a8ff-c8c99567e7d1", null, null, new DateOnly(2026, 4, 20), 2, 2, null, 1 });

            migrationBuilder.InsertData(
                table: "StudentLogs",
                columns: new[] { "LogId", "LogDate", "ProfileId", "Status", "TimeIn", "TimeOut", "TotalHours" },
                values: new object[] { 2, new DateOnly(2026, 4, 21), 2, 1, new TimeSpan(0, 8, 0, 0, 0), null, 0m });

            migrationBuilder.AddForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts",
                column: "HrProfileProfileId",
                principalTable: "HRProfile",
                principalColumn: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts");

            migrationBuilder.DeleteData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentLogs",
                keyColumn: "LogId",
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "HrProfileProfileId",
                table: "MissedTimeouts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MissedTimeouts_HRProfile_HrProfileProfileId",
                table: "MissedTimeouts",
                column: "HrProfileProfileId",
                principalTable: "HRProfile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
