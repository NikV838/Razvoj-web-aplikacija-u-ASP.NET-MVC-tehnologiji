using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SljemeTimeAttack.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Drivers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Drivers
                SET AppUserId = AspNetUsers.Id
                FROM Drivers
                INNER JOIN AspNetUsers ON AspNetUsers.LinkedDriverId = Drivers.Id
                WHERE AspNetUsers.LinkedDriverId IS NOT NULL
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Drivers_LinkedDriverId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LinkedDriverId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LinkedDriverId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 1,
                column: "AppUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 2,
                column: "AppUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 3,
                column: "AppUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 4,
                column: "AppUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 5,
                column: "AppUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_AppUserId",
                table: "Drivers",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_AspNetUsers_AppUserId",
                table: "Drivers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_AspNetUsers_AppUserId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_AppUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "LinkedDriverId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LinkedDriverId",
                table: "AspNetUsers",
                column: "LinkedDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Drivers_LinkedDriverId",
                table: "AspNetUsers",
                column: "LinkedDriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
