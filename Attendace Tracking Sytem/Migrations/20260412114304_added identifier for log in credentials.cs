using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class addedidentifierforlogincredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProfileCompleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileCompleted",
                table: "AspNetUsers");
        }
    }
}
