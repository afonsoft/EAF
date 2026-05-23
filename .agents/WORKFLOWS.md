# WORKFLOWS.md — EAF Automação

## Workflow: Desenvolvimento de Feature

### Precondições
- Branch criada a partir de `develop`
- Naming: `feature/<descricao>` ou `devin/<timestamp>-<descricao>`

### Passos
1. Criar branch a partir de `develop`
2. Implementar alterações seguindo AGENTS.md
3. Escrever testes (BDD: Dado/Quando/Então)
4. Executar verification loop local
5. Criar PR para `develop`
6. Aguardar CI (build + testes + cobertura)
7. Code review
8. Merge via squash

### Critérios de Sucesso
- Todos os testes passam
- Cobertura não diminuiu
- APIs públicas documentadas
- CI verde

---

## Workflow: Bug Fix

### Precondições
- Issue documentada com reprodução
- Branch: `bug/<descricao>` ou `hotfix/<descricao>`

### Passos
1. Reproduzir o bug (teste que falha)
2. Implementar correção
3. Verificar que teste agora passa
4. Executar suite completa
5. Criar PR

### Critérios de Sucesso
- Teste de regressão incluído
- Nenhum teste existente quebrado

---

## Workflow: Novo Módulo Middleware

### Precondições
- Aprovação para criar novo módulo

### Passos
1. Criar projeto em `src/Eaf.<NomeModulo>/`
2. Implementar `EafModule` com lifecycle (PreInitialize, Initialize, PostInitialize)
3. Declarar dependências com `[DependsOn]`
4. Criar projeto de teste em `test/Eaf.<NomeModulo>.Tests/`
5. Adicionar ao `Eaf.sln`
6. Documentar em `docs/`
7. Criar SKILL.md em `.agents/skills/eaf-<nome>/`

---

## Verification Loop

```
Agent Output → Lint → Tests → CI → Human Review
     ↑                                    |
     └────── Ajustar (máx. 2x) ──────────┘
```

### Execução Local
```bash
# 1. Build
dotnet build Eaf.sln --configuration Release

# 2. Testes com cobertura
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# 3. Angular lint (se aplicável)
cd Templates/Angular/Eaf.ProjectName.UI && npx eslint src

# 4. Angular testes (se aplicável)
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

### Execução CI
- Automática via `ci-build-test.yml` em push/PR
- Resultados em GitHub Actions checks

---

## Workflow: Release

### Precondições
- Todos os testes passam em `develop`
- Changelog atualizado

### Passos
1. Criar branch `release/<versao>` a partir de `develop`
2. Atualizar versão em `common.props`
3. Atualizar `CHANGELOG.md`
4. Criar PR para `main`
5. Após merge: tag + publish NuGet automático

---

## Estratégia de Rollback

- **Código**: `git revert` do commit problemático
- **NuGet**: unlist da versão + publish de patch
- **CI**: rerun do workflow anterior
- **Database**: migration down (se aplicável)

---

## Trigger Conditions

| Evento | Workflow |
|--------|---------|
| Push em `develop`, `feature/*`, `bug/*` | `ci-build-test.yml` |
| PR para `develop` ou `main` | `ci-build-test.yml` + `code-quality.yml` |
| Tag de release | `publish-all.yml` |
| Push em `main` | `auto-pr-from-main.yml` |
| Schedule (semanal) | `security-scan.yml` |
| Manual | `release.yml` |
