using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class removeforeignkeyforstudentlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_Id",
                table: "StudentLogs");

            migrationBuilder.DropIndex(
                name: "IX_StudentLogs_Id",
                table: "StudentLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "StudentLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "StudentWorkProfileId",
                table: "StudentLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLogs_StudentWorkProfileId",
                table: "StudentLogs",
                column: "StudentWorkProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs",
                column: "StudentWorkProfileId",
                principalTable: "StudentsWorkProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.DropIndex(
                name: "IX_StudentLogs_StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.DropColumn(
                name: "StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "StudentLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLogs_Id",
                table: "StudentLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_Id",
                table: "StudentLogs",
                column: "Id",
                principalTable: "StudentsWorkProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
