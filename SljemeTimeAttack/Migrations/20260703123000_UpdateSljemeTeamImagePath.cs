using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SljemeTimeAttack.Data;

#nullable disable

namespace SljemeTimeAttack.Migrations
{
    [DbContext(typeof(SljemeTimeAttackDbContext))]
    [Migration("20260703123000_UpdateSljemeTeamImagePath")]
    public partial class UpdateSljemeTeamImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Teams]
                SET [ImagePath] = '/img/teams/SljemeSpeedStars.png'
                WHERE [Id] = 2;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Teams]
                SET [ImagePath] = '/img/teams/SljemeSpeedStars.jpeg'
                WHERE [Id] = 2;
                """);
        }
    }
}
