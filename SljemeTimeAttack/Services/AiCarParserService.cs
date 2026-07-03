using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SljemeTimeAttack.Services
{
    public class AiCarParserService : IAiCarParserService
    {
        private static readonly RegexOptions ParserOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiCarParserService> _logger;

        public AiCarParserService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<AiCarParserService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ParsedCarPrompt> ParseAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["Ai:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ParseWithFallback(prompt);
            }

            try
            {
                var aiResult = await ParseWithAiAsync(prompt, apiKey, cancellationToken);
                if (aiResult?.HasAnyField == true)
                {
                    aiResult.Source = "ai";
                    return aiResult;
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "AI car parser failed. Falling back to local parser.");
            }

            return ParseWithFallback(prompt);
        }

        private async Task<ParsedCarPrompt?> ParseWithAiAsync(string prompt, string apiKey, CancellationToken cancellationToken)
        {
            var endpoint = _configuration["Ai:Endpoint"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                endpoint = "https://api.openai.com/v1/chat/completions";
            }

            var model = _configuration["Ai:Model"];
            if (string.IsNullOrWhiteSpace(model))
            {
                model = "gpt-4o-mini";
            }

            var payload = new
            {
                model,
                temperature = 0,
                response_format = new { type = "json_object" },
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Extract car data from the user's text. Return only JSON with keys Make, Model, Year, Horsepower, WeightKg, RegistrationNumber. Use null for unknown fields."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("AI car parser returned {StatusCode}. Falling back to local parser.", response.StatusCode);
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseBody);
            var content = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return string.IsNullOrWhiteSpace(content)
                ? null
                : ParseCarJson(content);
        }

        private static ParsedCarPrompt ParseCarJson(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            return new ParsedCarPrompt
            {
                Make = GetString(root, "Make"),
                Model = GetString(root, "Model"),
                Year = GetInt(root, "Year"),
                Horsepower = GetInt(root, "Horsepower"),
                WeightKg = GetDouble(root, "WeightKg"),
                RegistrationNumber = GetString(root, "RegistrationNumber")
            };
        }

        private static ParsedCarPrompt ParseWithFallback(string prompt)
        {
            var result = new ParsedCarPrompt { Source = "fallback" };
            var normalized = Regex.Replace(prompt.Trim(), @"\s+", " ");

            var yearMatch = Regex.Match(normalized, @"\b(19|20)\d{2}\b", ParserOptions);
            if (yearMatch.Success && int.TryParse(yearMatch.Value, out var year))
            {
                result.Year = year;
            }

            var horsepowerMatch = Regex.Match(normalized, @"\b(?<hp>\d{2,4})\s*(hp|horsepower|horses|ks)\b", ParserOptions);
            if (horsepowerMatch.Success && int.TryParse(horsepowerMatch.Groups["hp"].Value, out var horsepower))
            {
                result.Horsepower = horsepower;
            }

            var weightMatch = Regex.Match(normalized, @"\b(?<weight>\d{3,5}(?:[.,]\d+)?)\s*(kg|kilograms)\b", ParserOptions);
            if (weightMatch.Success && double.TryParse(weightMatch.Groups["weight"].Value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var weight))
            {
                result.WeightKg = weight;
            }

            var registrationMatch = Regex.Match(
                normalized,
                @"\b(registration(?:\s+number)?|reg|plate)\s*(is|number|:)?\s*(?<registration>[A-Z0-9][A-Z0-9 -]{2,20})",
                ParserOptions);
            if (registrationMatch.Success)
            {
                result.RegistrationNumber = CleanRegistration(registrationMatch.Groups["registration"].Value);
            }

            ApplyMakeAndModel(normalized, result);
            return result;
        }

        private static void ApplyMakeAndModel(string text, ParsedCarPrompt result)
        {
            var makes = new[]
            {
                "Alfa Romeo", "Volkswagen", "Mercedes", "Mitsubishi", "Chevrolet", "Subaru",
                "Toyota", "Mazda", "Honda", "Nissan", "Renault", "Peugeot", "Opel",
                "Hyundai", "Audi", "Ford", "Fiat", "BMW", "Kia", "VW"
            };

            foreach (var make in makes.OrderByDescending(make => make.Length))
            {
                var match = Regex.Match(
                    text,
                    $@"\b{Regex.Escape(make)}\b\s+(?<model>[A-Z0-9][A-Z0-9\- ]*?)(?=\s+(with|having|registration|reg|plate)\b|[,.;]|$)",
                    ParserOptions);

                if (!match.Success)
                {
                    continue;
                }

                result.Make = string.Equals(make, "VW", StringComparison.OrdinalIgnoreCase) ? "Volkswagen" : make;
                result.Model = CleanModel(match.Groups["model"].Value);
                return;
            }
        }

        private static string? CleanModel(string value)
        {
            var cleaned = Regex.Replace(value, @"\b(19|20)\d{2}\b.*$", string.Empty, ParserOptions);
            cleaned = Regex.Replace(cleaned, @"\b\d{2,4}\s*(hp|horsepower|horses|ks|kg)\b.*$", string.Empty, ParserOptions);
            cleaned = cleaned.Trim(' ', ',', '.', ';', ':');
            return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
        }

        private static string? CleanRegistration(string value)
        {
            var cleaned = value.Trim(' ', ',', '.', ';', ':').ToUpperInvariant();
            return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
        }

        private static string? GetString(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var property) &&
                !root.TryGetProperty(char.ToLowerInvariant(propertyName[0]) + propertyName[1..], out property))
            {
                return null;
            }

            return property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        }

        private static int? GetInt(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var property) &&
                !root.TryGetProperty(char.ToLowerInvariant(propertyName[0]) + propertyName[1..], out property))
            {
                return null;
            }

            if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var number))
            {
                return number;
            }

            return property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out number)
                ? number
                : null;
        }

        private static double? GetDouble(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var property) &&
                !root.TryGetProperty(char.ToLowerInvariant(propertyName[0]) + propertyName[1..], out property))
            {
                return null;
            }

            if (property.ValueKind == JsonValueKind.Number && property.TryGetDouble(out var number))
            {
                return number;
            }

            return property.ValueKind == JsonValueKind.String &&
                double.TryParse(property.GetString()?.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out number)
                    ? number
                    : null;
        }
    }
}
