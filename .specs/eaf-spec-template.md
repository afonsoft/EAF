# EAF Spec Engineering Template

> Purpose: convert a product or engineering request into a clear, traceable, testable, and safe technical specification for AI-assisted execution in the EAF (Enterprise Application Foundation) repository.
> This SPEC is the source of truth for humans, agents, implementation, tests, code review, and delivery evidence.

---

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | `[feature name]` |
| Product / System | EAF |
| Module / Bounded Context | `[example: Payments, RedisCache, Angular.Dashboard]` |
| Change type | `[Feature | Refactor | Bugfix | Migration | API | Job | Frontend | Infrastructure]` |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `[example: feature/eaf-001-redis-cache]` |
| Technical owner | `[person or team]` |
| Status | `[Draft | In review | Approved | In implementation | Completed]` |
| Date | `[YYYY-MM-DD]` |
| Target agent | `[Claude Code | Devin | Copilot | OpenCode | Other]` |

---

## 1. Executive Summary

### Problem

`[Explain the current problem, limitation, risk, impact, or opportunity in EAF.]`

### Objective

`[Explain the expected business and technical outcome for EAF middleware or templates.]`

### Expected outcome

`[Explain how we will know the delivery is correct. Include evidence from the real codebase.]`

### Out of scope

- `[Item explicitly out of scope]`
- `[Item explicitly out of scope]`

---

## 2. Agent Role

The agent must act as a senior software engineer specialized in C#/.NET, ASP.NET Boilerplate (ABP), Castle Windsor, Entity Framework Core, Angular, automated testing, security, observability, and clean code.

Responsibility is to implement the solution according to this SPEC without inventing requirements, expanding the scope, or making undocumented architectural decisions.

### Expected behavior

- Be conservative with architectural changes.
- Preserve backward compatibility whenever possible.
- Prioritize readability, testability, and maintainability.
- Make uncertainty explicit before implementing.
- Do not introduce external dependencies without justification.
- Do not remove existing tests without a documented technical reason.
- Do not change public contracts without declaring the impact.
- Follow ABP N-Layer architecture: Domain → Application → Infrastructure → Web/Presentation.
- Use BDD test naming in Portuguese (`Dado_Quando_Entao`) where applicable.

---

## 3. Agent Autonomy Level

Select one level:

| Level | Name | Recommended use |
|---|---|---|
| 0 | Experimental | Sandbox or proof of concept with no production impact |
| 1 | Supervised | Agent implements with mandatory human review |
| 2 | Reliable | Agent implements well-bounded tasks with tests and validation |
| 3 | Resilient | Agent may propose refactors, rollback strategy, and deeper observability |
| 4 | Mission-critical | Requires auditability, formal validation, gates, and complete evidence |

### Selected level

`[0 | 1 | 2 | 3 | 4]`

### Restrictions associated with this level

- `[Example: do not push directly to the remote branch]`
- `[Example: do not change database schemas without migration and rollback]`
- `[Example: do not publish NuGet packages or deploy automatically]`

---

## 4. Product Context

### Functional context

`[Explain where this feature fits in the EAF middleware, Api template, or Angular template user journey.]`

### Technical context

`[Explain the current architecture, layers, integrations, frameworks, and constraints. Mention whether the change affects EAF middleware packages, generated templates, or both.]`

### Relevant stack

- Language: `C# 14` / `.NET 10`
- Frameworks: `ASP.NET Boilerplate 10.5`, `ASP.NET Core`, `EF Core 10`, `Castle Windsor`
- Frontend: `Angular 20`, `TypeScript 5.8`, `PrimeNG 17`, `ngx-bootstrap 12`
- Tests: `xUnit`, `Shouldly`, `NSubstitute`, `coverlet`
- Database: `SQL Server` / `SQLite` / `PostgreSQL`
- Background jobs: `Hangfire`
- Observability: `OpenTelemetry`, `Serilog`
- CI/CD: `GitHub Actions`, `SonarCloud`

### Relevant files or directories

```text
/src
/test
/Templates
/docs
/.github
/.specs
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-specs-index-and-roadmap-2026.md`
- `[example: docs/architecture.md]`
- `[example: docs/domain/[bounded-context].md]`
- `[example: README.md]`

---

## 5. Task Definition

### Main task

`[Example: Create Eaf.RedisCache middleware module implementing ICacheManager distributed cache provider with Castle Windsor DI and a RedisConnectionFactory.]`

### Subtasks

- `[Subtask 1]`
- `[Subtask 2]`
- `[Subtask 3]`

### Do not do

- `[Example: do not create frontend screens if the SPEC is backend-only]`
- `[Example: do not change the legacy public contract without a migration note]`
- `[Example: do not add a new table without approval]`

---

## 6. Functional Requirements

### FR-001: `[Requirement name]`

**Description:**
`[The system must...]`

**Rules:**

- `[Rule 1]`
- `[Rule 2]`

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---:|---|
| `[field]` | `[type]` | `[yes/no]` | `[rule]` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `[field]` | `[type]` | `[description]` |

**Acceptance criteria:**

- [ ] `[Testable criterion]`
- [ ] `[Testable criterion]`

---

## 7. Business Rules

### BR-001: `[Rule name]`

`[Describe the rule objectively.]`

### BR-002: `[Rule name]`

`[Describe the rule objectively.]`

### Domain invariants

- `[Example: A tenant cannot be deleted while it has active subscriptions.]`
- `[Example: A payment gateway must be registered in the resolver before use.]`

---

## 8. Domain Modeling

### Bounded Context

`[Bounded context name]`

### Aggregates

| Aggregate | Responsibility | Invariants |
|---|---|---|
| `[Name]` | `[Responsibility]` | `[Invariants]` |

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `[Name]` | `[Identity]` | `[Responsibility]` |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `[Name]` | `[Fields]` | `[Validations]` |

### Domain Events

| Event | When it occurs | Payload |
|---|---|---|
| `[Name]` | `[When]` | `[Payload]` |

### Expected C# style

```csharp
public sealed class Example : Entity
{
    public virtual int Id { get; protected set; }

    protected Example() { }

    public Example(int id)
    {
        Id = id;
    }
}
```

---

## 9. Expected Architecture

### Architectural style

`[Clean Architecture | Layered Architecture | ABP Modular Monolith | Microservice]`

### Layers

```text
Domain
Application
Infrastructure
Web.Core / API
Tests
```

### Allowed dependencies

- `Domain` does not depend on any other layer.
- `Application` depends on `Domain`.
- `Infrastructure` implements contracts defined by `Application` or `Domain`.
- `Web.Core` / `API` orchestrates input and output.
- `Templates` consume EAF NuGet packages or source-project references.
- Tests validate domain, application, and integration behavior.

### Forbidden dependencies

- Domain accessing database, HTTP, queues, cache, or framework-specific APIs.
- Controller containing business rules.
- Handler containing complex domain rules.
- Repository validating business rules.
- DTOs leaking into the domain model.

---

## 10. API Contracts

### Endpoint

```http
[POST] /api/services/app/[Service]/[Action]
```

### Authentication and authorization

`[Example: JWT required, permission Pages_Administration_Users.]`

### Request

```json
{
  "field": "value"
}
```

### Success response

```json
{
  "id": 1,
  "status": "created"
}
```

### Error responses

| Status | When | Expected body |
|---:|---|---|
| 400 | Invalid request | `UserFriendlyException` / `ProblemDetails` |
| 401 | Unauthenticated | `ProblemDetails` |
| 403 | Unauthorized | `ProblemDetails` |
| 404 | Resource not found | `ProblemDetails` |
| 409 | Business conflict | `ProblemDetails` |
| 500 | Unexpected error | `ProblemDetails` without sensitive details |

---

## 11. Application Contracts

### DTO / Input / Output

```csharp
public class CreateExampleInput
{
    public string Name { get; set; }
}

public class CreateExampleOutput
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Expected AppService style

```csharp
public class ExampleAppService : ApplicationService, IExampleAppService
{
    private readonly IRepository<Example, int> _exampleRepository;

    public ExampleAppService(IRepository<Example, int> exampleRepository)
    {
        _exampleRepository = exampleRepository;
    }

    public async Task<CreateExampleOutput> CreateAsync(CreateExampleInput input)
    {
        // implementation
    }
}
```

---

## 12. Persistence and Data

### Persisted entities

| Table / Collection | Purpose |
|---|---|
| `[Table]` | `[Purpose]` |

### Migration required

`[Yes/No]`

### Migration strategy

- UP:
  - `[Operation 1]`
  - `[Operation 2]`
- DOWN:
  - `[Rollback operation 1]`
  - `[Rollback operation 2]`

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `[Index]` | `[Fields]` | `[Reason]` |

### Compatibility

- [ ] Does not break existing data.
- [ ] Includes rollback.
- [ ] Includes migration test, if applicable.
- [ ] Does not expose sensitive data.

---

## 13. Integrations

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---:|---|
| `[service]` | `[usage]` | `[REST/SignalR/Queue]` | `[ms]` | `[yes/no]` |

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| `[service]` | `[payload]` | `[response]` | `[auth, encryption]` |

### Expected failures

- `[Timeout]`
- `[Service unavailable]`
- `[Invalid response]`

### Resilience strategy

- Explicit timeout.
- Retry only for transient failures.
- Circuit breaker when applicable.
- Fallback only if defined in this SPEC.
- Logs without sensitive data.

---

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| `[Null input]` | `[null]` | `[Return 400 or raise validation error]` |
| `[Empty list]` | `[]` | `[Return domain validation error]` |
| `[Resource not found]` | `[invalid id]` | `[Return 404]` |
| `[Business conflict]` | `[invalid state]` | `[Return 409]` |

---

## 15. Few-Shot Examples

### Example 1: Happy path

**Input:**

```json
{
  "name": "Example"
}
```

**Expected output:**

```json
{
  "id": 1,
  "name": "Example"
}
```

### Example 2: Validation error

**Input:**

```json
{
  "name": ""
}
```

**Expected output:**

```json
{
  "title": "Validation error",
  "status": 400,
  "errors": {
    "name": [
      "Name is required."
    ]
  }
}
```

---

## 16. Non-Functional Requirements

### Performance

- `[Example: API endpoint must respond within 200ms P95 under nominal load.]`
- `[Example: Angular initial bundle must remain below 1 MB gzipped.]`

### Security

- `[Example: do not log sensitive data.]`
- `[Example: validate authorization before accessing a resource.]`
- `[Example: sanitize HTML before storing or rendering.]`

### Observability

- Structured logs with `ILogger` / `Serilog`.
- Metrics for success, errors, and latency.
- Tracing for external calls.
- CorrelationId across the request flow.

### Reliability

- Respect cancellation tokens.
- Implement timeouts for external calls.
- Avoid partially persisted operations.
- Guarantee idempotency when applicable.

### Maintainability

- Low coupling.
- High cohesion.
- Clear tests.
- Explicit names.
- Small classes.
- No domain logic in controllers.

---

## 17. Mandatory Guardrails

The agent must follow these rules:

- Do not invent requirements.
- Do not create a new architecture without justification.
- Do not modify a public contract without documenting the breaking change.
- Do not remove or ignore existing tests.
- Do not add a library without an explicit need.
- Do not place business rules in controllers.
- Do not access infrastructure directly from the domain layer.
- Do not expose secrets, tokens, personal data, or regulated data in logs.
- Do not deploy, push, or merge automatically.
- Do not modify CI/CD pipelines unless this SPEC has a dedicated section for it.
- Do not expand the scope with opportunistic improvements.
- Stop and request human review when there is critical ambiguity.
- Do not edit generated files (`service-proxies.ts`, `*.Designer.cs`, `*.g.cs`).
- Do not reduce test coverage.

---

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `[Class]` | `[Scenario 1, Scenario 2]` |
| `[Class]` | `[Scenario 1, Scenario 2]` |

### Integration tests

| Flow | Validation |
|---|---|
| `[API/Flow]` | `[Expected result]` |
| `[API/Flow]` | `[Expected result]` |

### Contract tests

- [ ] Valid request.
- [ ] Expected response.
- [ ] Standardized errors.
- [ ] Compatibility with consumers.

### xUnit example

```csharp
public sealed class ExampleTests
{
    [Fact]
    public void Constructor_Should_Set_Name()
    {
        var example = new Example(1, "Test");
        Assert.Equal("Test", example.Name);
    }
}
```

---

## 19. Acceptance Criteria

The implementation is considered complete only when:

- [ ] All functional requirements are implemented.
- [ ] All business rules are covered by tests.
- [ ] All listed edge cases are handled.
- [ ] The defined architecture is respected.
- [ ] There is no business logic in controllers.
- [ ] There is no infrastructure dependency in the domain layer.
- [ ] All existing tests continue to pass.
- [ ] New tests are added.
- [ ] Logs do not expose sensitive data.
- [ ] Errors follow the defined standard.
- [ ] Build, lint, and tests pass locally.
- [ ] The implementation is traceable to this SPEC.

---

## 20. Implementation Plan

### Step 1: Discovery

- [ ] Read context files.
- [ ] Identify the current architecture.
- [ ] Identify existing patterns.
- [ ] Locate related tests.
- [ ] Confirm existing dependencies and contracts.

### Step 2: Technical design

- [ ] Define domain classes.
- [ ] Define DTOs and application services.
- [ ] Define API contracts.
- [ ] Define repositories and interfaces.
- [ ] Define migrations, if applicable.

### Step 3: Implementation

- [ ] Implement domain.
- [ ] Implement application layer.
- [ ] Implement infrastructure.
- [ ] Implement API / UI.
- [ ] Implement validations.
- [ ] Implement observability.

### Step 4: Tests

- [ ] Domain tests.
- [ ] Application tests.
- [ ] Integration tests.
- [ ] Contract tests.
- [ ] Regression tests.

### Step 5: Final validation

- [ ] Run build.
- [ ] Run tests.
- [ ] Review architecture.
- [ ] Review logs.
- [ ] Review security.
- [ ] Update documentation.

---

## 21. Rollback Strategy

### When to trigger rollback

- `[Example: increased 5xx errors]`
- `[Example: contract break]`
- `[Example: data inconsistency]`

### How to revert

- `[Example: disable feature flag]`
- `[Example: rollback migration]`
- `[Example: rollback deployment]`

### Expected evidence

- Logs.
- Metrics.
- Tests.
- Smoke test result.

---

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| `[Risk]` | `[High/Medium/Low]` | `[High/Medium/Low]` | `[Mitigation]` |

---

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Implementation follows the SPEC.
- [ ] Automated tests created.
- [ ] Build validated.
- [ ] Contracts preserved or versioned.
- [ ] Observability implemented.
- [ ] Documentation updated.
- [ ] PR describes changes, risks, and evidence.
- [ ] No critical TODO left in the code.
- [ ] No implicit architectural decision.

---

## 24. Key Reminder

> The SPEC is the contract.
> The agent must not optimize, expand, or reinterpret the scope.
> In case of ambiguity, the agent must stop, make the uncertainty explicit, and propose technical options with impact, risk, and recommendation.

---

# Standard Prompt to Generate an EAF SPEC from a Request

Use the prompt below with an AI coding agent when you want it to produce a SPEC before implementation.

```markdown
You must act as a SPEC Engineering agent for EAF (Enterprise Application Foundation).

Your task is to convert the request below into a complete technical SPEC using the standard EAF SPEC Engineering template in `.specs/eaf-spec-template.md`.

Mandatory rules:
- Do not invent requirements.
- When information is missing, mark it as `[TO BE DEFINED]`.
- Clearly separate context, requirements, business rules, architecture, contracts, tests, risks, and acceptance criteria.
- Use technical, objective, and traceable language.
- Adapt the output to C# 14, .NET 10, ASP.NET Boilerplate 10.5, Castle Windsor, EF Core 10, Angular 20, TypeScript 5.8, PrimeNG 17, xUnit/Shouldly/NSubstitute, and observability best practices.
- Do not generate production code yet.
- First generate only the SPEC.
- Implementation must start only after human approval.

Input request:

```text
[PASTE THE REQUEST HERE]
```

Additional project context:

```text
[PASTE CONTEXT, LINKS, FILES, OR CONSTRAINTS HERE]
```

Expected output:
- Generate a `SPEC.md` file under `.specs/`.
- At the end, include a `Pending Questions and Ambiguities` section.
- Include a human approval checklist.
```

---

# Recommended Repository Structure for a SPEC

```text
.specs/
  SPEC.md                          # this specification
  eaf-specs-index-and-roadmap-2026.md
src/
  Eaf.[Module]/                    # middleware module (if applicable)
  Eaf.[Module].Tests/
Templates/
  Angular/Eaf.ProjectName.UI/      # frontend changes (if applicable)
  Api/                             # generated project template (if applicable)
docs/
  [relevant documentation]
```
