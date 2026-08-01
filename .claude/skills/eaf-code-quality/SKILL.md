---
name: eaf-code-quality
description: >-
  Guidance for analyzing and improving code quality in EAF projects. Covers
  build warning analysis, test coverage improvement, code analysis rules,
  and systematic approaches to technical debt reduction. Use when fixing
  compiler warnings, increasing test coverage, running code analysis, or
  performing code quality audits. Do NOT use for new feature development,
  architectural changes, or frontend development.
metadata:
  version: '1.0.0'
---

# EAF Code Quality Skill

Guidance for systematic code quality improvement in EAF middleware modules.

## Build Warning Analysis

### Common Warning Categories

| Category | Examples | Fix Complexity |
|----------|----------|---------------|
| NU1510 | Unnecessary PackageReferences | Simple: remove from .csproj |
| NU1902/NU1903 | Package vulnerabilities | Moderate: update versions, check compatibility |
| CS1591 | Missing XML documentation | Simple: add `/// <summary>` |
| CS0169 | Unused fields | Simple: remove field |
| CS0219 | Unused variables | Simple: remove variable |
| CA2254 | Non-constant log templates | Simple: use string concatenation |
| CS1572/CS1573 | Mismatched XML param tags | Simple: fix param names |

### Workflow

```bash
# 1. Build and capture all warnings
dotnet build Eaf.sln 2>&1 | tee /tmp/build_output.txt

# 2. Extract unique warnings (excluding NU1902/NU1903 vulnerabilities)
grep "warning " /tmp/build_output.txt | grep -v "NU190[23]" | sort -u

# 3. Count warnings by type
grep "warning " /tmp/build_output.txt | grep -oP "warning \w+" | sort | uniq -c | sort -rn
```

### Package Vulnerability Assessment

Before upgrading packages for NU1902/NU1903:
1. Check if the vulnerability is exploitable in the EAF context
2. Verify compatibility of new version with ABP 10.5.0
3. Run full test suite after upgrade
4. Consider suppressing if risk is low and upgrade is breaking

## Test Coverage Improvement

### Coverage Commands

```bash
# Run tests with coverage
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate HTML report
dotnet tool run reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"Html;TextSummary"

# View summary
cat TestResults/CoverageReport/Summary.txt
```

### Coverage Targets by Module

| Module | Target | Strategy |
|--------|--------|----------|
| Eaf.Castle.Serilog | 80%+ | Test all log levels and format methods |
| Eaf.Log4NetServiceBus | 70%+ | Test LogMessage, LogExtensions; mock ServiceBus |
| Eaf.Middleware.Worker | 50%+ | Test PathUtils, AppConfigurations, AppFolders |
| Eaf.KeyVault | 80%+ | Already has good coverage; fill gaps |
| Eaf.OpenTelemetry | 80%+ | Test configuration and telemetry setup |
| Eaf.SqliteCache/SqlServerCache | 85%+ | Test cache operations |
| Eaf.Middleware.Core | 30%+ | Focus on utilities, extensions, helpers |
| Eaf.Middleware.Application | 30%+ | Focus on DTOs, validators, simple services |
| Eaf.Middleware.Web.Core | 15%+ | Focus on models, helpers, URL services |

### Writing New Tests

Follow BDD pattern in Portuguese:
```csharp
[Fact]
public void Dado_Condicao_Quando_Acao_Entao_ResultadoEsperado()
{
    // Arrange
    var sut = new ClasseEmTeste();

    // Act
    var resultado = sut.MetodoEmTeste();

    // Assert
    resultado.ShouldBe(valorEsperado);
}
```

### Test File Naming

- New test files: `{ClassName}Tests.cs` in the corresponding test project
- Coverage-specific additions: add to existing test file or create `{ClassName}CoverageTests.cs`
- Place in matching namespace/folder structure

### Easy Coverage Wins

1. **DTO/POCO classes**: Property get/set tests, default value verification
2. **Extension methods**: Input/output validation, edge cases
3. **Constants/Enums**: Verify values haven't changed (regression guard)
4. **Utility classes**: FormatSize, IsEmail, PathNavigatesAboveRoot
5. **Format methods**: All overloads of logging methods (Debug/Info/Warn/Error/Fatal/Trace)

### Hard-to-Test Areas (Avoid Unless Critical)

- ABP Application Services (require full DI + DbContext)
- Controllers (require HTTP pipeline)
- SignalR Hubs (require SignalR test infrastructure)
- ServiceBusQueueAppender (requires Azure connection)
- Module initialization (PreInitialize/Initialize lifecycle)

## Code Analysis Configuration

### Suppressed Warnings (common.props)

Currently suppressed in `common.props`:
- CS1591: Missing XML comments (controlled via GenerateDocumentationFile)
- SYSLIB0001-0003: Obsolete crypto/encoding APIs
- MSB3277: Assembly version conflicts
- CS8632: Nullable reference types annotations

### Adding New Suppressions

Only suppress warnings when:
1. The fix would require breaking architectural changes
2. The warning is a false positive in the EAF context
3. The warning applies to third-party/generated code

Add to `common.props` `<NoWarn>` for project-wide, or `#pragma warning disable` for specific lines.

## Quality Metrics

Current baseline (as of PR #706):
- Line coverage: 24.1%
- Branch coverage: 15.5%
- Method coverage: 54.5%
- Total tests: 1492 (1491 passing, 1 skipped)
- Build warnings: 0 CS-level, ~25 NU-level (package vulnerabilities)

Target:
- Line coverage: 30%+
- Method coverage: 60%+
- Zero CS-level warnings
- All tests passing
