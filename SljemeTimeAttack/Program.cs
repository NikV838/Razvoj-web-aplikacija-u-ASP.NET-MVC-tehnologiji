using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SljemeTimeAttackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<TeamEfRepository>();
builder.Services.AddScoped<DriverEfRepository>();
builder.Services.AddScoped<CarEfRepository>();
builder.Services.AddScoped<RunEfRepository>();
builder.Services.AddScoped<TireEfRepository>();
builder.Services.AddScoped<SuspensionEfRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

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

app.Run();
