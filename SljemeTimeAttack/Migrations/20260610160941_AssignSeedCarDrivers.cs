using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SljemeTimeAttack.Migrations
{
    /// <inheritdoc />
    public partial class AssignSeedCarDrivers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Hiro Nakamura");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1,
                column: "DriverId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2,
                column: "DriverId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3,
                column: "DriverId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 4,
                column: "DriverId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 5,
                column: "DriverId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 2,
                column: "DriverId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 5,
                column: "DriverId",
                value: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Hiro Tanaka");

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 5,
                column: "DriverId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 2,
                column: "DriverId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 5,
                column: "DriverId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 4,
                column: "DriverId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3,
                column: "DriverId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2,
                column: "DriverId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1,
                column: "DriverId",
                value: null);
        }
    }
}
