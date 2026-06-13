using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class remove_notification_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsentismNotified",
                table: "StudentsProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbsentismNotified",
                table: "StudentsProfile",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
