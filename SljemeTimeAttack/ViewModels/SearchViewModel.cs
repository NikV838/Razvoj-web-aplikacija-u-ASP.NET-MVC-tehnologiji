using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.ViewModels;

public class SearchViewModel
{
    public string Query { get; set; } = string.Empty;

    public List<Car> Cars { get; set; } = [];

    public List<Driver> Drivers { get; set; } = [];

    public List<Team> Teams { get; set; } = [];

    public List<Run> Runs { get; set; } = [];

    public List<Tire> Tires { get; set; } = [];

    public List<Rim> Rims { get; set; } = [];

    public List<Suspension> Suspensions { get; set; } = [];

    public bool HasQuery => !string.IsNullOrWhiteSpace(Query);

    public bool HasResults =>
        Cars.Count > 0 ||
        Drivers.Count > 0 ||
        Teams.Count > 0 ||
        Runs.Count > 0 ||
        Tires.Count > 0 ||
        Rims.Count > 0 ||
        Suspensions.Count > 0;
}
