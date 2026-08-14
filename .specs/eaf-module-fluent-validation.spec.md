# EAF — Eaf.FluentValidation Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.FluentValidation Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Validation |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-fluent-validation` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

ABP uses DataAnnotations for DTO validation. EAF lacks a `FluentValidation` integration that would allow complex, reusable, and testable validation rules for application service inputs.

### Objective

Create `Eaf.FluentValidation` middleware module that integrates `FluentValidation` validators into the ABP validation pipeline via `IValidationInterceptor` or a custom `IMethodParameterValidator`.

### Expected outcome

- New `src/Eaf.FluentValidation/` project.
- `EafFluentValidationModule` registers all `IValidator<T>` implementations.
- ABP validation pipeline uses FluentValidation rules alongside DataAnnotations.
- Tests for validator registration and rule execution.

### Out of scope

- Replacing DataAnnotations entirely.
- Client-side validation generation.
- Localization of validation messages beyond ABP `ILocalizationSource`.

## 2. Agent Role

Senior .NET/ABP engineer. Add FluentValidation as an opt-in validation provider without removing existing DataAnnotations support.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not remove `AbpValidation` module; do not change ABP validation contracts.

## 4. Product Context

### Functional context

Application service DTOs can use `IValidator<T>` classes for complex cross-property and conditional rules.

### Technical context

- ABP `ValidationInterceptor` / `IMethodParameterValidator`.
- Castle Windsor registration by convention (`IValidator<T>` → concrete validators).
- `FluentValidation` 11.x.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- FluentValidation 11.x
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.FluentValidation/
src/Eaf.Middleware.Application/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Application/Validation/` (if any)
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.FluentValidation` module and tests.

### Subtasks

1. Create `src/Eaf.FluentValidation/` project.
2. Define `IFluentValidator<T>` wrapper or adapter.
3. Implement `EafFluentValidationMethodParameterValidator`.
4. Create `EafFluentValidationModule` and register validator types by convention.
5. Add `Eaf.FluentValidation.Tests`.
6. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not remove DataAnnotations support.
- Do not change ABP `ValidationInterceptor` source.
- Do not add UI code to this backend module.

## 6. Functional Requirements

### FR-001: FluentValidation registration by convention

**Description:** The module must scan assemblies and register all `IValidator<T>` implementations with Castle Windsor.

**Rules:**

- Auto-register public validators ending with `Validator`.
- Use `Scoped` lifetime matching application services.
- Skip abstract classes and interfaces.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `assembly` | `Assembly` | yes | Assembly to scan |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `registration` | `IWindsorInstaller` | Windsor registration object |

**Acceptance criteria:**

- [ ] `IValidator<CreateUserInput>` resolves from DI.
- [ ] Validators in `Application` layer are auto-registered.

### FR-002: ABP validation pipeline integration

**Description:** FluentValidation results must be converted to ABP `ValidationErrors` and participate in existing `AbpValidationException`.

**Rules:**

- Validator runs before or alongside ABP DataAnnotations validation.
- Combine errors from both sources.
- Property names map to DTO property names.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `input` | `object` | yes | DTO or input object |
| `validator` | `IValidator<T>` | yes | Registered validator for `T` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `validationResult` | `ValidationResult` | FluentValidation result |

**Acceptance criteria:**

- [ ] ABP throws `AbpValidationException` with FluentValidation errors.
- [ ] Errors include localized messages via ABP localization.
- [ ] Invalid input fails before reaching application service logic.

## 7. Business Rules

### BR-001: Opt-in module

`EafFluentValidationModule` is not required by default. Generated templates can add it if they want FluentValidation.

### BR-002: DataAnnotations coexistence

Both validation systems may run. FluentValidation errors are additive to DataAnnotations errors.

## 8. Domain Modeling

### Bounded Context

Validation

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `EafFluentValidationMethodParameterValidator` | validator | Adapts FluentValidation to ABP pipeline |
| `EafFluentValidationValidatorFactory` | factory | Resolves `IValidator<T>` from Windsor |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `FluentValidationOptions` | `ValidatorAssemblies` | List of assemblies to scan |

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.FluentValidation/
  FluentValidation/
    EafFluentValidationModule.cs
    EafFluentValidationMethodParameterValidator.cs
    EafFluentValidationValidatorFactory.cs
    FluentValidationOptions.cs
  README.md
  Eaf.FluentValidation.csproj
test/Eaf.FluentValidation.Tests/
  EafFluentValidation_Tests.cs
  SampleValidators/
```

### Allowed dependencies

- `Abp`
- `FluentValidation`
- `Castle.Windsor`

### Forbidden dependencies

- UI frameworks.
- EF Core.

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafFluentValidationOptions>(options =>
{
    options.ValidatorAssemblies.Add(typeof(MyApplicationModule).GetAssembly());
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public class EafFluentValidationOptions
{
    public List<Assembly> ValidatorAssemblies { get; set; } = new List<Assembly>();
}

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8);
    }
}
```

## 12. Persistence and Data

N/A.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| ABP `ValidationInterceptor` | Trigger validation | In-process | — | no |
| `IValidator<T>` | Execute FluentValidation rules | In-process | — | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| No validator for type | input without matching `IValidator<T>` | Skip FluentValidation; run DataAnnotations only |
| Null input | `null` | Let ABP handle null validation as before |
| Validator throws exception | invalid rule | Wrap in `AbpValidationException` with inner exception |
| Both DataAnnotations and FluentValidation fail | invalid input | Combine errors in one `AbpValidationException` |

## 15. Few-Shot Examples

### Example 1: Happy path

```csharp
public class MyAppService : ApplicationService
{
    public async Task CreateAsync(CreateUserInput input)
    {
        // FluentValidation runs before this method body via ABP pipeline.
    }
}
```

### Example 2: Validation error

```csharp
var input = new CreateUserInput { Email = "not-an-email" };
// AbpValidationException thrown with message "Email is not valid."
```

## 16. Non-Functional Requirements

### Performance

- Validator resolution and execution overhead < 5 ms per request.

### Security

- Do not return raw exception messages to clients.
- Do not log sensitive DTO fields.

### Observability

- Structured logs via `ILogger` for validation pipeline failures.

## 17. Mandatory Guardrails

- Do not remove DataAnnotations.
- Do not modify ABP validation contracts.
- Do not add UI code.
- Stop and ask if `FluentValidation` version or licensing is ambiguous.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafFluentValidationValidatorFactory` | Resolves validators, returns null for unregistered types |
| `EafFluentValidationMethodParameterValidator` | Runs validator, maps errors to ABP format |

### Integration tests

| Flow | Validation |
|---|---|
| AppService with validator | Invalid input throws `AbpValidationException` |
| Combined DataAnnotations + FluentValidation | Both error sets present in exception |

### xUnit example

```csharp
public class EafFluentValidation_Tests : AbpIntegratedTestBase<EafFluentValidationModule>
{
    [Fact]
    public void Dado_InputInvalido_Quando_Validar_Entao_DisparaAbpValidationException()
    {
        var validator = Resolve<IValidator<CreateUserInput>>();
        var input = new CreateUserInput { Email = "invalid" };
        Assert.Throws<AbpValidationException>(() => validator.Validate(input));
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.FluentValidation` compiles and packs as NuGet.
- [ ] `IValidator<T>` resolves from DI.
- [ ] `AbpValidationException` contains FluentValidation errors.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — inspect ABP validation pipeline in `src/`.
2. Design — choose integration point (`IMethodParameterValidator` or interceptor).
3. Project setup — create `src/Eaf.FluentValidation/` and `test/Eaf.FluentValidation.Tests/`.
4. Implementation — validator factory, parameter validator, module, options.
5. Tests — unit and integration tests.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafFluentValidationModule))]`.
- Remove `IValidator<T>` classes or convert to DataAnnotations.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Double validation errors (overlap with DataAnnotations) | Medium | Medium | Combine errors deduplicated by property name |
| ABP validation pipeline changes | Medium | Low | Pin ABP version; target public extensibility points |
| Performance overhead in high-throughput APIs | Medium | Low | Cache validator instances in DI scope |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> FluentValidation must coexist with DataAnnotations, not replace it. Keep ABP validation contracts intact.
