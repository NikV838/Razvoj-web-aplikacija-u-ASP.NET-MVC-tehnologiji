#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SljemeTimeAttack.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int YearsOfExperience { get; set; }
        public int? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual ICollection<Car> CarsOwned { get; set; }
        public virtual ICollection<Run> Runs { get; set; }

        public Driver()
        {
            Username = string.Empty;
            Name = string.Empty;
            CarsOwned = new List<Car>();
            Runs = new List<Run>();
        }

        public Driver(
            int id,
            string username,
            string name,
            int age,
            int yearsOfExperience,
            Team? team,
            string? email,
            string? phoneNumber,
            List<Car>? carsOwned = null,
            List<Run>? runs = null)
        {
            Id = id;
            Username = username;
            Name = name;
            Age = age;
            YearsOfExperience = yearsOfExperience;
            TeamId = team?.Id;
            Team = team;
            Email = email;
            PhoneNumber = phoneNumber;
            CarsOwned = carsOwned ?? new List<Car>();
            Runs = runs ?? new List<Run>();
        }
    }
}
