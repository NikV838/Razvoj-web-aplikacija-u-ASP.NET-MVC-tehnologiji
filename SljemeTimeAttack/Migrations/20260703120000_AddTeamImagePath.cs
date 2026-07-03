using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SljemeTimeAttack.Data;

#nullable disable

namespace SljemeTimeAttack.Migrations
{
    [DbContext(typeof(SljemeTimeAttackDbContext))]
    [Migration("20260703120000_AddTeamImagePath")]
    public partial class AddTeamImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [Teams]
                SET [ImagePath] = CASE [Id]
                    WHEN 1 THEN '/img/teams/redsuns.webp'
                    WHEN 2 THEN '/img/teams/SljemeSpeedStars.png'
                    WHEN 3 THEN '/img/teams/nightkids.webp'
                    ELSE [ImagePath]
                END
                WHERE [Id] IN (1, 2, 3);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Teams");
        }
    }
}
