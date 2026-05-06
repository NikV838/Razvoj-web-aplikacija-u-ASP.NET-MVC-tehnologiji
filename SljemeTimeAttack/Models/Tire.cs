#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SljemeTimeAttack.Models
{
    public class Tire
    {
        [Key]
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public double SizeInMm { get; set; }
        public string Dot { get; set; }
        public int RimId { get; set; }
        [ForeignKey(nameof(RimId))]
        public Rim Rim { get; set; }

        public Tire()
        {
            Brand = string.Empty;
            Model = string.Empty;
            Type = string.Empty;
            Dot = string.Empty;
            Rim = null!;
        }

        public Tire(int id, string brand, string model, string type, double sizeInMm, string dot, Rim rim)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Type = type;
            SizeInMm = sizeInMm;
            Dot = dot;
            RimId = rim.Id;
            Rim = rim;
        }
    }
}
