namespace SljemeTimeAttack.Services
{
    public class ParsedCarPrompt
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public int? Horsepower { get; set; }
        public double? WeightKg { get; set; }
        public string? RegistrationNumber { get; set; }
        public string Source { get; set; } = "fallback";

        public bool HasAnyField =>
            !string.IsNullOrWhiteSpace(Make) ||
            !string.IsNullOrWhiteSpace(Model) ||
            Year.HasValue ||
            Horsepower.HasValue ||
            WeightKg.HasValue ||
            !string.IsNullOrWhiteSpace(RegistrationNumber);
    }
}
