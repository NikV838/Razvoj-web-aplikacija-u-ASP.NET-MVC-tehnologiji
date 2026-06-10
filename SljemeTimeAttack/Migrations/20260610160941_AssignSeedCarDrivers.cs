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
            migrationBuilder.Sql("""
                UPDATE [Drivers]
                SET [Name] = N'Hiro Nakamura'
                WHERE [Id] = 5;

                UPDATE [Cars]
                SET [DriverId] = CASE [Id]
                    WHEN 1 THEN 1
                    WHEN 2 THEN 2
                    WHEN 3 THEN 3
                    WHEN 4 THEN 4
                    WHEN 5 THEN 5
                END
                WHERE [Id] IN (1, 2, 3, 4, 5);

                UPDATE [Runs]
                SET [DriverId] = CASE [Id]
                    WHEN 2 THEN 4
                    WHEN 5 THEN 5
                END
                WHERE [Id] IN (2, 5);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Drivers]
                SET [Name] = N'Hiro Tanaka'
                WHERE [Id] = 5;

                UPDATE [Runs]
                SET [DriverId] = CASE [Id]
                    WHEN 2 THEN 1
                    WHEN 5 THEN 4
                END
                WHERE [Id] IN (2, 5);

                UPDATE [Cars]
                SET [DriverId] = NULL
                WHERE [Id] IN (1, 2, 3, 4, 5);
                """);
        }
    }
}
