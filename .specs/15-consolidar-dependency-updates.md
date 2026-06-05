# 15 — Consolidar Atualizações de Dependências (PRs Dependabot)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 0 — Pré-requisito (executar ANTES de todas as outras fases) |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Apenas bumps de versão patch/minor |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 9 arquivos .csproj |
| **Status** | ✅ CONCLUÍDA — PR criado e PRs Dependabot fechados |

## Objetivo

Consolidar 5 PRs Dependabot (#52-#56) em uma única atualização:

| PR | Pacote | De → Para |
|---|--------|----------|
| #56 | Microsoft.EntityFrameworkCore (todas variantes) | 10.0.3 → 10.0.8 |
| #55 | Microsoft.Data.Sqlite.Core | 10.0.3 → 10.0.8 |
| #54 | Microsoft.Data.Sqlite + Sqlite.Core | 10.0.3 → 10.0.8 |
| #53 | Microsoft.AspNetCore.TestHost | 10.0.3 → 10.0.8 |
| #52 | EPPlus | 8.5.4 → 8.6.0 |

## Motivo

- 5 PRs individuais dificultam review e aumentam risco de conflito
- PR #55 é redundante (PR #54 já inclui a mesma atualização + Sqlite base)
- Atualizações devem ir para `develop` primeiro, não direto para `main`
- CI nos PRs individuais falha por problemas pré-existentes em `main`

## Arquivos Afetados

| Arquivo | Mudanças |
|---------|---------|
| `src/Eaf.Middleware.Application/Eaf.Middleware.Application.csproj` | EFCore 10.0.8, EPPlus 8.6.0 |
| `src/Eaf.SqliteCache/Eaf.SqliteCache.csproj` | Sqlite.Core 10.0.8, Sqlite 10.0.8 |
| `Templates/Api/src/.../Eaf.ProjectName.EntityFrameworkCore.csproj` | EFCore.SqlServer, Tools, Design 10.0.8 |
| `Templates/Api/test/.../Eaf.ProjectName.Tests.csproj` | EFCore.InMemory, EFCore.Sqlite 10.0.8, EPPlus 8.6.0 |
| `test/Eaf.MiddlewareCore.SampleApp/...Tests.csproj` | EFCore.SqlServer, Tools, Design 10.0.8 |
| `test/Eaf.MiddlewareCore.Tests/...Tests.csproj` | EFCore.Sqlite, Sqlite 10.0.8 |
| `test/Eaf.KeyVault.AspNetCore.Tests/...Tests.csproj` | TestHost 10.0.8 |
| `test/Eaf.OpenTelemetry.Tests/...Tests.csproj` | TestHost 10.0.8 |
| `test/Eaf.SqliteCache.Tests/...Tests.csproj` | Sqlite 10.0.8 |

## Verificação

```bash
# Build
dotnet build Eaf.sln --configuration Release

# Testes
dotnet test Eaf.sln --configuration Release --no-build

# Resultado esperado: 1159 testes, 0 falhas
```

## Resultado

- Build: ✅ Sucesso (61 warnings — pré-existentes)
- Testes: ✅ 1159 total, 1158 sucesso, 1 skipped, 0 falhas
- PRs Dependabot: Fechados (#52-#56)
