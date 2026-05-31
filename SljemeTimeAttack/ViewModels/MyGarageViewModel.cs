using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.ViewModels;

public class MyGarageViewModel
{
    public string UserName { get; set; } = string.Empty;

    public Driver? Driver { get; set; }

    public IReadOnlyCollection<Car> Cars { get; set; } = [];

    public IReadOnlyCollection<Run> Runs { get; set; } = [];
}
