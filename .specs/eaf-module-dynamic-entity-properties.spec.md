# EAF — Dynamic Entity Properties Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Dynamic Entity Properties Module |
| Product / System | EAF Middleware / Angular UI |
| Module / Bounded Context | Administration / Extensibility |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-dynamic-entity-properties` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF does not have the ABP Dynamic Parameter System, which allows administrators to add custom properties to entities at runtime without code changes. ASP.NET Zero documents this as "Dynamic Property System" and uses it for cities, counties, status codes, etc.

### Objective

Implement `Eaf.DynamicEntityProperties` (backend module + Angular UI) that lets host admins define dynamic properties, assign them to entities, and manage per-entity values through a generic manager component.

### Expected outcome

- `src/Eaf.DynamicEntityProperties/` backend module.
- `DynamicProperty`, `DynamicPropertyValue`, `EntityDynamicProperty`, `EntityDynamicPropertyValue` entities.
- `DynamicPropertyManager`, `DynamicEntityPropertyManager`, `DynamicEntityPropertyValueManager` domain services.
- Application services and DTOs for CRUD.
- Angular `dynamic-entity-property-manager` component and `admin/dynamic-property` page.

### Out of scope

- Custom input type rendering beyond standard HTML inputs (text, number, date, boolean, select).
- Business rule validation on dynamic values (values are stored as-is).

## 2. Agent Role

Senior full-stack .NET/ABP + Angular engineer. Follow ABP Dynamic Parameter System public docs and Zero docs.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not change existing entity schemas; do not remove existing admin pages; do not push NuGet packages.

## 4. Product Context

### Functional context

Host admins define extra fields (e.g., `Department`, `HireDate`) for `User`, `Tenant`, or any entity. Entity details pages show a "Dynamic Properties" action that opens a modal to edit values.

### Technical context

- ABP docs: `https://aspnetboilerplate.com/Pages/Documents/Dynamic-Parameter-System`
- Zero docs: `https://docs.aspnetzero.com/aspnet-core-angular/latest/Feature-Dynamic-Entity-Parameters-Angular`
- EAF uses PrimeNG 17 and reactive forms.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5 / EF Core 10
- Angular 20 / TypeScript 5.8 / PrimeNG 17
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Middleware.Core/
Templates/Angular/Eaf.ProjectName.UI/src/app/admin/
Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/
```

### Context files the agent must read before implementation

- `.specs/eaf-abp-feature-parity.spec.md`
- `src/Eaf.Middleware.Core/Authorization/Users/User.cs`

## 5. Task Definition

### Main task

Create `Eaf.DynamicEntityProperties` backend module and Angular UI.

### Subtasks

1. Create backend project and module.
2. Define entities: `DynamicProperty`, `DynamicPropertyValue`, `EntityDynamicProperty`, `EntityDynamicPropertyValue`.
3. Implement domain managers.
4. Add application services and permissions.
5. Add EF Core migration.
6. Add Angular manager component and admin page.
7. Add tests.
8. Add README.md.

### Do not do

- Do not modify existing entities to add dynamic columns; use a side table.
- Do not implement arbitrary code execution from dynamic properties.
- Do not break existing authorization permissions.

## 6. Functional Requirements

### FR-001: Dynamic property definition

**Description:** CRUD for dynamic properties with input types.

**Rules:**

- `DynamicProperty` has `PropertyName`, `InputType`, `Permission`.
- `InputType` is one of `Text`, `Number`, `Date`, `Boolean`, `Select`.
- `DynamicPropertyValue` stores allowed values for `Select` input type.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `PropertyName` | `string` | yes | Unique per entity type |
| `InputType` | `string` | yes | Supported type |
| `Permission` | `string` | no | Required permission to manage values |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `DynamicPropertyDto` | object | Created property |

**Acceptance criteria:**

- [ ] Admin can create/edit/delete dynamic properties.
- [ ] Duplicate property names per entity type are rejected.

### FR-002: Entity assignment

**Description:** Assign dynamic properties to entity types and individual entities.

**Rules:**

- `EntityDynamicProperty` links property to entity type (`User`, `Tenant`, etc.).
- `EntityDynamicPropertyValue` stores the actual value for an entity instance.
- Values are stored as strings and coerced by input type.

**Acceptance criteria:**

- [ ] Values can be set per entity.
- [ ] Reading values returns typed data (or string with metadata).

### FR-003: Angular manager component

**Description:** Reusable component to manage dynamic properties of any entity.

**Rules:**

- Component accepts entity type full name and entity id.
- Renders inputs based on `InputType`.
- Uses PrimeNG form controls.

**Acceptance criteria:**

- [ ] `dynamic-entity-property-manager` component works in entity detail pages.
- [ ] Admin page `admin/dynamic-property` manages property definitions.

### FR-004: Permissions

**Description:** Protect dynamic property management.

**Rules:**

- Permission names: `Pages_Administration_DynamicProperties`, `Pages_Administration_DynamicProperties_Values`.
- Optional per-property permission overrides.

**Acceptance criteria:**

- [ ] Unauthorized users cannot open the manager.
- [ ] Per-property permissions are enforced.

## 7. Business Rules

### BR-001: Entity type reference

Entity type is identified by full CLR type name (e.g., `MyCompany.AbpZeroTemplate.Authorization.Users.User`).

### BR-002: No schema changes

Dynamic properties are stored in side tables; no entity table is altered.

### BR-003: Tenant isolation

Dynamic property definitions can be global or tenant-specific.

## 8. Domain Modeling

### Bounded Context

Administration / Extensibility

### Aggregates

| Aggregate | Responsibility | Invariants |
|---|---|---|
| `DynamicProperty` | Property definition | Unique name per entity type |
| `EntityDynamicProperty` | Assignment to entity | Property assigned to entity type |

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `DynamicProperty` | `int` or `long` | Property definition |
| `DynamicPropertyValue` | `int` | Allowed values for Select type |
| `EntityDynamicProperty` | `int` | Link property to entity type |
| `EntityDynamicPropertyValue` | `int` | Actual value per entity |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `DynamicPropertyInputType` | `string` | One of supported types |

### Domain Events

- `DynamicPropertyValueChangedEvent`

## 9. Expected Architecture

```text
src/Eaf.DynamicEntityProperties/
  EafDynamicEntityPropertiesModule.cs
  Domain/
    DynamicProperty.cs
    DynamicPropertyValue.cs
    EntityDynamicProperty.cs
    EntityDynamicPropertyValue.cs
    Managers/
      DynamicPropertyManager.cs
      DynamicEntityPropertyManager.cs
      DynamicEntityPropertyValueManager.cs
  Application/
    DynamicPropertyAppService.cs
    IDynamicPropertyAppService.cs
    Dto/
      DynamicPropertyDto.cs
      CreateDynamicPropertyInput.cs
      EntityDynamicPropertyValueDto.cs
  ...
test/Eaf.DynamicEntityProperties.Tests/
```

## 10. API Contracts

### CRUD properties

```http
POST /api/services/app/DynamicProperty/Create
GET /api/services/app/DynamicProperty/GetAll
PUT /api/services/app/DynamicProperty/Update
DELETE /api/services/app/DynamicProperty/Delete
```

### Manage values

```http
POST /api/services/app/DynamicEntityPropertyValue/CreateOrUpdate
GET /api/services/app/DynamicEntityPropertyValue/GetAllValues?entityFullName=...&entityId=...
```

## 11. Application Contracts

```csharp
public interface IDynamicPropertyAppService : IApplicationService
{
    Task<DynamicPropertyDto> CreateAsync(CreateDynamicPropertyInput input);
    Task<List<DynamicPropertyDto>> GetAllAsync();
    Task DeleteAsync(int id);
}
```

## 12. Persistence and Data

### Persisted entities

| Table | Purpose |
|---|---|
| `EafDynamicProperties` | Property definitions |
| `EafDynamicPropertyValues` | Allowed values for Select |
| `EafEntityDynamicProperties` | Property-to-entity-type assignments |
| `EafEntityDynamicPropertyValues` | Per-entity values |

### Migration required

Yes.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_EafDynamicProperties_EntityFullName_PropertyName` | `EntityFullName`, `PropertyName` | Unique lookup |
| `IX_EafEntityDynamicPropertyValues_EntityId_PropertyId` | `EntityId`, `EntityDynamicPropertyId` | Fast value lookup |

## 13. Integrations

### External services

N/A.

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `IRepository<T>` | CRUD | EF Core | default | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Duplicate property name | same entity type + name | Validation error 400 |
| Unknown input type | `"Color"` | Reject with supported types list |
| Entity with no assigned properties | empty list | Manager shows empty form |
| Tenant-specific override | tenant defines same property | Tenant value wins if configured |

## 15. Few-Shot Examples

### Example 1: Define and use

```csharp
var prop = await _dynamicPropertyAppService.CreateAsync(new CreateDynamicPropertyInput
{
    PropertyName = "Department",
    InputType = "Text",
    EntityFullName = typeof(User).FullName
});
```

### Example 2: Angular usage

```html
<dynamic-entity-property-manager
  [entityFullName]="'MyCompany.AbpZeroTemplate.Authorization.Users.User'"
  [entityId]="user.id">
</dynamic-entity-property-manager>
```

## 16. Non-Functional Requirements

### Performance

- Property list loads < 200 ms.
- Value save < 100 ms.

### Security

- Values are stored as strings; do not execute as code.
- Sanitize rendered HTML if values are displayed.

### Observability

- Logs for property changes.

### Maintainability

- README.md with input type extension guide.

## 17. Mandatory Guardrails

- Do not modify existing entity tables.
- Do not execute dynamic values as code or SQL.
- Do not grant unauthorized access.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `DynamicPropertyManager` | CRUD, duplicate detection |
| `DynamicEntityPropertyValueManager` | Value coercion, tenant isolation |

### Integration tests

| Flow | Validation |
|---|---|
| End-to-end CRUD | Create property, assign, set value, read back |
| Permission enforcement | Unauthorized request rejected |

## 19. Acceptance Criteria

- [ ] Backend module compiles and packs.
- [ ] Angular manager component works in an entity page.
- [ ] Admin CRUD page present.
- [ ] Tests pass.

## 20. Implementation Plan

1. Create backend project and entities.
2. Implement domain managers and app services.
3. Add migration.
4. Build Angular component and admin page.
5. Add tests and README.
6. Update index.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafDynamicEntityPropertiesModule))]`.
- Revert migration if not deployed.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Performance on large value tables | Medium | Medium | Add indexes and paginate |
| Type coercion errors | Medium | Low | Unit tests for each input type |
| UI form generation complexity | Medium | Medium | Start with 5 standard input types |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Backend and Angular implemented.
- [ ] Tests pass.
- [ ] README and index updated.

## 24. Key Reminder

> Use side tables only. Never alter existing entity tables for dynamic properties. Start with a small set of input types.
