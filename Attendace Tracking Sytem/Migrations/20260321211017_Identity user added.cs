using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class Identityuseradded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentsWorkProfile_StudentsProfile_StudentProfileId",
                table: "StudentsWorkProfile");

            migrationBuilder.AlterColumn<int>(
                name: "StudentProfileId",
                table: "StudentsWorkProfile",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StudentsProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsWorkProfile_StudentsProfile_StudentProfileId",
                table: "StudentsWorkProfile",
                column: "StudentProfileId",
                principalTable: "StudentsProfile",
                principalColumn: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentsWorkProfile_StudentsProfile_StudentProfileId",
                table: "StudentsWorkProfile");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentsProfile");

            migrationBuilder.AlterColumn<int>(
                name: "StudentProfileId",
                table: "StudentsWorkProfile",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsWorkProfile_StudentsProfile_StudentProfileId",
                table: "StudentsWorkProfile",
                column: "StudentProfileId",
                principalTable: "StudentsProfile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
