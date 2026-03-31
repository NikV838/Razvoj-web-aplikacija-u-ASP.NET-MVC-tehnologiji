namespace SljemeTimeAttack.Models
{
    public class Rim
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public double SizeInJ { get; set; }
        public string Material { get; set; }

        //public Rim() { }

        public Rim(int id, string make, string model, double sizeInJ, string material)
        {
            Id = id;
            Make = make;
            Model = model;
            SizeInJ = sizeInJ;
            Material = material;
        }
    }
}
