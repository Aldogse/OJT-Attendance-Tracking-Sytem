using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class addedtimeoutrowformissedtimeouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timeout",
                table: "MissedTimeouts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timeout",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeout",
                table: "MissedTimeouts");
        }
    }
}
