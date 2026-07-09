using Microsoft.Extensions.DependencyInjection;
using SljemeTimeAttack.Data;

namespace SljemeTimeAttack.Tests;

public static class TestDatabase
{
    public static void Reset(SljemeApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SljemeTimeAttackDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
