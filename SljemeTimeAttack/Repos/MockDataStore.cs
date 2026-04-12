using SljemeTimeAttack.Models;
using SljemeTimeAttack.Enums;
using System;
using System.Collections.Generic;

namespace SljemeTimeAttack.Repos
{
    public static class MockDataStore
    {
        public static List<Team> Teams { get; private set; }
        public static List<Driver> Drivers { get; private set; }
        public static List<Car> Cars { get; private set; }
        public static List<Tire> Tires { get; private set; }
        public static List<Rim> Rims { get; private set; }
        public static List<Suspension> Suspensions { get; private set; }
        public static List<Run> Runs { get; private set; }
        public static List<RunNote> Notes { get; private set; }

        static MockDataStore()
        {
            Teams = new List<Team>
            {
                new Team(1, "Red Suns", "Japan", "HKS"),
                new Team(2, "Sljeme Speed Stars", "Croatia", null),
                new Team(3, "Night Kids", "Japan", "RE Amemiya")
            };

            Drivers = new List<Driver>
            {
                new Driver(1, "denis_rx", "Denis Horvat", 23, 5, Teams[1], "denis@mail.com", null),
                new Driver(2, "takumi86", "Takumi Fujiwara", 21, 4, Teams[1], null, null),
                new Driver(3, "keisuke_fd", "Keisuke Takahashi", 25, 7, Teams[2], null, null),
                new Driver(4, "ryosuke_fc", "Ryosuke Takahashi", 28, 10, Teams[2], null, null),
                new Driver(5, "hiro_s2k", "Hiro Tanaka", 26, 6, Teams[0], null, null)
            };

            Rims = new List<Rim>
            {
                new Rim(1, "Rial", "Astorga", 8.0, "Alloy"),           // RX-8
                new Rim(2, "Japan Racing", "SL-03", 8.0, "Alloy"),     // Celica
                new Rim(3, "Enkei", "RPF1", 7.5, "Alloy"),             // MR2
                new Rim(4, "OZ Racing", "Ultraleggera", 7.0, "Alloy"), // Civic
                new Rim(5, "Volk Racing", "TE37", 8.5, "Forged Alloy") // S2000
            };

            Tires = new List<Tire>
            {
                new Tire(1, "Pirelli", "P Zero 225/45 R18", "Street Performance", 225, "2024", Rims[0]), // RX-8
                new Tire(2, "Pirelli", "P Zero 225/45 R17", "Street Performance", 225, "2026", Rims[1]), // Celica
                new Tire(3, "Bridgestone", "RE71R 215/45 R17", "Semi-Slick", 215, "2023", Rims[2]),      // MR2
                new Tire(4, "Toyo", "R888R 225/45 R15", "Semi-Slick", 225, "2022", Rims[3]),              // Civic
                new Tire(5, "Dunlop", "Direzza 215/45 R17", "Semi-Slick", 215, "2023", Rims[4])           // S2000
            };

            Suspensions = new List<Suspension>
            {
                new Suspension(1, "Stock", "Mazda OEM", false, false, 120, false, false, null, null),   // RX-8
                new Suspension(2, "Stock", "Toyota OEM", false, false, 120, false, false, null, null),  // Celica
                new Suspension(3, "Coilover", "KW", true, true, 85, true, true, 9.0, 8.0),               // MR2
                new Suspension(4, "Coilover", "MTS Technik", true, true, 95, true, true, 8.0, 7.0),      // Civic
                new Suspension(5, "Coilover", "Ohlins", true, true, 88, true, true, 9.5, 8.5)            // S2000
            };

            Cars = new List<Car>
            {
                new Car(1, "Mazda", "RX-8 2004", 231, 1300, 2004, "ZG1234AA", Tires[0], Suspensions[0]),
                new Car(2, "Toyota", "Celica 1998 3SGE", 180, 1200, 1998, "ZG5678BB", Tires[1], Suspensions[1]),
                new Car(3, "Toyota", "MR2 2000 1ZZ", 140, 1100, 2000, "ZG9012CC", Tires[2], Suspensions[2]),
                new Car(4, "Honda", "Civic EG6 1993 B16A2", 160, 1050, 1993, "ZG3456DD", Tires[3], Suspensions[3]),
                new Car(5, "Honda", "S2000 2001", 240, 1250, 2001, "ZG7890EE", Tires[4], Suspensions[4])
            };

            Drivers[0].CarsOwned = new List<Car> { Cars[0], Cars[3] }; // Denis → RX-8 + Civic
            Drivers[1].CarsOwned = new List<Car> { Cars[1] };          // Takumi → Celica
            Drivers[2].CarsOwned = new List<Car> { Cars[2] };          // Keisuke → MR2
            Drivers[3].CarsOwned = new List<Car> { Cars[4] };          // Ryosuke → S2000
            Drivers[4].CarsOwned = new List<Car>();                    // Hiro

            Teams[1].Drivers.Add(Drivers[0]); // Denis
            Teams[1].Drivers.Add(Drivers[1]); // Takumi
            Teams[0].Drivers.Add(Drivers[4]); // Hiro
            Teams[2].Drivers.Add(Drivers[2]); // Keisuke
            Teams[2].Drivers.Add(Drivers[3]); // Ryosuke

            Runs = new List<Run>
            {
                new Run(1, 1, 1, Track.Gračani_Sljeme, TimeSpan.FromSeconds(320), DateTime.Now.AddDays(-1), DriveDirection.Uphill, WeatherCondition.Cloudy),
                new Run(2, 1, 4, Track.Bliznec_Brestovac, TimeSpan.FromSeconds(300), DateTime.Now.AddDays(-2), DriveDirection.Downhill, WeatherCondition.Cloudy),
                new Run(3, 2, 2, Track.Bistra_Sljeme, TimeSpan.FromSeconds(340), DateTime.Now.AddDays(-3), DriveDirection.Uphill, WeatherCondition.Rainy),
                new Run(4, 3, 3, Track.Stubica_Sljeme, TimeSpan.FromSeconds(310), DateTime.Now.AddDays(-1), DriveDirection.Downhill, WeatherCondition.Sunny),
                new Run(5, 4, 5, Track.Gračani_Sljeme, TimeSpan.FromSeconds(295), DateTime.Now.AddDays(-4), DriveDirection.Uphill, WeatherCondition.Sunny)
            };

            Drivers[0].Runs = new List<Run> { Runs[0], Runs[1] };
            Drivers[1].Runs = new List<Run> { Runs[2] };
            Drivers[2].Runs = new List<Run> { Runs[3] };
            Drivers[3].Runs = new List<Run> { Runs[4] };
            Drivers[4].Runs = new List<Run>();

            Notes = new List<RunNote>
            {
                new RunNote(1, 1, "Stock RX-8 setup, stable through medium-speed corners.", DateTime.Now),
                new RunNote(2, 2, "Civic with MTS Technik feels sharper on turn-in.", DateTime.Now),
                new RunNote(3, 3, "Celica on stock suspension struggled a bit in wet conditions.", DateTime.Now)
            };
        }
    }
}