using SljemeTimeAttack.Models;
using SljemeTimeAttack.Enums;

var teams = new List<Team>
{
    new Team(1, "Red Suns", "Japan", "HKS"),
    new Team(2, "Sljeme Speed Stars", "Croatia", null),
    new Team(3, "Night Kids", "Japan", "RE Amemiya")
};

var drivers = new List<Driver>
{
    new Driver(1, "denis_rx", "Denis Horvat", 23, 5, teams[1], "denis@mail.com", null),
    new Driver(2, "takumi86", "Takumi Fujiwara", 21, 4, teams[1], null, null),
    new Driver(3, "keisuke_fd", "Keisuke Takahashi", 25, 7, teams[2], null, null),
    new Driver(4, "ryosuke_fc", "Ryosuke Takahashi", 28, 10, teams[2], null, null),
    new Driver(5, "hiro_s2k", "Hiro Tanaka", 26, 6, teams[0], null, null)
};

var rims = new List<Rim>
{
    new Rim(1, "Rial", "Astorga", 8.0, "Alloy"),           // RX-8
    new Rim(2, "Japan Racing", "SL-03", 8.0, "Alloy"),     // Celica
    new Rim(3, "Enkei", "RPF1", 7.5, "Alloy"),             // MR2
    new Rim(4, "OZ Racing", "Ultraleggera", 7.0, "Alloy"), // Civic
    new Rim(5, "Volk Racing", "TE37", 8.5, "Forged Alloy") // S2000
};

var tires = new List<Tire>
{
    new Tire(1, "Pirelli", "P Zero 225/45 R18", "Street Performance", 225, "2024", rims[0]), // RX-8
    new Tire(2, "Pirelli", "P Zero 225/45 R17", "Street Performance", 225, "2026", rims[1]), // Celica
    new Tire(3, "Bridgestone", "RE71R 215/45 R17", "Semi-Slick", 215, "2023", rims[2]),      // MR2
    new Tire(4, "Toyo", "R888R 225/45 R15", "Semi-Slick", 225, "2022", rims[3]),              // Civic
    new Tire(5, "Dunlop", "Direzza 215/45 R17", "Semi-Slick", 215, "2023", rims[4])           // S2000
};

var suspensions = new List<Suspension>
{
    new Suspension(1, "Stock", "Mazda OEM", false, false, 120, false, false, null, null),   // RX-8
    new Suspension(2, "Stock", "Toyota OEM", false, false, 120, false, false, null, null),  // Celica
    new Suspension(3, "Coilover", "KW", true, true, 85, true, true, 9.0, 8.0),               // MR2
    new Suspension(4, "Coilover", "MTS Technik", true, true, 95, true, true, 8.0, 7.0),      // Civic
    new Suspension(5, "Coilover", "Ohlins", true, true, 88, true, true, 9.5, 8.5)            // S2000
};

var cars = new List<Car>
{
    new Car(1, "Mazda", "RX-8 2004", 231, 1300, 2004, "ZG1234AA", tires[0], suspensions[0]),
    new Car(2, "Toyota", "Celica 1998 3SGE", 180, 1200, 1998, "ZG5678BB", tires[1], suspensions[1]),
    new Car(3, "Toyota", "MR2 2000 1ZZ", 140, 1100, 2000, "ZG9012CC", tires[2], suspensions[2]),
    new Car(4, "Honda", "Civic EG6 1993 B16A2", 160, 1050, 1993, "ZG3456DD", tires[3], suspensions[3]),
    new Car(5, "Honda", "S2000 2001", 240, 1250, 2001, "ZG7890EE", tires[4], suspensions[4])
};


drivers[0].CarsOwned = new List<Car> { cars[0], cars[3] }; // Denis → RX-8 + Civic
drivers[1].CarsOwned = new List<Car> { cars[1] };          // Takumi → Celica
drivers[2].CarsOwned = new List<Car> { cars[2] };          // Keisuke → MR2
drivers[3].CarsOwned = new List<Car> { cars[4] };          // Ryosuke → S2000
drivers[4].CarsOwned = new List<Car>();                    // Hiro


// Sljeme Speed Stars
teams[1].Drivers.Add(drivers[0]); // Denis
teams[1].Drivers.Add(drivers[1]); // Takumi

// Red Suns
teams[0].Drivers.Add(drivers[4]); // Hiro

// Night Kids
teams[2].Drivers.Add(drivers[2]); // Keisuke
teams[2].Drivers.Add(drivers[3]); // Ryosuke


var runs = new List<Run>
{
    new Run(1, 1, 1, Track.Gračani_Sljeme, TimeSpan.FromSeconds(320), DateTime.Now.AddDays(-1), DriveDirection.Uphill, WeatherCondition.Cloudy),
    new Run(2, 1, 4, Track.Bliznec_Brestovac, TimeSpan.FromSeconds(300), DateTime.Now.AddDays(-2), DriveDirection.Downhill, WeatherCondition.Cloudy),
    new Run(3, 2, 2, Track.Bistra_Sljeme, TimeSpan.FromSeconds(340), DateTime.Now.AddDays(-3), DriveDirection.Uphill, WeatherCondition.Rainy),
    new Run(4, 3, 3, Track.Stubica_Sljeme, TimeSpan.FromSeconds(310), DateTime.Now.AddDays(-1), DriveDirection.Downhill, WeatherCondition.Sunny),
    new Run(5, 4, 5, Track.Gračani_Sljeme, TimeSpan.FromSeconds(295), DateTime.Now.AddDays(-4), DriveDirection.Uphill, WeatherCondition.Sunny)
};


drivers[0].Runs = new List<Run> { runs[0], runs[1] };
drivers[1].Runs = new List<Run> { runs[2] };
drivers[2].Runs = new List<Run> { runs[3] };
drivers[3].Runs = new List<Run> { runs[4] };
drivers[4].Runs = new List<Run>();


var notes = new List<RunNote>
{
    new RunNote(1, 1, "Stock RX-8 setup, stable through medium-speed corners.", DateTime.Now),
    new RunNote(2, 2, "Civic with MTS Technik feels sharper on turn-in.", DateTime.Now),
    new RunNote(3, 3, "Celica on stock suspension struggled a bit in wet conditions.", DateTime.Now)
};

Console.WriteLine("Podaci inicijalizirani.");
Console.WriteLine($"Timovi: {teams.Count}");
Console.WriteLine($"Vozači: {drivers.Count}");
Console.WriteLine($"Auti: {cars.Count}");
Console.WriteLine($"Runovi: {runs.Count}");

Console.WriteLine("\n--- LINQ UPITI ---");

// 1. Tri najbrža runa
var fastestRuns = runs
    .OrderBy(r => r.BestTime)
    .Take(3);

Console.WriteLine("\n1. Tri najbrža runa:");
foreach (var run in fastestRuns)
{
    var driver = drivers.FirstOrDefault(d => d.Id == run.DriverId);
    var car = cars.FirstOrDefault(c => c.Id == run.CarId);

    Console.WriteLine($"{driver?.Name} - {car?.Make} {car?.Model} - {run.BestTime} - {run.Track}");
}

// 2. Vozači s više od 5 godina iskustva
var experiencedDrivers = drivers
    .Where(d => d.YearsOfExperience > 5);

Console.WriteLine("\n2. Vozači s više od 5 godina iskustva:");
foreach (var driver in experiencedDrivers)
{
    Console.WriteLine($"{driver.Name} ({driver.YearsOfExperience} godina iskustva)");
}


// 3. Vozači i broj auta koje posjeduju
var driverCarCounts = drivers
    .Select(d => new
    {
        DriverName = d.Name,
        CarCount = d.CarsOwned?.Count ?? 0
    });

Console.WriteLine("\n3. Vozači i broj auta koje posjeduju:");
foreach (var item in driverCarCounts)
{
    Console.WriteLine($"{item.DriverName} - {item.CarCount} auta");
}

// 4. Auti sortirani po snazi silazno
var carsByHorsepower = cars
    .OrderByDescending(c => c.Horsepower);

Console.WriteLine("\n4. Auti sortirani po snazi:");
foreach (var car in carsByHorsepower)
{
    Console.WriteLine($"{car.Make} {car.Model} - {car.Horsepower} KS");
}
