namespace SljemeTimeAttack.Services
{
    public interface IAiCarParserService
    {
        Task<ParsedCarPrompt> ParseAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
