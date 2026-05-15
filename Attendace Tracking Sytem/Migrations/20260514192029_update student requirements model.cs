using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class updatestudentrequirementsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemorandumOfAgreement",
                table: "StudentRequirements");

            migrationBuilder.DropColumn(
                name: "NBI",
                table: "StudentRequirements");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentRequirements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MemorandumOfAgreement",
                table: "StudentRequirements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NBI",
                table: "StudentRequirements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StudentId",
                table: "StudentRequirements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MemorandumOfAgreement", "NBI", "StudentId" },
                values: new object[] { false, false, false });

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MemorandumOfAgreement", "NBI", "StudentId" },
                values: new object[] { false, false, false });

            migrationBuilder.UpdateData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MemorandumOfAgreement", "NBI", "StudentId" },
                values: new object[] { false, false, false });
        }
    }
}
