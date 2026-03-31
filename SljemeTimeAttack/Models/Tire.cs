#nullable enable
namespace SljemeTimeAttack.Models
{
    public class Tire
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public double SizeInMm { get; set; }
        public string Dot { get; set; }
        public Rim Rim { get; set; }

        //public Tire() { }

        public Tire(int id, string brand, string model, string type, double sizeInMm, string dot, Rim rim)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Type = type;
            SizeInMm = sizeInMm;
            Dot = dot;
            Rim = rim;
        }
    }
}
