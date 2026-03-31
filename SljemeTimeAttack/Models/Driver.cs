#nullable enable
using System;
using System.Collections.Generic;

namespace SljemeTimeAttack.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int YearsOfExperience { get; set; }
        public Team? Team { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public List<Car>? CarsOwned { get; set; }
        public List<Run>? Runs { get; set; }

        public Driver()
        {
            CarsOwned = null;
            Runs = null;
        }

        public Driver(int id, string username, string name, int age, int yearsOfExperience, Team? team, string? email, string? phoneNumber, List<Car>? carsOwned = null, List<Run>? runs = null)
            : this()
        {
            Id = id;
            Username = username;
            Name = name;
            Age = age;
            YearsOfExperience = yearsOfExperience;
            Team = team;
            Email = email;
            PhoneNumber = phoneNumber;
            CarsOwned = carsOwned;
            Runs = runs;
        }
    }
}
