using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class adjustcolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentLogs");

            migrationBuilder.AlterColumn<int>(
                name: "StudentWorkProfileId",
                table: "StudentLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs",
                column: "StudentWorkProfileId",
                principalTable: "StudentsWorkProfile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.AlterColumn<int>(
                name: "StudentWorkProfileId",
                table: "StudentLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StudentLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs",
                column: "StudentWorkProfileId",
                principalTable: "StudentsWorkProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
