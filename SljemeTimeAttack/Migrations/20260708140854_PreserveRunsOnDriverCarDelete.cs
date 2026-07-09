using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SljemeTimeAttack.Migrations
{
    /// <inheritdoc />
    public partial class PreserveRunsOnDriverCarDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Cars_CarId",
                table: "Runs");

            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Drivers_DriverId",
                table: "Runs");

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "Runs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Runs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CarDisplayNameSnapshot",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarMakeSnapshot",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModelSnapshot",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarRegistrationNumberSnapshot",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverNameSnapshot",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE r
                SET
                    r.DriverNameSnapshot = COALESCE(r.DriverNameSnapshot, d.Name),
                    r.CarMakeSnapshot = COALESCE(r.CarMakeSnapshot, c.Make),
                    r.CarModelSnapshot = COALESCE(r.CarModelSnapshot, c.Model),
                    r.CarRegistrationNumberSnapshot = COALESCE(r.CarRegistrationNumberSnapshot, c.RegistrationNumber),
                    r.CarDisplayNameSnapshot = COALESCE(r.CarDisplayNameSnapshot, CONCAT(c.Make, ' ', c.Model))
                FROM Runs r
                LEFT JOIN Drivers d ON r.DriverId = d.Id
                LEFT JOIN Cars c ON r.CarId = c.Id;
                """);

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CarDisplayNameSnapshot", "CarMakeSnapshot", "CarModelSnapshot", "CarRegistrationNumberSnapshot", "DriverNameSnapshot" },
                values: new object[] { "Mazda RX-8 2004.", "Mazda", "RX-8 2004.", "ZG1234AA", "Denis Horvat" });

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CarDisplayNameSnapshot", "CarMakeSnapshot", "CarModelSnapshot", "CarRegistrationNumberSnapshot", "DriverNameSnapshot" },
                values: new object[] { "Honda Civic EG6 1993.", "Honda", "Civic EG6 1993.", "ZG3456DD", "Ryosuke Takahashi" });

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CarDisplayNameSnapshot", "CarMakeSnapshot", "CarModelSnapshot", "CarRegistrationNumberSnapshot", "DriverNameSnapshot" },
                values: new object[] { "Toyota Celica GT 1998.", "Toyota", "Celica GT 1998.", "ZG5678BB", "Takumi Fujiwara" });

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CarDisplayNameSnapshot", "CarMakeSnapshot", "CarModelSnapshot", "CarRegistrationNumberSnapshot", "DriverNameSnapshot" },
                values: new object[] { "Toyota MR2 Spyder 2000.", "Toyota", "MR2 Spyder 2000.", "ZG9012CC", "Keisuke Takahashi" });

            migrationBuilder.UpdateData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CarDisplayNameSnapshot", "CarMakeSnapshot", "CarModelSnapshot", "CarRegistrationNumberSnapshot", "DriverNameSnapshot" },
                values: new object[] { "Honda S2000 2001.", "Honda", "S2000 2001.", "ZG7890EE", "Hiro Nakamura" });

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Cars_CarId",
                table: "Runs",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Drivers_DriverId",
                table: "Runs",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Cars_CarId",
                table: "Runs");

            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Drivers_DriverId",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "CarDisplayNameSnapshot",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "CarMakeSnapshot",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "CarModelSnapshot",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "CarRegistrationNumberSnapshot",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "DriverNameSnapshot",
                table: "Runs");

            migrationBuilder.Sql("""
                UPDATE Runs
                SET DriverId = (SELECT TOP 1 Id FROM Drivers ORDER BY Id)
                WHERE DriverId IS NULL AND EXISTS (SELECT 1 FROM Drivers);

                UPDATE Runs
                SET CarId = (SELECT TOP 1 Id FROM Cars ORDER BY Id)
                WHERE CarId IS NULL AND EXISTS (SELECT 1 FROM Cars);
                """);

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "Runs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Runs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Cars_CarId",
                table: "Runs",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Drivers_DriverId",
                table: "Runs",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
