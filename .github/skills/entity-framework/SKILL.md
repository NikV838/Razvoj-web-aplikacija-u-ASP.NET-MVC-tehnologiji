---
name: entity-framework
description: Use this skill when making Entity Framework Core changes in the SljemeTimeAttack ASP.NET Core MVC project, including DbContext updates, SQL Server migrations, repository changes, seed data, dependency injection, and database update commands.
---

# Entity Framework Core Skill

Use this skill for EF Core work in `SljemeTimeAttack`, especially when adding or changing entities, relationships, migrations, repositories, seed data, or database setup.

## Project Context

- ASP.NET Core MVC app using Entity Framework Core with SQL Server.
- Main DbContext: `SljemeTimeAttack/Data/SljemeTimeAttackDbContext.cs`.
- EF repositories live in `SljemeTimeAttack/Repos/`.
- Migrations live in `SljemeTimeAttack/Migrations/`.
- Services are registered with constructor dependency injection in `Program.cs`.
- A Docker SQL Server setup exists for local database work.

## EF Change Workflow

1. Inspect the current model classes, `SljemeTimeAttackDbContext`, repositories, migrations, and relevant views/controllers before editing.
2. Make model changes first, then update `SljemeTimeAttackDbContext`.
3. Update repositories to expose the data needed by controllers/views.
4. Add or update seed data only when the schema or baseline demo data requires it.
5. Generate a migration after code changes.
6. Review the generated migration and model snapshot before applying it.
7. Apply the migration only after confirming it matches the intended schema/data changes.

## DbContext Rules

- Add new `DbSet<T>` properties for new persisted entities.
- Configure relationships, keys, required fields, precision, and delete behavior in `OnModelCreating` when conventions are not clear enough.
- Keep configuration close to existing project style.
- Do not remove existing DbContext configuration unless the task explicitly requires it.
- Keep seed data deterministic and compatible with migrations.

## Migration Rules

- Create migrations from the project root or specify the project explicitly.
- Use clear migration names that describe the change.
- Always inspect generated migration files before running `database update`.
- Check both the migration file and `SljemeTimeAttackDbContextModelSnapshot.cs`.
- Do not hand-edit migrations unless needed to fix data operations, rename operations, or EF-generated output that would otherwise lose data.
- Avoid applying migrations that drop or recreate data unexpectedly.

Example commands:

```powershell
dotnet ef migrations add AddDriverNationality --project SljemeTimeAttack
dotnet ef database update --project SljemeTimeAttack
```

## Repository Rules

- Keep repository classes in `SljemeTimeAttack/Repos/`.
- Use constructor dependency injection for `SljemeTimeAttackDbContext`.
- Return model data needed by controllers without putting UI logic in repositories.
- Use `Include()` and `ThenInclude()` when related data is needed by details pages, list pages, or views.
- Prefer focused methods such as `GetAll()` and `GetById(int id)` when matching the existing repository style.
- Keep `MockDataStore` unless the user explicitly asks to remove it or replace it fully.

## Seed Data Rules

- Add seed data through `OnModelCreating` using EF Core `HasData` when it should be part of migrations.
- Use fixed `DateTime` values. Do not use `DateTime.Now`, `DateTime.UtcNow`, or other runtime-generated values in seed data.
- Ensure seeded foreign keys match existing seeded primary keys.
- Keep seed data small, readable, and useful for local development/demo scenarios.
- Generate and inspect a migration after seed data changes.

## Dependency Injection Rules

- Register new EF repositories in `Program.cs` with constructor dependency injection.
- Follow the current service lifetime pattern used by existing repositories.
- Controllers should request repositories through constructors, not instantiate DbContext or repositories manually.

## Before Finishing

- Confirm application code, migrations, repositories, and seed data are consistent.
- Run a build when possible:

```powershell
dotnet build
```

- If database changes were made, document whether `dotnet ef database update --project SljemeTimeAttack` was run.
