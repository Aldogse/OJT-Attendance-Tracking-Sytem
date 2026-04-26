using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class addednewmodelforStudentrequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    NBI = table.Column<bool>(type: "bit", nullable: false),
                    MemorandumOfAgreement = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<bool>(type: "bit", nullable: false),
                    NbiImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemorandumOfAgreementImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentIdImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRequirements_StudentsProfile_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentsProfile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequirements_StudentProfileId",
                table: "StudentRequirements",
                column: "StudentProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentRequirements");
        }
    }
}
