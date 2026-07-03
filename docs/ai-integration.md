# AI-assisted car entry

SljemeTimeAttack includes a small AI assistant on the Car Create page. It reads a natural-language car description and fills the existing create form fields. It does not create or save a car by itself; the user still reviews the form and clicks **Create car**.

## Endpoint

The UI posts JSON to:

```http
POST /Car/ParseCarPrompt
```

Request body:

```json
{
  "prompt": "Add a 2004 Mazda RX-8 with 231 horsepower, 1310 kg, registration ZG-RX8-04."
}
```

Response fields:

```json
{
  "make": "Mazda",
  "model": "RX-8",
  "year": 2004,
  "horsepower": 231,
  "weightKg": 1310,
  "registrationNumber": "ZG-RX8-04",
  "source": "fallback"
}
```

## Configuration

The app reads the AI key from configuration:

```bash
dotnet user-secrets set "Ai:ApiKey" "YOUR_API_KEY" --project SljemeTimeAttack
```

Optional settings:

```bash
dotnet user-secrets set "Ai:Model" "gpt-4o-mini" --project SljemeTimeAttack
dotnet user-secrets set "Ai:Endpoint" "https://api.openai.com/v1/chat/completions" --project SljemeTimeAttack
```

Do not store real API keys in source control.

## Fallback mode

If `Ai:ApiKey` is missing, or the AI request fails, the app uses a local regex-based parser. The fallback parser can detect common fields such as make, model, year, horsepower, weight in kg, and registration number. This keeps the lab demo usable without external services.

## Files

- `Services/IAiCarParserService.cs` defines the parser abstraction.
- `Services/AiCarParserService.cs` calls the configured AI endpoint and falls back to local parsing.
- `Controllers/CarController.cs` exposes `ParseCarPrompt`.
- `Views/Car/Create.cshtml` contains the assistant panel.
- `wwwroot/js/car-ai-fill.js` calls the endpoint and fills the form.
