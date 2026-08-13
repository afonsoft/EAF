# EAF — Template Migration and Update Spec

## Summary

Ongoing migration and update specification for EAF templates (API, Angular, Worker, Gateway) covering version alignment, dependency upgrades, breaking changes and new feature integration.

## Current Baseline

- Backend: .NET 10.0, ASP.NET Boilerplate 10.5.0, EF Core 10.0, Hangfire.
- Angular: Angular 20, TypeScript 5.8, PrimeNG 17.17.0, ngx-bootstrap 12.
- Middleware: 14 modules under `src/`.

## Objectives

1. Keep template-generated projects in sync with the latest EAF middleware releases.
2. Simplify the upgrade path by documenting package changes, migrations and config updates.
3. Remove legacy boilerplate and adopt modern ABP/EAF patterns.

## Backend Updates

### .NET 10 / ABP 10.5
- Use `EafModule` pattern with `[DependsOn]`.
- Configure `Program.cs`:
  ```csharp
  builder.Services.AddEaf<MyModule>(...);
  var app = builder.Build();
  app.UseEaf();
  ```
- Replace `IWebHost`/`IHost` manual setup with minimal hosting model.

### EF Core
- Use `IDesignTimeDbContextFactory<>` for migrations.
- Add `Pluralize` extension conventions if needed.
- Use `DbContext` pooling in high-throughput scenarios.

### Configuration
- Use `EafConfiguration` section.
- Validate options with `IOptions<T>`.

## Angular Updates

### Dependency Alignment
- Keep `package.json` aligned with `Templates/Angular/Eaf.ProjectName.UI/package.json`.
- Do not pin versions lower than the template.

### Module Loading
- Adopt lazy loading for admin areas.
- Use standalone components where Angular 20 supports it without breaking ABP integration.

### Service Proxies
- Regenerate with NSwag after every API change.
- Do not hand-edit `service-proxies.ts`.

### Theming
- Plan migration to CSS variables and dark mode (see `eaf-angular-dark-mode-theming.spec.md`).

## Worker / Gateway

- Worker uses `Eaf.Middleware.Worker`.
- Gateway uses `Eaf.Gateways.API` (YARP or Ocelot configuration).
- Add health checks and OpenTelemetry in both.

## Breaking Change Register

| Version | Change | Mitigation |
|---|---|---|
| 9.4.x → 9.5.x | `EafMiddlewareWebModule` namespace | Update `using` statements |
| 9.5.x → 10.x | .NET 9 → .NET 10 | Update global.json and runtime |
| 10.x → current | PrimeNG 16 → 17 | Follow PrimeNG migration guide |

## Implementation Status (2026-08)

In progress. The repository already targets .NET 10 and Angular 20. Migration guide is being consolidated across multiple `eaf-template-migration-*.md` files.

## Migration Plan
1. Consolidate all migration notes into `docs/migration/` or keep them under `.specs/`.
2. Add a version matrix (`EAF version ↔ .NET ↔ ABP ↔ Angular ↔ PrimeNG`).
3. Automate template updates via a GitHub Actions workflow.
4. Provide sample diff for each major version.

## Impact
- **Low**: documentation and process.
- **High**: reduces upgrade friction for EAF consumers.

## Risks
- Multiple migration files can become out of date; assign an owner to refresh each release.

## References
- `common.props`
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
- `src/Eaf.Middleware.Web.Core/EafMiddlewareWebModule.cs`
