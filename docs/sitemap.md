# Sitemap ruta za Lab 3

Ovaj dokument opisuje dostupne URL-ove u ASP.NET Core MVC aplikaciji **SljemeTimeAttack**. Za svaku rutu navedeni su kontroler, akcija, pogled koji se koristi i kratka svrha stranice.

## Popis dostupnih URL-ova

| URL | Controller | Action | View used | Svrha |
| --- | --- | --- | --- | --- |
| `/` | `HomeController` | `Index` | `Views/Home/Index.cshtml` | Pocetna stranica aplikacije. |
| `/Home/Index` | `HomeController` | `Index` | `Views/Home/Index.cshtml` | Pocetna stranica dostupna preko zadane MVC rute. |
| `/drivers` | `DriverController` | `Index` | `Views/Driver/Index.cshtml` | Popis svih vozaca preko prilagodene Lab 3 rute. |
| `/drivers/{id}` | `DriverController` | `Details` | `Views/Driver/Details.cshtml` | Detalji odabranog vozaca preko prilagodene Lab 3 rute. |
| `/Driver/Index` | `DriverController` | `Index` | `Views/Driver/Index.cshtml` | Popis svih vozaca preko zadane MVC rute. |
| `/Driver/Details/{id}` | `DriverController` | `Details` | `Views/Driver/Details.cshtml` | Detalji odabranog vozaca preko zadane MVC rute. |
| `/cars` | `CarController` | `Index` | `Views/Car/Index.cshtml` | Popis svih automobila preko prilagodene Lab 3 rute. |
| `/Car/Index` | `CarController` | `Index` | `Views/Car/Index.cshtml` | Popis svih automobila preko zadane MVC rute. |
| `/Car/Details/{id}` | `CarController` | `Details` | `Views/Car/Details.cshtml` | Detalji odabranog automobila preko zadane MVC rute. |
| `/runs` | `RunController` | `Index` | `Views/Run/Index.cshtml` | Popis svih voznji preko prilagodene Lab 3 rute. |
| `/Run/Index` | `RunController` | `Index` | `Views/Run/Index.cshtml` | Popis svih voznji preko zadane MVC rute. |
| `/Run/Details/{id}` | `RunController` | `Details` | `Views/Run/Details.cshtml` | Detalji odabrane voznje preko zadane MVC rute. |
| `/Team/Index` | `TeamController` | `Index` | `Views/Team/Index.cshtml` | Popis svih timova preko zadane MVC rute. |
| `/teams/{id}` | `TeamController` | `Details` | `Views/Team/Details.cshtml` | Detalji odabranog tima preko prilagodene Lab 3 rute. |
| `/Team/Details/{id}` | `TeamController` | `Details` | `Views/Team/Details.cshtml` | Detalji odabranog tima preko zadane MVC rute. |

## Napomena o rutiranju

Aplikacija i dalje koristi zadanu MVC rutu:

```text
/{controller}/{action}/{id?}
```

Zbog toga su stranice i dalje dostupne preko URL-ova kao sto su `/Driver/Index`, `/Car/Details/{id}` i `/Team/Details/{id}`. Za Lab 3 dodane su posebne prilagodene rute, npr. `/drivers`, `/drivers/{id}`, `/cars`, `/runs` i `/teams/{id}`.

Zadana ruta ostaje vazna za normalnu MVC navigaciju, ali se ne racuna kao ispunjenje zahtjeva za prilagodeno rutiranje u Lab 3. Taj zahtjev pokrivaju samo eksplicitno definirane prilagodene rute.
