using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddDbContext<SljemeTimeAttackDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
    {
        options.UseInMemoryDatabase("SljemeTimeAttackTests");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<SljemeTimeAttackDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var authenticationBuilder = builder.Services.AddAuthentication();
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
if (!string.IsNullOrWhiteSpace(facebookAppId) && !string.IsNullOrWhiteSpace(facebookAppSecret))
{
    authenticationBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
        options.Scope.Add("email");
        options.Fields.Add("name");
        options.Fields.Add("email");
    });
}
builder.Services.AddScoped<TeamEfRepository>();
builder.Services.AddScoped<DriverEfRepository>();
builder.Services.AddScoped<CarEfRepository>();
builder.Services.AddScoped<RunEfRepository>();
builder.Services.AddScoped<TireEfRepository>();
builder.Services.AddScoped<SuspensionEfRepository>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var staticFileContentTypes = new FileExtensionContentTypeProvider();
staticFileContentTypes.Mappings[".avif"] = "image/avif";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = staticFileContentTypes
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "drivers-list",
    pattern: "drivers",
    defaults: new { controller = "Driver", action = "Index" });


app.MapControllerRoute(
    name: "driver-details",
    pattern: "drivers/{id:int}",
    defaults: new { controller = "Driver", action = "Details" });


app.MapControllerRoute(
    name: "cars-list",
    pattern: "cars",
    defaults: new { controller = "Car", action = "Index" });


app.MapControllerRoute(
    name: "runs-list",
    pattern: "runs",
    defaults: new { controller = "Run", action = "Index" });


app.MapControllerRoute(
    name: "team-details",
    pattern: "teams/{id:int}",
    defaults: new { controller = "Team", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await ApplyDatabaseMigrationsAsync(app.Services, app.Environment);
await SeedIdentityAsync(app.Services, app.Configuration);

app.Run();

public partial class Program
{
    private static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services, IHostEnvironment environment)
    {
        if (environment.IsEnvironment("Testing"))
        {
            return;
        }

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SljemeTimeAttackDbContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedIdentityAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (var role in new[] { "Admin", "User", "Racer", "Spectator" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminUserName = "Admin";
        const string adminEmail = "nvdovic@tvz.hr";
        const string adminPassword = "Pa$$w0rd";

        var admin = await userManager.FindByNameAsync(adminUserName)
            ?? await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = "Admin"
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                return;
            }
        }
        else
        {
            if (admin.UserName != adminUserName && await userManager.FindByNameAsync(adminUserName) == null)
            {
                await userManager.SetUserNameAsync(admin, adminUserName);
            }

            if (admin.DisplayName != "Admin")
            {
                admin.DisplayName = "Admin";
                await userManager.UpdateAsync(admin);
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
