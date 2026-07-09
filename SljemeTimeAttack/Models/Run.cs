using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SljemeTimeAttack.Enums;

namespace SljemeTimeAttack.Models
{
    public class Run
    {
        [Key]
        public int Id { get; set; }
        public int? DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }
        public int? CarId { get; set; }
        [ForeignKey(nameof(CarId))]
        public Car? Car { get; set; }
        public string? DriverNameSnapshot { get; set; }
        public string? CarMakeSnapshot { get; set; }
        public string? CarModelSnapshot { get; set; }
        public string? CarRegistrationNumberSnapshot { get; set; }
        public string? CarDisplayNameSnapshot { get; set; }
        public Track Track { get; set; }
        public TimeSpan BestTime { get; set; }
        public DateTime Date { get; set; }
        public DriveDirection Direction { get; set; }
        public WeatherCondition Weather { get; set; }
        public ICollection<RunFile> Files { get; set; }

        public string DriverDisplayName => Driver?.Name ?? DriverNameSnapshot ?? "Deleted driver";

        public string CarDisplayName
        {
            get
            {
                if (Car != null)
                {
                    return $"{Car.Make} {Car.Model}";
                }

                if (!string.IsNullOrWhiteSpace(CarDisplayNameSnapshot))
                {
                    return CarDisplayNameSnapshot;
                }

                var snapshotName = $"{CarMakeSnapshot} {CarModelSnapshot}".Trim();
                return string.IsNullOrWhiteSpace(snapshotName) ? "Deleted car" : snapshotName;
            }
        }

        public string CarRegistrationDisplay => Car?.RegistrationNumber ?? CarRegistrationNumberSnapshot ?? "N/A";

        public Run()
        {
            Driver = null!;
            Car = null!;
            Files = new List<RunFile>();
        }

        public Run(int id, int driverId, int carId, Track track, TimeSpan bestTime, DateTime date, DriveDirection direction, WeatherCondition weather)
        {
            Id = id;
            DriverId = driverId;
            Driver = null!;
            CarId = carId;
            Car = null!;
            Track = track;
            BestTime = bestTime;
            Date = date;
            Direction = direction;
            Weather = weather;
            Files = new List<RunFile>();
        }
    }
}
