#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SljemeTimeAttack.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Horsepower { get; set; }
        public double WeightKg { get; set; }
        public int Year { get; set; }
        public string RegistrationNumber { get; set; }
        public int? DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }
        public int TireId { get; set; }
        [ForeignKey(nameof(TireId))]
        public Tire WheelSetup { get; set; }
        public int SuspensionId { get; set; }
        [ForeignKey(nameof(SuspensionId))]
        public Suspension Suspension { get; set; }

        public Car()
        {
            Make = string.Empty;
            Model = string.Empty;
            RegistrationNumber = string.Empty;
            WheelSetup = null!;
            Suspension = null!;
        }

        public Car(int id, string make, string model, int horsepower, double weightKg, int year, string registrationNumber, Tire wheelSetup, Suspension suspension)
        {
            Id = id;
            Make = make;
            Model = model;
            Horsepower = horsepower;
            WeightKg = weightKg;
            Year = year;
            RegistrationNumber = registrationNumber;
            TireId = wheelSetup.Id;
            WheelSetup = wheelSetup;
            SuspensionId = suspension.Id;
            Suspension = suspension;
        }
    }
}
