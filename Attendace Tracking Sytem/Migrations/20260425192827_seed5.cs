using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Attendace_Tracking_Sytem.Migrations
{
    /// <inheritdoc />
    public partial class seed5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StudentRequirements",
                columns: new[] { "Id", "MemorandumOfAgreement", "MemorandumOfAgreementImagePath", "NBI", "NbiImagePath", "StudentId", "StudentIdImagePath", "StudentProfileId" },
                values: new object[,]
                {
                    { 1, false, null, false, null, false, null, 5 },
                    { 2, false, null, false, null, false, null, 4 },
                    { 3, false, null, false, null, false, null, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StudentRequirements",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
