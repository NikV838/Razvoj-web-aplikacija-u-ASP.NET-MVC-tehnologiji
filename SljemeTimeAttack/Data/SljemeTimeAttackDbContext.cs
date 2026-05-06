using System;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Enums;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Data;

public class SljemeTimeAttackDbContext : DbContext
{
    public SljemeTimeAttackDbContext(DbContextOptions<SljemeTimeAttackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Driver> Drivers { get; set; }

    public DbSet<Car> Cars { get; set; }

    public DbSet<Tire> Tires { get; set; }

    public DbSet<Rim> Rims { get; set; }

    public DbSet<Suspension> Suspensions { get; set; }

    public DbSet<Run> Runs { get; set; }

    public DbSet<RunNote> RunNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rim>().HasData(
            new { Id = 1, Make = "Rial", Model = "Astorga", SizeInJ = 8.0, Material = "Alloy" },
            new { Id = 2, Make = "Japan Racing", Model = "SL-03", SizeInJ = 8.0, Material = "Alloy" },
            new { Id = 3, Make = "Enkei", Model = "RPF1", SizeInJ = 7.5, Material = "Alloy" },
            new { Id = 4, Make = "OZ Racing", Model = "Ultraleggera", SizeInJ = 7.0, Material = "Alloy" },
            new { Id = 5, Make = "Volk Racing", Model = "TE37", SizeInJ = 8.5, Material = "Forged Alloy" });

        modelBuilder.Entity<Suspension>().HasData(
            new { Id = 1, Type = "Stock", Brand = "Mazda OEM", HasFrontStrutBar = false, HasRearStrutBar = false, RideHeightMm = 120.0, IsHeightAdjustable = false, IsStiffnessAdjustable = false, FrontStiffness = (double?)null, RearStiffness = (double?)null },
            new { Id = 2, Type = "Stock", Brand = "Toyota OEM", HasFrontStrutBar = false, HasRearStrutBar = false, RideHeightMm = 120.0, IsHeightAdjustable = false, IsStiffnessAdjustable = false, FrontStiffness = (double?)null, RearStiffness = (double?)null },
            new { Id = 3, Type = "Coilover", Brand = "KW", HasFrontStrutBar = true, HasRearStrutBar = true, RideHeightMm = 85.0, IsHeightAdjustable = true, IsStiffnessAdjustable = true, FrontStiffness = (double?)9.0, RearStiffness = (double?)8.0 },
            new { Id = 4, Type = "Coilover", Brand = "MTS Technik", HasFrontStrutBar = true, HasRearStrutBar = true, RideHeightMm = 95.0, IsHeightAdjustable = true, IsStiffnessAdjustable = true, FrontStiffness = (double?)8.0, RearStiffness = (double?)7.0 },
            new { Id = 5, Type = "Coilover", Brand = "Ohlins", HasFrontStrutBar = true, HasRearStrutBar = true, RideHeightMm = 88.0, IsHeightAdjustable = true, IsStiffnessAdjustable = true, FrontStiffness = (double?)9.5, RearStiffness = (double?)8.5 });

        modelBuilder.Entity<Tire>().HasData(
            new { Id = 1, Brand = "Pirelli", Model = "P Zero 225/45 R18", Type = "Street Performance", SizeInMm = 225.0, Dot = "2024", RimId = 1 },
            new { Id = 2, Brand = "Pirelli", Model = "P Zero 225/45 R17", Type = "Street Performance", SizeInMm = 225.0, Dot = "2026", RimId = 2 },
            new { Id = 3, Brand = "Bridgestone", Model = "RE71R 215/45 R17", Type = "Semi-Slick", SizeInMm = 215.0, Dot = "2023", RimId = 3 },
            new { Id = 4, Brand = "Toyo", Model = "R888R 225/45 R15", Type = "Semi-Slick", SizeInMm = 225.0, Dot = "2022", RimId = 4 },
            new { Id = 5, Brand = "Dunlop", Model = "Direzza 215/45 R17", Type = "Semi-Slick", SizeInMm = 215.0, Dot = "2023", RimId = 5 });

        modelBuilder.Entity<Team>().HasData(
            new { Id = 1, Name = "Red Suns", Country = "Japan", Sponsor = "HKS" },
            new { Id = 2, Name = "Sljeme Speed Stars", Country = "Croatia", Sponsor = (string?)null },
            new { Id = 3, Name = "Night Kids", Country = "Japan", Sponsor = "RE Amemiya" });

        modelBuilder.Entity<Driver>().HasData(
            new { Id = 1, Username = "denis_rx", Name = "Denis Horvat", Age = 23, YearsOfExperience = 5, TeamId = (int?)2, Email = "denis@mail.com", PhoneNumber = (string?)null },
            new { Id = 2, Username = "takumi86", Name = "Takumi Fujiwara", Age = 21, YearsOfExperience = 4, TeamId = (int?)2, Email = (string?)null, PhoneNumber = (string?)null },
            new { Id = 3, Username = "keisuke_fd", Name = "Keisuke Takahashi", Age = 25, YearsOfExperience = 7, TeamId = (int?)3, Email = (string?)null, PhoneNumber = (string?)null },
            new { Id = 4, Username = "ryosuke_fc", Name = "Ryosuke Takahashi", Age = 28, YearsOfExperience = 10, TeamId = (int?)3, Email = (string?)null, PhoneNumber = (string?)null },
            new { Id = 5, Username = "hiro_s2k", Name = "Hiro Tanaka", Age = 26, YearsOfExperience = 6, TeamId = (int?)1, Email = (string?)null, PhoneNumber = (string?)null });

        modelBuilder.Entity<Car>().HasData(
            new { Id = 1, Make = "Mazda", Model = "RX-8 2004.", Horsepower = 231, WeightKg = 1300.0, Year = 2004, RegistrationNumber = "ZG1234AA", TireId = 1, SuspensionId = 1 },
            new { Id = 2, Make = "Toyota", Model = "Celica GT 1998.", Horsepower = 180, WeightKg = 1200.0, Year = 1998, RegistrationNumber = "ZG5678BB", TireId = 2, SuspensionId = 2 },
            new { Id = 3, Make = "Toyota", Model = "MR2 Spyder 2000.", Horsepower = 140, WeightKg = 1100.0, Year = 2000, RegistrationNumber = "ZG9012CC", TireId = 3, SuspensionId = 3 },
            new { Id = 4, Make = "Honda", Model = "Civic EG6 1993.", Horsepower = 160, WeightKg = 1050.0, Year = 1993, RegistrationNumber = "ZG3456DD", TireId = 4, SuspensionId = 4 },
            new { Id = 5, Make = "Honda", Model = "S2000 2001.", Horsepower = 240, WeightKg = 1250.0, Year = 2001, RegistrationNumber = "ZG7890EE", TireId = 5, SuspensionId = 5 });

        modelBuilder.Entity<Run>().HasData(
            new { Id = 1, DriverId = 1, CarId = 1, Track = (Track)1, BestTime = TimeSpan.FromSeconds(320), Date = new DateTime(2026, 5, 3, 12, 0, 0), Direction = DriveDirection.Uphill, Weather = WeatherCondition.Cloudy },
            new { Id = 2, DriverId = 1, CarId = 4, Track = Track.Bliznec_Brestovac, BestTime = TimeSpan.FromSeconds(300), Date = new DateTime(2026, 5, 2, 12, 0, 0), Direction = DriveDirection.Downhill, Weather = WeatherCondition.Cloudy },
            new { Id = 3, DriverId = 2, CarId = 2, Track = Track.Bistra_Sljeme, BestTime = TimeSpan.FromSeconds(340), Date = new DateTime(2026, 5, 1, 12, 0, 0), Direction = DriveDirection.Uphill, Weather = WeatherCondition.Rainy },
            new { Id = 4, DriverId = 3, CarId = 3, Track = Track.Stubica_Sljeme, BestTime = TimeSpan.FromSeconds(310), Date = new DateTime(2026, 5, 3, 13, 0, 0), Direction = DriveDirection.Downhill, Weather = WeatherCondition.Sunny },
            new { Id = 5, DriverId = 4, CarId = 5, Track = (Track)1, BestTime = TimeSpan.FromSeconds(295), Date = new DateTime(2026, 4, 30, 12, 0, 0), Direction = DriveDirection.Uphill, Weather = WeatherCondition.Sunny });

        modelBuilder.Entity<RunNote>().HasData(
            new { Id = 1, RunId = 1, Note = "Stock RX-8 setup, stable through medium-speed corners.", CreatedDate = new DateTime(2026, 5, 4, 12, 0, 0) },
            new { Id = 2, RunId = 2, Note = "Civic with MTS Technik feels sharper on turn-in.", CreatedDate = new DateTime(2026, 5, 4, 12, 5, 0) },
            new { Id = 3, RunId = 3, Note = "Celica on stock suspension struggled a bit in wet conditions.", CreatedDate = new DateTime(2026, 5, 4, 12, 10, 0) });
    }
}
