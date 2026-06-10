# Lab 5 Authentication Configuration

Local Identity accounts are enabled through the MVC `AccountController`.

The application seeds the `Admin` and `User` roles at startup. The built-in development admin account is created with `UserManager.CreateAsync`, not manual SQL.

Google authentication is optional. The app starts normally when Google credentials are missing, and the Account page disables the Google button until both values are configured.

Store Google credentials with user secrets:

```powershell
dotnet user-secrets init --project SljemeTimeAttack
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID" --project SljemeTimeAttack
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET" --project SljemeTimeAttack
```

The local Google callback URL is:

```text
https://localhost:7081/signin-google
```

The port must match the app's actual HTTPS port. If the app runs on another HTTPS port, register that port in the Google Cloud OAuth client.

`appsettings.json` should contain placeholders only:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    }
  }
}
```

Do not commit real Google client secrets or production admin passwords.
