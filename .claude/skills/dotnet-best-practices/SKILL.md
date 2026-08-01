---
name: dotnet-best-practices
description: >-
  Ensure .NET/C# code meets best practices for the EAF solution. Covers
  documentation, architecture, DI with Castle Windsor, async patterns,
  testing with xUnit/Shouldly/NSubstitute, configuration, error handling,
  and SOLID principles. Use when reviewing code quality, implementing new
  features, or ensuring standards compliance.
metadata:
  version: '1.0.0'
---

# .NET/C# Best Practices for EAF

Ensure .NET/C# code meets the best practices for the EAF (Enterprise Application Foundation) solution.

## Documentation & Structure

- Create comprehensive XML documentation comments for all public classes, interfaces, methods, and properties
- Include parameter descriptions and return value descriptions in XML comments
- Use Portuguese language for documentation summaries
- Follow the established namespace structure matching module hierarchy

## Design Patterns & Architecture

- Follow ABP layered architecture (Domain, Application, Infrastructure, Presentation)
- Use Castle Windsor for dependency injection with `ISingletonDependency`, `ITransientDependency`
- Implement Repository pattern for data access via `IRepository<TEntity>`
- Use Application Service pattern (`IApplicationService`) for business logic
- Apply Domain Services (`IDomainService`) for cross-entity logic
- Follow DDD: Aggregates, Entities, Value Objects, Domain Events

## Dependency Injection

- Register via interface inheritance (`ISingletonDependency`, `ITransientDependency`)
- Use constructor injection (Castle Windsor resolves automatically)
- Avoid service locator pattern
- Use `IIocResolver` only for dynamic resolution scenarios

## Async/Await Patterns

- Use async/await for all I/O operations and long-running tasks
- Return `Task` or `Task<T>` from async methods
- Avoid `async void` except for event handlers
- Handle async exceptions properly with try/catch
- Do NOT use `.Result` or `.Wait()` on tasks

## Testing Standards

- Use **xUnit 2.9.3** for test framework
- Use **Shouldly** for fluent assertions (`value.ShouldBe(expected)`)
- Use **NSubstitute** for mocking (`Substitute.For<IInterface>()`)
- Follow BDD pattern in Portuguese: Dado/Quando/Entao (Given/When/Then)
- Test naming: `Dado_Condicao_Quando_Acao_Entao_Resultado()`
- Include null parameter validation tests
- Test both success and failure scenarios

## Configuration & Settings

- Use `IConfiguration` via `IAppConfigurationAccessor`
- Support `appsettings.json` and environment-specific overrides
- Use ABP Settings system for dynamic tenant/user configuration
- Use EAF KeyVault module for secrets management

## Error Handling & Logging

- Use `Castle.Core.Logging.ILogger` for logging in ABP services
- Use structured logging with format parameters (NOT interpolation)
- Throw `UserFriendlyException` for user-facing errors
- Throw `AbpException` for framework-level errors
- Use `try-catch` only for expected failure scenarios
- Preserve stack traces: use `throw;` not `throw ex;`

## Performance & Security

- Use .NET 10.0 features and optimizations
- Implement proper input validation with Data Annotations
- Use parameterized queries (EF Core does this by default)
- Never expose secrets in logs or responses
- Follow CA2254: Use constant log message templates

## Code Quality

- Follow SOLID principles
- Avoid code duplication through base classes and utilities
- Use meaningful names reflecting domain concepts (Portuguese OK)
- Keep methods focused and cohesive (max ~30 lines)
- Implement `IDisposable` for unmanaged resources
- Prefer `IEnumerable<T>` over `List<T>` in interfaces
- Use `readonly` for fields that don't change after construction

## SOLID Principles Checklist

- **SRP**: Each class has one responsibility. Validation separate from business logic.
- **OCP**: Extend behavior via inheritance/interfaces, not modification.
- **LSP**: Derived classes substitutable for base classes.
- **ISP**: Small, focused interfaces over large ones.
- **DIP**: Depend on abstractions, not implementations.

## Design Pattern Review Checklist

When reviewing code for design patterns:
- **Command Pattern**: Handler pattern for operations
- **Factory Pattern**: Complex object creation with DI
- **Repository Pattern**: Data access abstractions
- **Provider Pattern**: External service abstractions
- **Template Method**: Base class with extensible steps
- **Strategy Pattern**: Interchangeable algorithms via interfaces
