# EAF — Eaf.HtmlSanitizer Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.HtmlSanitizer Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Security / Content Sanitization |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-html-sanitizer` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF does not provide an XSS-safe HTML sanitizer abstraction. Applications that render user-generated rich content currently rely on ad-hoc solutions or no sanitization at all, increasing XSS and injection risk.

### Objective

Create `Eaf.HtmlSanitizer` middleware module that provides `IHtmlSanitizer` with a default `HtmlSanitizer` implementation based on `AngleSharp` or `HtmlSanitizer`, configurable via `EafModule` options.

### Expected outcome

- New `src/Eaf.HtmlSanitizer/` project exposing `IHtmlSanitizer` and `HtmlSanitizer`.
- `EafHtmlSanitizerModule` registers the sanitizer as a singleton.
- Tests verify allowed tags and XSS payload neutralization.

### Out of scope

- WYSIWYG editor UI components.
- File upload / binary content scanning.
- Markdown sanitization.

## 2. Agent Role

Senior .NET/ABP engineer. Implement a small, focused security infrastructure module.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not modify existing `Eaf.Middleware` DTOs; do not push directly to remote.

## 4. Product Context

### Functional context

Sanitize user-generated HTML before persistence or rendering in Angular admin/tenant portals.

### Technical context

- ABP `EafModule` lifecycle.
- Castle Windsor DI.
- Common XSS vectors: `<script>`, `onerror`, `javascript:`, style injection.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- `HtmlSanitizer` package (v8.x or newer) or `AngleSharp`
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.HtmlSanitizer/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/Html/HtmlSanitizer.cs` (if any)
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.HtmlSanitizer` module and tests.

### Subtasks

1. Create `src/Eaf.HtmlSanitizer/` project.
2. Define `IHtmlSanitizer` with `Sanitize(string html, HtmlSanitizerOptions options = null)`.
3. Implement `DefaultHtmlSanitizer` using `HtmlSanitizer` package.
4. Create `HtmlSanitizerOptions` for allowed tags/attributes/schemas.
5. Create `EafHtmlSanitizerModule`.
6. Add `Eaf.HtmlSanitizer.Tests`.
7. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not change how existing DTOs handle HTML strings.
- Do not add UI code to this backend module.
- Do not introduce non-open-source or deprecated sanitization libraries.

## 6. Functional Requirements

### FR-001: HTML sanitization API

**Description:** The module must provide a clean `IHtmlSanitizer` interface that removes unsafe tags and attributes.

**Rules:**

- Allowed tags and attributes are configurable.
- Default policy removes `<script>`, `<style>`, event handlers, and `javascript:` URIs.
- Output is well-formed HTML or plain text fallback.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `html` | `string` | yes | Raw HTML or null |
| `options` | `HtmlSanitizerOptions` | no | Overrides default policy |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `sanitizedHtml` | `string` | Safe HTML string |

**Acceptance criteria:**

- [ ] `<script>alert(1)</script>` returns empty string or safe text.
- [ ] `onerror`, `onclick` attributes are stripped.
- [ ] `javascript://` URIs are stripped or replaced.
- [ ] Allowed tags like `<p>`, `<strong>` are preserved.

### FR-002: Configuration via module options

**Description:** Consumers configure allowed tags, attributes, and URI schemes through `EafHtmlSanitizerOptions`.

**Rules:**

- Options registered via `Configuration.Modules.EafHtmlSanitizer()`.
- Defaults documented and immutable after module initialization.

**Acceptance criteria:**

- [ ] `IHtmlSanitizer` honors configured allowed tags.
- [ ] Default options can be replaced per tenant if multi-tenancy is enabled.

## 7. Business Rules

### BR-001: Security by default

Default policy must be restrictive. Explicit opt-in is required to allow risky tags or attributes.

### BR-002: No mutations on null/empty input

`Sanitize(null)` returns `string.Empty`. `Sanitize(string.Empty)` returns `string.Empty`.

## 8. Domain Modeling

### Bounded Context

Security / Content Sanitization

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `DefaultHtmlSanitizer` | singleton | Sanitizes HTML using configured options |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `HtmlSanitizerOptions` | `AllowedTags`, `AllowedAttributes`, `AllowedUriSchemes` | Non-null collections; default to safe allow-lists |

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.HtmlSanitizer/
  Html/
    IHtmlSanitizer.cs
    DefaultHtmlSanitizer.cs
    HtmlSanitizerOptions.cs
  EafHtmlSanitizerModule.cs
  EafHtmlSanitizerConfiguration.cs
  README.md
  Eaf.HtmlSanitizer.csproj
test/Eaf.HtmlSanitizer.Tests/
  DefaultHtmlSanitizer_Tests.cs
  EafHtmlSanitizerModule_Tests.cs
```

### Allowed dependencies

- `Abp`
- `HtmlSanitizer` (or `AngleSharp` if license is clearer)
- `Microsoft.Extensions.Options`

### Forbidden dependencies

- UI frameworks.
- EF Core.

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafHtmlSanitizerOptions>(options =>
{
    options.AllowedTags = new HashSet<string> { "p", "strong", "em", "a" };
    options.AllowedAttributes = new HashSet<string> { "href" };
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public interface IHtmlSanitizer
{
    string Sanitize(string html, HtmlSanitizerOptions options = null);
}

public class HtmlSanitizerOptions
{
    public ISet<string> AllowedTags { get; set; } = new HashSet<string>();
    public ISet<string> AllowedAttributes { get; set; } = new HashSet<string>();
    public ISet<string> AllowedUriSchemes { get; set; } = new HashSet<string> { "https", "http", "mailto" };
}
```

## 12. Persistence and Data

N/A — no persistence.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `HtmlSanitizer` library | Parse and sanitize HTML | In-process | — | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Null input | `null` | Return `string.Empty` |
| Malformed HTML | `<p>unclosed` | Return balanced safe HTML |
| Style injection | `<p style="background-image:url(javascript:alert(1))">` | Strip style attribute or unsafe value |
| Unknown tags | `<custom-element>` | Remove tag or encode content based on options |

## 15. Few-Shot Examples

### Example 1: Happy path

**Input:**

```csharp
_sanitizer.Sanitize("<p>Hello <strong>world</strong><script>alert(1)</script></p>")
```

**Expected output:** `<p>Hello <strong>world</strong></p>`

### Example 2: XSS neutralized

**Input:**

```csharp
_sanitizer.Sanitize("<img src=x onerror=alert(1)>")
```

**Expected output:** `<img src="x">` or `<img src="x" />`

## 16. Non-Functional Requirements

### Performance

- Sanitization of 10 KB input < 5 ms P95.

### Security

- Default deny policy for tags and attributes.
- Do not log raw HTML content.

### Observability

- Structured logs via `ILogger` for unexpected parse errors only.

## 17. Mandatory Guardrails

- Do not allow `<script>` or event handlers by default.
- Do not add UI or frontend code.
- Do not publish packages automatically.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `DefaultHtmlSanitizer` | Allowed tags preserved, script tags removed, event handlers stripped, URI schemes enforced |
| `HtmlSanitizerOptions` | Defaults, merging with overrides |
| `EafHtmlSanitizerModule` | DI registration resolves `IHtmlSanitizer` |

### xUnit example

```csharp
public class DefaultHtmlSanitizer_Tests
{
    private readonly IHtmlSanitizer _sanitizer = new DefaultHtmlSanitizer();

    [Fact]
    public void Dado_HtmlComScript_Quando_Sanitizar_Entao_RemoveScript()
    {
        var result = _sanitizer.Sanitize("<p>ok</p><script>alert(1)</script>");
        result.ShouldBe("<p>ok</p>");
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.HtmlSanitizer` compiles and packs as NuGet.
- [ ] `IHtmlSanitizer` resolves from DI in a test host.
- [ ] XSS payloads are neutralized by default.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — verify no existing sanitizer in `src/`.
2. Design — choose `HtmlSanitizer` package version.
3. Project setup — create `src/Eaf.HtmlSanitizer/` and `test/Eaf.HtmlSanitizer.Tests/`.
4. Implementation — interface, options, default sanitizer, module.
5. Tests — unit tests for XSS vectors.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafHtmlSanitizerModule))]` and revert the branch.
- Replace usages with manual `HtmlSanitizer` package calls.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| `HtmlSanitizer` package API changes | Medium | Low | Pin version in `common.props` |
| Policy too restrictive breaks rich-text display | Medium | Medium | Document override examples |
| Unknown XSS bypass | High | Low | Use established library; add regression tests for CVEs |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> The module is a security primitive. Default policy must be deny-first. Do not expand scope to include UI components.
