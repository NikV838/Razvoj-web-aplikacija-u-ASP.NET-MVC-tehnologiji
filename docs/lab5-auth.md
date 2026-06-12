# Lab 5 Authentication Configuration

Local Identity accounts are enabled through the MVC `AccountController`.

The application seeds the `Admin` and `User` roles at startup. The built-in development admin account is created with `UserManager.CreateAsync`, not manual SQL.

Google and Facebook authentication are optional. The app starts normally when external credentials are missing, and the Account page disables each provider button until both values for that provider are configured.

Store Google credentials with user secrets:

```powershell
dotnet user-secrets init --project SljemeTimeAttack
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID" --project SljemeTimeAttack
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET" --project SljemeTimeAttack
```

Store Facebook credentials with user secrets:

```powershell
dotnet user-secrets set "Authentication:Facebook:AppId" "YOUR_APP_ID" --project SljemeTimeAttack
dotnet user-secrets set "Authentication:Facebook:AppSecret" "YOUR_APP_SECRET" --project SljemeTimeAttack
```

The local Google callback URL is:

```text
https://localhost:7081/signin-google
```

The local Facebook callback URL is:

```text
https://localhost:7081/signin-facebook
```

The port must match the app's actual HTTPS port. If the app runs on another HTTPS port, register that port in the Google Cloud or Facebook developer app settings.

`appsettings.json` should contain placeholders only:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Facebook": {
      "AppId": "",
      "AppSecret": ""
    }
  }
}
```

Do not commit real Google client secrets, Facebook App IDs/App Secrets, or production admin passwords.
