using System;
using SljemeTimeAttack.Enums;

namespace SljemeTimeAttack.Models
{
    public class Run
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int CarId { get; set; }
        public Track Track { get; set; }
        public TimeSpan BestTime { get; set; }
        public DateTime Date { get; set; }
        public DriveDirection Direction { get; set; }
        public WeatherCondition Weather { get; set; }

        public Run() { }

        public Run(int id, int driverId, int carId, Track track, TimeSpan bestTime, DateTime date, DriveDirection direction, WeatherCondition weather)
        {
            Id = id;
            DriverId = driverId;
            CarId = carId;
            Track = track;
            BestTime = bestTime;
            Date = date;
            Direction = direction;
            Weather = weather;
        }
    }
}
