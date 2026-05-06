using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SljemeTimeAttack.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rims",
                columns: new[] { "Id", "Make", "Material", "Model", "SizeInJ" },
                values: new object[,]
                {
                    { 1, "Rial", "Alloy", "Astorga", 8.0 },
                    { 2, "Japan Racing", "Alloy", "SL-03", 8.0 },
                    { 3, "Enkei", "Alloy", "RPF1", 7.5 },
                    { 4, "OZ Racing", "Alloy", "Ultraleggera", 7.0 },
                    { 5, "Volk Racing", "Forged Alloy", "TE37", 8.5 }
                });

            migrationBuilder.InsertData(
                table: "Suspensions",
                columns: new[] { "Id", "Brand", "FrontStiffness", "HasFrontStrutBar", "HasRearStrutBar", "IsHeightAdjustable", "IsStiffnessAdjustable", "RearStiffness", "RideHeightMm", "Type" },
                values: new object[,]
                {
                    { 1, "Mazda OEM", null, false, false, false, false, null, 120.0, "Stock" },
                    { 2, "Toyota OEM", null, false, false, false, false, null, 120.0, "Stock" },
                    { 3, "KW", 9.0, true, true, true, true, 8.0, 85.0, "Coilover" },
                    { 4, "MTS Technik", 8.0, true, true, true, true, 7.0, 95.0, "Coilover" },
                    { 5, "Ohlins", 9.5, true, true, true, true, 8.5, 88.0, "Coilover" }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Country", "Name", "Sponsor" },
                values: new object[,]
                {
                    { 1, "Japan", "Red Suns", "HKS" },
                    { 2, "Croatia", "Sljeme Speed Stars", null },
                    { 3, "Japan", "Night Kids", "RE Amemiya" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Age", "Email", "Name", "PhoneNumber", "TeamId", "Username", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 23, "denis@mail.com", "Denis Horvat", null, 2, "denis_rx", 5 },
                    { 2, 21, null, "Takumi Fujiwara", null, 2, "takumi86", 4 },
                    { 3, 25, null, "Keisuke Takahashi", null, 3, "keisuke_fd", 7 },
                    { 4, 28, null, "Ryosuke Takahashi", null, 3, "ryosuke_fc", 10 },
                    { 5, 26, null, "Hiro Tanaka", null, 1, "hiro_s2k", 6 }
                });

            migrationBuilder.InsertData(
                table: "Tires",
                columns: new[] { "Id", "Brand", "Dot", "Model", "RimId", "SizeInMm", "Type" },
                values: new object[,]
                {
                    { 1, "Pirelli", "2024", "P Zero 225/45 R18", 1, 225.0, "Street Performance" },
                    { 2, "Pirelli", "2026", "P Zero 225/45 R17", 2, 225.0, "Street Performance" },
                    { 3, "Bridgestone", "2023", "RE71R 215/45 R17", 3, 215.0, "Semi-Slick" },
                    { 4, "Toyo", "2022", "R888R 225/45 R15", 4, 225.0, "Semi-Slick" },
                    { 5, "Dunlop", "2023", "Direzza 215/45 R17", 5, 215.0, "Semi-Slick" }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "DriverId", "Horsepower", "Make", "Model", "RegistrationNumber", "SuspensionId", "TireId", "WeightKg", "Year" },
                values: new object[,]
                {
                    { 1, null, 231, "Mazda", "RX-8 2004.", "ZG1234AA", 1, 1, 1300.0, 2004 },
                    { 2, null, 180, "Toyota", "Celica GT 1998.", "ZG5678BB", 2, 2, 1200.0, 1998 },
                    { 3, null, 140, "Toyota", "MR2 Spyder 2000.", "ZG9012CC", 3, 3, 1100.0, 2000 },
                    { 4, null, 160, "Honda", "Civic EG6 1993.", "ZG3456DD", 4, 4, 1050.0, 1993 },
                    { 5, null, 240, "Honda", "S2000 2001.", "ZG7890EE", 5, 5, 1250.0, 2001 }
                });

            migrationBuilder.InsertData(
                table: "Runs",
                columns: new[] { "Id", "BestTime", "CarId", "Date", "Direction", "DriverId", "Track", "Weather" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 0, 5, 20, 0), 1, new DateTime(2026, 5, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, 1, 1 },
                    { 2, new TimeSpan(0, 0, 5, 0, 0), 4, new DateTime(2026, 5, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, 1 },
                    { 3, new TimeSpan(0, 0, 5, 40, 0), 2, new DateTime(2026, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, 2, 2 },
                    { 4, new TimeSpan(0, 0, 5, 10, 0), 3, new DateTime(2026, 5, 3, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 3, 0 },
                    { 5, new TimeSpan(0, 0, 4, 55, 0), 5, new DateTime(2026, 4, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), 0, 4, 1, 0 }
                });

            migrationBuilder.InsertData(
                table: "RunNotes",
                columns: new[] { "Id", "CreatedDate", "Note", "RunId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), "Stock RX-8 setup, stable through medium-speed corners.", 1 },
                    { 2, new DateTime(2026, 5, 4, 12, 5, 0, 0, DateTimeKind.Unspecified), "Civic with MTS Technik feels sharper on turn-in.", 2 },
                    { 3, new DateTime(2026, 5, 4, 12, 10, 0, 0, DateTimeKind.Unspecified), "Celica on stock suspension struggled a bit in wet conditions.", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RunNotes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RunNotes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RunNotes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Runs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suspensions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Suspensions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Suspensions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suspensions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suspensions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rims",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
