#nullable enable
namespace SljemeTimeAttack.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Horsepower { get; set; }
        public double WeightKg { get; set; }
        public int Year { get; set; }

        public Tire WheelSetup { get; set; }
        public Suspension Suspension { get; set; }

        public Car() { }

        public Car(int id, string make, string model, int horsepower, double weightKg, int year, Tire wheelSetup, Suspension suspension)
        {
            Id = id;
            Make = make;
            Model = model;
            Horsepower = horsepower;
            WeightKg = weightKg;
            Year = year;
            WheelSetup = wheelSetup;
            Suspension = suspension;
        }
    }
}