#nullable enable
namespace SljemeTimeAttack.Models
{
    public class Suspension
    {
        public int Id { get; set; }
        public string Type { get; set; } // Coilover, Air, Stock
        public string Brand { get; set; }

        public bool HasFrontStrutBar { get; set; }
        public bool HasRearStrutBar { get; set; }

        public double RideHeightMm { get; set; }

        public bool IsHeightAdjustable { get; set; }
        public bool IsStiffnessAdjustable { get; set; }

        public double? FrontStiffness { get; set; }
        public double? RearStiffness { get; set; }

        //public Suspension() { }

        public Suspension(int id, string type, string brand, bool hasFrontStrutBar, bool hasRearStrutBar, double rideHeightMm, bool isHeightAdjustable, bool isStiffnessAdjustable, double? frontStiffness, double? rearStiffness)
        {
            Id = id;
            Type = type;
            Brand = brand;
            HasFrontStrutBar = hasFrontStrutBar;
            HasRearStrutBar = hasRearStrutBar;
            RideHeightMm = rideHeightMm;
            IsHeightAdjustable = isHeightAdjustable;
            IsStiffnessAdjustable = isStiffnessAdjustable;
            FrontStiffness = frontStiffness;
            RearStiffness = rearStiffness;
        }
    }
}
