# EAF Next Session Prompt P64 - Quality Gate & Technical Debt

## Contexto

O P63 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13310 / 13589) |
| Branch coverage | 90.5% (2597 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4604 total, 4603 passando, 1 ignorado |
| Build warnings | 162 |

Branch ativa: `feature/devin-20260715-priority64-quality-debt` (a criar a partir de `origin/main` no commit do P63).

## Objetivo

Reduzir débito técnico e warnings do build/SonarCloud sem diminuir a cobertura de testes. Foco em:

1. Revisar e corrigir warnings de build que são seguros de alterar (evitar modificar `.github/workflows/`).
2. Tratar os apontamentos do SonarCloud para as classes com cobertura abaixo de 100% quando possível sem modificar comportamento.
3. Garantir que `Templates/Api`, `Templates/Worker` e `Templates/Angular/Eaf.ProjectName.UI` continuem buildando.

## Tarefas

1. Executar `dotnet build Eaf.sln --configuration Release` e catalogar os 162 warnings por tipo/projeto.
2. Priorizar correções que reduzam warnings sem alterar lógica de negócio:
   - Anotações de nulidade (`CS8600`, `CS8602`, `CS8625`) em projetos de teste.
   - Warnings de pacotes obsoletos/configuração (avaliar se é seguro atualizar versões em `Directory.Build.props`/`common.props` ou arquivos `.csproj` dos templates).
3. Verificar o quality gate do SonarCloud após o merge do P63 e tratar issues classificadas como `Bug` ou `Vulnerability` de baixo risco, se houver.
4. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
5. Build dos templates:
   - `dotnet build Templates/Api/Eaf.ProjectName.sln --configuration Release`
   - `dotnet build Templates/Worker/Eaf.ProjectName.WorkerService.sln --configuration Release`
   - `cd Templates/Angular/Eaf.ProjectName.UI && source /home/ubuntu/.nvm/nvm.sh && nvm use 20 && npm install --legacy-peer-deps && npx ng build --configuration=production`
6. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p64.md`, `docs/development/session-summaries/eaf-next-session-prompt-p65.md` e `.agents/MEMORY.md` com as métricas finais e notas.
7. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P63 (aprendizados)

- SonarCloud duplication no PR #197 foi resolvido com `sonar.cpd.exclusions=Templates/**` e `/d:sonar.cpd.exclusions="Templates/**"`.
- `EafHangfireAuthorizationFilter` atingiu 100% de line coverage com um `FakeServiceProvider` simulando `ISupportRequiredService`.
- `PermissionAppService.AddPermission` (`permission.Children == null`), `LdapSettings.GetContextType`, `AzureActiveDirectoryAuthenticationSource`, `LdapAuthenticationSource`, `MiddlewareAppServiceBase.GetCurrentTenant`, `EafSqliteCache.Connect` outer catch, `ServiceBusQueueAppender.OnClose` catch, `EafHangfireApplicationBuilderExtensions.UseEafHangfire` `DisplayNameFunc`, `TokenAuthController` e `MiddlewareWebCoreModule` possuem ramos Windows/infrastructure-limited que devem ser mantidos como inalcançáveis em Linux.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p63.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
