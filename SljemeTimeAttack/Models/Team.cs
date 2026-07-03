#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string? Sponsor { get; set; }
        public string? ImagePath { get; set; }

        public virtual ICollection<Driver> Drivers { get; set; }

        public Team()
        {
            Name = string.Empty;
            Country = string.Empty;
            Drivers = new List<Driver>();
        }

        public Team(int id, string name, string country, string? sponsor)
        {
            Id = id;
            Name = name;
            Country = country;
            Sponsor = sponsor;
            Drivers = new List<Driver>();
        }
    }
}
