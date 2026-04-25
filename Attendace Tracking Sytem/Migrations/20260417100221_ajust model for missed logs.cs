using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class ajustmodelformissedlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isApproved",
                table: "MissedTimeouts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "MissedTimeouts",
                keyColumn: "Id",
                keyValue: 1,
                column: "isApproved",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isApproved",
                table: "MissedTimeouts");
        }
    }
}
