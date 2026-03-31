#nullable enable
using System;
using System.Collections.Generic;

namespace SljemeTimeAttack.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string? Sponsor { get; set; }

        public List<Driver> Drivers { get; set; }

        public Team()
        {
            Drivers = new List<Driver>();
        }

        public Team(int id, string name, string country, string? sponsor)
            : this()
        {
            Id = id;
            Name = name;
            Country = country;
            Sponsor = sponsor;
        }
    }
}
