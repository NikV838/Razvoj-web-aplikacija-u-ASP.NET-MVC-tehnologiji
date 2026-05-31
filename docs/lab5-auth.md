# Lab 5 Authentication Configuration

Local Identity accounts are enabled through the MVC `AccountController`.

The application seeds the `Admin` and `Racer` roles at startup. To seed an admin account, set these values with user secrets, environment variables, or a local-only settings file:

```json
{
  "SeedAdmin": {
    "Email": "admin@example.com",
    "Password": "replace-with-a-strong-password",
    "DisplayName": "Sljeme Admin"
  }
}
```

Google authentication is prepared but disabled until both values are provided:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "google-client-id",
      "ClientSecret": "google-client-secret"
    }
  }
}
```

Do not commit real Google client secrets or production admin passwords.
