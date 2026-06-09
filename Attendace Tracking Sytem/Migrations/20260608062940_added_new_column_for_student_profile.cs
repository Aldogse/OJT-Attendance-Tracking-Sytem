using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class added_new_column_for_student_profile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbsentismNotified",
                table: "StudentsProfile",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsentismNotified",
                table: "StudentsProfile");
        }
    }
}
