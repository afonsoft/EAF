---
name: eaf-template
description: Expert guidance for creating, maintaining, and modernizing EAF (Enterprise Application Foundation) project templates (API, Angular, Worker). Covers template structure, nswag TypeScript generation, common.props, Dockerfile, CI/CD alignment, and spec-driven template updates. Use this skill when scaffolding, updating, or comparing EAF templates against ABP Boilerplate / ASP.NET Zero practices. Do NOT use for general frontend or backend feature development.
metadata:
  version: '1.0.0'
---

# EAF Template Development Skill

You are an expert in EAF project templates. You create, maintain, and modernize the templates under `Templates/` so new EAF-based projects start with up-to-date middleware, UI, Docker, and CI/CD patterns.

## Project Context

EAF is an open-source middleware platform built on ASP.NET Boilerplate (ABP) for .NET 10. The `Templates/` folder contains the scaffolds used to generate new solutions.

### Template Types

| Template | Path | Purpose |
|---|---|---|
| API | `Templates/Api/` | ASP.NET Core Web API + ABP middleware |
| Angular | `Templates/Angular/Eaf.ProjectName.UI/` | Angular 20 admin UI consuming EAF API |
| Worker | `Templates/Worker/` | .NET worker service background job host |

### Technology Stack

- **Backend**: .NET 10, ABP 10.5, EF Core 10, Castle Windsor, Hangfire
- **Frontend**: Angular 20, TypeScript 5.8, PrimeNG 17.17.0, `ngx-bootstrap` 12 (legacy), RxJS 7, Chart.js
- **Build**: Angular CLI, `nswag` for TypeScript proxy generation
- **PWA**: `@angular/pwa` / `@angular/service-worker` installed but not fully configured
- **CI/CD**: `publish-all.yml`, `build-all.yml`, Docker builds in `.github/workflows`

## Template Structure

### API Template

```text
Templates/Api/
├── Eaf.ProjectName.sln
├── src/
│   ├── Eaf.ProjectName.Web.Core/
│   ├── Eaf.ProjectName.Web.Host/
│   └── Eaf.ProjectName.EntityFrameworkCore/
└── ...
```

### Angular Template

```text
Templates/Angular/Eaf.ProjectName.UI/
├── src/
│   ├── app/
│   │   ├── account/
│   │   ├── admin/
│   │   ├── main/
│   │   ├── shared/
│   │   └── core/
│   ├── assets/
│   ├── environments/
│   └── styles/
├── angular.json
├── package.json
├── nswag/
│   └── service.config.nswag
└── ngsw-config.json
```

### Worker Template

```text
Templates/Worker/
├── Eaf.ProjectName.Worker.sln
├── src/
│   └── ...
```

## How to Update a Template

1. Read the relevant `.specs/eaf-*.spec.md` (e.g., `eaf-template-migration-and-update.spec.md`).
2. Identify whether the change is a shared-library update (`common.props`), a middleware integration, or a UI modernization.
3. For backend changes, update `common.props`, `Program.cs`, module dependencies, and `appsettings.json` examples.
4. For frontend changes:
   - Run `nvm use 20` then `npm install --legacy-peer-deps`.
   - Update `package.json` and `angular.json` as needed.
   - Regenerate `service-proxies.ts` with `nswag` only if the API changed.
   - Do not hand-edit `service-proxies.ts`.
5. Add or update `Dockerfile` and `docker-compose.yml` if required.
6. Run the build commands in `CLAUDE.md` before committing.
7. Update `Templates/*/README.md` and `.specs/eaf-specs-index-and-roadmap-2026.md`.

## Angular Template Conventions

### Components

- Prefer standalone components for new features.
- Use PrimeNG 17 components (`p-table`, `p-dialog`, `p-fileUpload`, `p-paginator`, `p-progressbar`, `p-button`).
- Remove `ngx-bootstrap` references when refactoring a page; do not do global removal unless explicitly asked.
- Use reactive forms with `FormBuilder`.
- Use `OnPush` change detection for read-only lists when feasible.

### Services

- API services are generated via `nswag` into `src/shared/service-proxies`.
- Add thin wrapper services under `src/app/shared/` only for UI-specific logic.
- Use `AppConsts` for global constants, not hard-coded strings.

### Routing

- Admin pages under `app/admin/:module`.
- Account pages under `app/account/:page`.
- Main dashboard under `app/main/dashboard`.
- Add route guards where required by permission.

### Styling

- Global variables in `src/styles.css` or `src/assets/`
- Metronic 5/6 legacy classes remain; new components should use PrimeNG design tokens or Bootstrap 5 utility classes.
- Dark mode tokens: follow `eaf-angular-dark-mode-theming.spec.md`.

## Backend Template Conventions

### common.props

All template projects should reference the central `common.props` at the repo root for:

- `TargetFramework` (`net10.0`)
- `LangVersion` (`14.0`)
- `Nullable` (`disabled`)
- Version and SourceLink settings
- Shared package versions

### Module Registration

```csharp
[DependsOn(
    typeof(AbpAspNetCoreModule),
    typeof(EafMiddlewareWebCoreModule),
    // add new EAF modules here
)]
public class ProjectNameWebModule : AbpModule
{
    public override void PreInitialize()
    {
        // Configure middleware options
    }
}
```

### nswag

- Configuration lives in `Templates/Angular/Eaf.ProjectName.UI/nswag/`.
- Regenerate after backend DTO/service changes:
  ```bash
  cd Templates/Angular/Eaf.ProjectName.UI
  npx nswag run nswag/service.config.nswag
  ```
- Do not edit `service-proxies.ts` manually.

## Modernization Roadmap Specs

The `.specs/` folder contains the template roadmap. Always consult it first:

- `eaf-template-migration-and-update.spec.md`
- `eaf-template-migration-9.4.1.md`
- `eaf-angular-metronic8-bootstrap5-migration.spec.md`
- `eaf-angular-modern-primeng-components.spec.md`
- `eaf-angular-dark-mode-theming.spec.md`
- `eaf-angular-pwa-offline.spec.md`
- `eaf-angular-mobile-responsive-layout.spec.md`
- `eaf-angular-customizable-dashboard.spec.md`
- `eaf-angular-audit-logs-ui.spec.md`

## Spec-Driven Template Updates

When a spec from `.specs/` requires a template change:

1. Read the spec.
2. Open the affected template files.
3. Implement only the changes requested in the spec.
4. Update the spec status in `.specs/eaf-specs-index-and-roadmap-2026.md`.
5. Add a note to `Templates/*/README.md` if behavior changed.

## Testing Template Changes

### Angular

```bash
cd Templates/Angular/Eaf.ProjectName.UI
nvm use 20
npm install --legacy-peer-deps
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

### API

```bash
dotnet build Templates/Api/Eaf.ProjectName.sln
dotnet test Templates/Api/test/*.sln  # if present
```

### Worker

```bash
dotnet build Templates/Worker/Eaf.ProjectName.Worker.sln
```

## Docker and CI/CD

- `Templates/Api/Dockerfile` and `Templates/Angular/Eaf.ProjectName.UI/Dockerfile` should mirror production builds.
- GitHub Actions in `.github/workflows` build templates; do not change workflow files without user approval.
- Keep `docker-compose.yml` examples for local dev.

## Best Practices

- Do not hand-edit generated files (`service-proxies.ts`, `*.g.cs`, `*.Designer.cs`).
- Keep `package-lock.json` committed after a verified install.
- Avoid adding heavy new frontend dependencies; prefer PrimeNG or built-in Angular APIs.
- Template scaffolds must remain neutral (no hard-coded customer names, secrets, or endpoints).
- When updating a template, verify the same change works in `Templates/Api`, `Templates/Angular`, and `Templates/Worker` if applicable.

## When in Doubt

- Follow ABP conventions for backend structure.
- Follow the existing EAF template style for the Angular UI.
- Refer to the relevant `.specs/*.md` before making changes.
- Test `ng build` and `dotnet build` before committing.
