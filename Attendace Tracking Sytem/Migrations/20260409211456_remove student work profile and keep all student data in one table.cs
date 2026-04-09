using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class removestudentworkprofileandkeepallstudentdatainonetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs");

            migrationBuilder.DropTable(
                name: "StudentsWorkProfile");

            migrationBuilder.RenameColumn(
                name: "StudentWorkProfileId",
                table: "StudentLogs",
                newName: "ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLogs_StudentWorkProfileId",
                table: "StudentLogs",
                newName: "IX_StudentLogs_ProfileId");

            migrationBuilder.RenameColumn(
                name: "StudentWorkProfileId",
                table: "MissedTimeouts",
                newName: "ProfileId");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "StudentsProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "StudentsProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "HoursRendered",
                table: "StudentsProfile",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequiredHours",
                table: "StudentsProfile",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ShiftEnd",
                table: "StudentsProfile",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ShiftStart",
                table: "StudentsProfile",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "StudentsProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentsProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsProfile_ProfileId",
                table: "StudentLogs",
                column: "ProfileId",
                principalTable: "StudentsProfile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLogs_StudentsProfile_ProfileId",
                table: "StudentLogs");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "HoursRendered",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "RequiredHours",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "ShiftEnd",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "ShiftStart",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentsProfile");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentsProfile");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "StudentLogs",
                newName: "StudentWorkProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLogs_ProfileId",
                table: "StudentLogs",
                newName: "IX_StudentLogs_StudentWorkProfileId");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "MissedTimeouts",
                newName: "StudentWorkProfileId");

            migrationBuilder.CreateTable(
                name: "StudentsWorkProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HoursRendered = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequiredHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShiftEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    ShiftStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsWorkProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentsWorkProfile_StudentsProfile_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentsProfile",
                        principalColumn: "ProfileId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentsWorkProfile_StudentProfileId",
                table: "StudentsWorkProfile",
                column: "StudentProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLogs_StudentsWorkProfile_StudentWorkProfileId",
                table: "StudentLogs",
                column: "StudentWorkProfileId",
                principalTable: "StudentsWorkProfile",
                principalColumn: "Id");
        }
    }
}
