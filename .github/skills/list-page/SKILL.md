---
name: list-page
description: Use this skill when creating or updating ASP.NET Core MVC Index/list pages in the SljemeTimeAttack project, including controller Index actions, Razor collection views, Details navigation, and motorsport dashboard UI consistency.
---

# MVC List Page Skill

Use this skill for `Index` pages that list entities such as `Driver`, `Car`, `Run`, and `Team` in the `SljemeTimeAttack` ASP.NET Core MVC app.

## List Page Workflow

1. Inspect the matching model, repository, controller, existing `Index.cshtml`, `Details.cshtml`, and page-specific CSS before editing.
2. Keep the `Index` action focused on loading collection data and returning the view.
3. Pass a strongly typed collection to the Razor view.
4. Render the collection with clear scan-friendly layout.
5. Link each item to its `Details` page.
6. Preserve the custom motorsport dashboard style.

## Controller Rules

- Use constructor dependency injection for repositories.
- Keep `Index()` simple: call the repository, store the returned collection in a local variable, and `return View(collection)`.
- Do not put formatting, sorting UI, or presentation markup in controllers.
- Ensure repositories load related data needed by the list view before the model reaches Razor.

Example pattern:

```csharp
public IActionResult Index()
{
    var drivers = _driverRepository.GetAll();
    return View(drivers);
}
```

## Razor View Rules

- Use a strongly typed collection model, for example:

```csharp
@model IEnumerable<SljemeTimeAttack.Models.Driver>
```

- Set `ViewData["Title"]` to the page name.
- Use `@foreach (var item in Model)` to render list items.
- Keep null handling readable for optional related data.
- Do not query repositories or data stores from Razor when the controller/repository can provide the needed data.
- Prefer tag helpers for links instead of hard-coded URLs.

## Details Navigation

- Each list item should provide a clear path to the matching details page.
- Use MVC tag helpers:

```html
<a asp-controller="Driver" asp-action="Details" asp-route-id="@driver.Id">
    Details
</a>
```

- A whole-card or whole-row link is acceptable when it is visually clear.
- Include a visible `Details` button or obvious clickable item affordance when the layout is not self-evident.
- Do not add `Create`, `Edit`, or `Delete` actions unless the user explicitly requests CRUD behavior.

## UI Style Rules

- Match the SljemeTimeAttack motorsport dashboard look: dark surfaces, strong hierarchy, racing/data-oriented spacing, and high-contrast details.
- Prefer custom page classes and page-specific CSS files such as `driver-pages.css`, `car-pages.css`, `run-pages.css`, or `team-pages.css`.
- Use cards, grids, tables, or structured list rows based on the data density.
- Make key fields easy to scan: name/title, team or category, numeric performance values, and badges/status labels.
- Keep buttons clear and action-oriented, especially `Details`.
- Avoid the plain default Bootstrap scaffold/template look.
- Bootstrap utilities are fine, but the final page should feel like the existing custom motorsport UI.

## Before Finishing

- Confirm the `Index` route still maps to the expected controller/action.
- Confirm every rendered item can navigate to `Details`.
- Check that related data used by the view is loaded safely.
- Run `dotnet build` when practical.
