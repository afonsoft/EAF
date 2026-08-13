# EAF Template Migration 9.4.1

## Summary

Guide for updating projects generated from the EAF template to version **9.4.1**, including package upgrades, configuration changes and migration steps for the Angular UI and .NET backend.

## Scope

- Update `Eaf.*` NuGet packages to `9.4.1`.
- Update Angular dependencies to compatible versions.
- Adjust breaking changes introduced between previous releases and 9.4.1.

## Backend Migration

### 1. NuGet Packages
Update all `Eaf.*` package references to `9.4.1`:
```xml
<PackageReference Include="Eaf.Middleware.Core" Version="9.4.1" />
<PackageReference Include="Eaf.Middleware.Web.Core" Version="9.4.1" />
```

### 2. Program / Startup
- Ensure `app.UseEaf(...)` and module registration match the 9.4.1 pattern.
- Replace obsolete configuration keys with the new `EafConfiguration` section.

### 3. Entity Framework
- Apply any new migrations:
  ```bash
  dotnet ef migrations add Eaf_9_4_1
  dotnet ef database update
  ```
- Check for new entities (e.g. `TenantJoinRequest`, `PaymentSubscription`, `AuditLog` fields).

### 4. AppSettings
- Add new optional settings such as `PaymentGateway`, `OpenTelemetry`, `KeyVault` sections.

## Angular Migration

### 1. npm Packages
```bash
npm install eaf-web-resources@^9.4.1 eaf-ngx@^9.4.1
```

### 2. PrimeNG / ngx-bootstrap
- PrimeNG 17 remains the target.
- `ngx-bootstrap` 12 is still supported in 9.4.1.

### 3. Service Proxies
- Regenerate `service-proxies.ts` with NSwag after backend build:
  ```bash
  nswag run
  ```

### 4. Theme / Assets
- Copy updated theme files from the template if custom themes were modified.

## Breaking Changes

| From | To | Action |
|---|---|---|
| `IConfiguration.GetSection("Eaf")` | `IConfiguration.GetSection("EafConfiguration")` | Rename in `appsettings.json` |
| `PaymentGatewayManager` | `PaymentGatewayResolver` | Update DI usage |
| `MassNotificationAppService.Create` signature | `CreateAsync` | Rename calls in tests and UI |

## Implementation Status (2026-08)

This spec is a migration guide. The current EAF repository already targets versions newer than 9.4.1. Verify the exact current version in `common.props` or `Directory.Build.props` before applying this guide.

## References
- `common.props`
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
