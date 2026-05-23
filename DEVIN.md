# Devin Configuration — EAF

> Referência principal: [AGENTS.md](AGENTS.md)

## Delta Devin

### Ambiente
- .NET 10.0 pré-instalado no snapshot
- Node 18 via `nvm use 18` para Angular
- `npm install --legacy-peer-deps` obrigatório no Template Angular

### Skills
Skills carregadas de `.agents/skills/` sob demanda. Invocar pelo nome:
- `eaf-api`, `eaf-modules`, `eaf-testing`, `eaf-ui`, `eaf-cicd`
- `dotnet-best-practices`, `analyzing-dotnet-performance`
- `aspnet-boilerplate-development`, `aspnet-boilerplate-modules`

### Comandos Rápidos
```bash
dotnet restore Eaf.sln
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### PRs
- Branch: `devin/<timestamp>-<descricao>`
- Target: `develop` (features/bugs) ou `main` (hotfixes)
- CI deve passar antes de notificar o usuário

### Notas
- `service-proxies.ts` é gerado pelo NSwag (16k+ linhas) — não editar manualmente
- Angular 15 no Template UI usa Karma/Jasmine, não Jest
- Postinstall corrige BOM issue no `devtools-ignore-plugin.js`
