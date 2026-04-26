using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class addedverifiedcolumnandmessageforRequirementscolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "StudentRequirements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "StudentRequirements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Message", "Verified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Message", "Verified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Message", "Verified" },
                values: new object[] { null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "StudentRequirements");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "StudentRequirements");
        }
    }
}
