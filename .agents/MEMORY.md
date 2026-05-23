# EAF Agent Memory

This file is automatically maintained by the AI agent to persist learnings about the EAF codebase.

## Recent Learnings

### Module System
- EAF follows ABP module system with lifecycle methods: PreInitialize, Initialize, PostInitialize, Shutdown
- Modules use DependsOn attribute to declare dependencies
- Core modules: Eaf.Middleware.Core, Eaf.Middleware.Application, Eaf.Middleware.Web.Core
- Middleware modules: KeyVault, OpenTelemetry, SqlServerCache, SqliteCache, Castle.Serilog, etc.

### Testing Patterns
- BDD pattern in Portuguese: Dado/Quando/Então (Given/When/Then)
- Test base classes: ProjectNameTestBase for integration tests
- Use Shouldly for assertions, NSubstitute for mocking
- Target 90% code coverage
- Tests in Templates/Api/test/Eaf.ProjectName.Tests

### Build Commands
- Restore: `dotnet restore Eaf.sln`
- Build: `dotnet build Eaf.sln`
- Test with coverage: `dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings`

### API Template
- Located in Templates/Api/
- 122 tests implemented (121 passing, 1 skipped)
- Follows ABP dynamic API generation pattern
- Application Services exposed as REST APIs automatically

### Documentation
- Main docs in docs/ directory
- MODULE_SYSTEM.md contains module system documentation
- API docs in docs/api/
- Architecture docs in docs/architecture/
