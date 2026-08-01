---
name: angular-development
description: Expert guidance for TypeScript and Angular development for the EAF (Enterprise Application Foundation) project. Covers Angular 18, standalone components, signals, reactive forms, PrimeNG integration, jQuery legacy integration, SignalR real-time features, accessibility (WCAG AA), and EAF-specific framework integration. Use this skill when developing Angular components, integrating with EAF backend, or troubleshooting frontend issues. Do NOT use for backend API development, non-Angular frontend frameworks, or general TypeScript projects.
metadata:
  version: '1.0.0'
---

# Angular Development Skill

You are an expert in TypeScript, Angular, and scalable web application development for the EAF (Enterprise Application Foundation) project. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

## Project Context

The EAF project is an enterprise application framework based on ASP.NET Boilerplate (ABP) with Angular frontend. The project structure includes:
- **Backend**: .NET 10.0 with ASP.NET Core
- **Frontend**: Angular 20 in `Templates/Angular/Eaf.ProjectName.UI` (package.json shows ^20.x)
- **UI Libraries**: PrimeNG 17, ngx-bootstrap 12, Metronic theme bundles (legacy Metronic 5/6 style, migration to Metronic 8 + Bootstrap 5 planned), EAF.js framework
- **Integration**: SignalR for real-time, jQuery for legacy components, Service Worker / PWA packages present but not fully configured
- **Responsiveness**: Minimal custom media queries; mobile improvements planned (see `.specs/eaf-angular-mobile-responsive-layout.spec.md`)

## TypeScript Best Practices

- Use strict type checking in `tsconfig.json`
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain
- Use `readonly` for properties that shouldn't change
- Prefer `const` assertions for literal types
- Use type guards for runtime type checking

## Angular Best Practices

### Component Architecture
- Always use standalone components over NgModules (Angular 18+)
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- Prefer Reactive forms instead of Template-driven ones

### Template Guidelines
- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file

### Decorator Guidelines
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Group Angular-specific properties before methods
- Keep lifecycle methods simple
- Use lifecycle hook interfaces

### State Management
- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead
- Implement lazy loading for feature routes

### Service Guidelines
- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection
- Use `NgOptimizedImage` for all static images
- `NgOptimizedImage` does not work for inline base64 images

### Accessibility Requirements
- It MUST pass all AXE checks
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes
- Use semantic HTML elements
- Ensure keyboard navigation works
- Provide alternative text for images
- Use ARIA labels for interactive elements

## EAF-Specific Guidelines

### EAF Framework Integration
- The EAF project uses custom module loading with `@eaf/*` path mappings
- EAF.js framework integration is in `src/assets/lib/eaf-web-resources/`
- Test all EAF module imports and path mappings after Angular upgrades
- EAF initialization sequence must be tested after Angular 19 bootstrap changes

### jQuery Integration
- Heavy jQuery usage alongside Angular exists in the codebase
- Isolate jQuery usage to avoid conflicts with Angular change detection
- Test Angular change detection with jQuery DOM manipulation
- Consider gradual migration from jQuery to pure Angular

### SignalR Integration
- @microsoft/signalr version is ^7.0.14
- Test all SignalR connections and real-time updates after Angular upgrades
- Real-time features may break with Angular 19's change detection

### PrimeNG Components
- PrimeNG 17.17.0 is currently used in `Templates/Angular/Eaf.ProjectName.UI`
- Test all PrimeNG components visually and functionally after upgrades
- Consider standardizing on PrimeNG instead of `ngx-bootstrap` and legacy jQuery widgets
- Prefer PrimeNG theming v17+ (styled/unstyled modes, design tokens) for future migrations

### Responsiveness and Mobile
- The current layout is desktop-first with Metronic legacy bundles
- Mobile improvements are planned; use Bootstrap 5 grid and CSS custom properties for new components
- Add `@media` breakpoints for critical components (chat, sidebar, tables, forms)
- Ensure touch targets >= 44x44px and avoid hover-only interactions on touch
- See `.specs/eaf-angular-mobile-responsive-layout.spec.md` for the roadmap

### PWA / Offline
- `@angular/pwa` and `@angular/service-worker` are already in `package.json`
- Configure `ngsw-config.json`, `manifest.webmanifest` and register the service worker
- Use `localforage` (already present) for offline data queue and cache
- See `.specs/eaf-angular-pwa-offline.spec.md`

## File Structure Conventions

### Naming Conventions
- Separate words in file names with hyphens
- Use the same name for a file's tests with .spec at the end
- Match file names to the TypeScript identifier within
- Use the same file name for a component's TypeScript, template, and styles

### Project Structure
- All the application's code goes in a directory named `src`
- Bootstrap your application in a file named `main.ts` directly inside src
- Group closely related files together in the same directory
- Organize your project by feature areas
- One concept per file

## Migration Guidelines

### Angular 18/19/20 Migration
- Follow the automated migration guide in `Templates/Angular/Eaf.ProjectName.UI/docs/MIGRATION_ANGULAR_17_TO_19.md` if available
- Use Angular CLI schematics for automatic migrations where possible
- Migrate control flow syntax: `*ngIf` → `@if`, `*ngFor` → `@for`, `*ngSwitch` → `@switch`
- Gradually migrate to standalone components
- The project is already on Angular 20 packages; verify runtime and build compatibility
- Test EAF framework integration after each migration step

### Metronic 8 + Bootstrap 5 Migration
- This is a long-term UI modernization (see `.specs/eaf-angular-metronic8-bootstrap5-migration.spec.md`)
- Avoid adding new code using legacy Metronic classes (`m-stack`, `m-grid__item`)
- Prefer Bootstrap 5 utilities (`d-flex`, `flex-*`, `gap-*`, `offcanvas`) and CSS Grid
- Migrate `ngx-bootstrap` usage to PrimeNG native components before removing `ngx-bootstrap`
- Replace Font Awesome 5 / Line Awesome / Flaticon mix with a single icon library (Font Awesome 6 or Bootstrap Icons)

### Material Design Implementation
- Consider PrimeNG 17+ as the primary component library instead of migrating to Angular Material
- If Angular Material is introduced, configure theme in `src/styles.scss`
- Maintain existing EAF theming system (`currentTheme.baseSettings.header.headerSkin`, `menu.asideSkin`)

## Testing Guidelines

### Test Coverage Target
- Aim for 90%+ code coverage
- Generate tests for all 37 components in the UI template
- Test component rendering, user interactions, and data flow
- Mock services and dependencies appropriately
- Test integration with EAF framework

### Test Structure
- Use Jasmine/Karma for unit testing
- Use TestBed for component testing
- Use RouterTestingModule for routing tests
- Use BrowserAnimationsModule for animations
- Mock services with jasmine.createSpyObj

## Performance Optimization

- Use `NgOptimizedImage` for all static images
- Implement lazy loading for feature routes
- Use OnPush change detection strategy
- Consider zoneless mode (Angular 19) for performance
- Optimize bundle size with tree-shaking
- Use deferred loading (@defer) for heavy components

## Security Best Practices

- Sanitize all user inputs
- Use Angular's built-in sanitization for HTML
- Implement proper authentication and authorization
- Use HTTPS for all API calls
- Validate data on both client and server
- Keep dependencies updated

## Common Patterns to Avoid

- Do NOT use jQuery for DOM manipulation in new code
- Do NOT use `any` type
- Do NOT use `ngClass` or `ngStyle` directives
- Do NOT use `@HostBinding` or `@HostListener` decorators
- Do NOT use `mutate` on signals
- Do NOT assume global variables are available in templates

## When in Doubt

- Prefer consistency with existing code patterns
- Follow Angular official documentation
- Test changes thoroughly before committing
- Consider backward compatibility
- Consult the EAF team for architectural decisions
