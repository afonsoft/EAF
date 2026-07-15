# EAF Next Session Prompt P63 - Coverage Audit Final + Templates

## Contexto

O P62 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13309 / 13589) |
| Branch coverage | 90.4% (2595 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4602 total, 4601 passando, 1 ignorado |
| Build warnings | 162 |

Branch ativa: `feature/devin-20260715-priority63-coverage-audit` (a criar a partir de `origin/main` no commit do P62).

## Objetivo

Finalizar o coverage audit adicionando testes BDD (`Dado/Quando/Então` em português) para os ramos acessíveis das classes ainda abaixo de 100%, manter ou aumentar as métricas atuais e garantir que os templates `Templates/Api`, `Templates/Worker` e `Templates/Angular/Eaf.ProjectName.UI` continuem buildando.

## Tarefas

1. Adicionar/ajustar testes BDD para ramos acessíveis das seguintes classes (foco nas de menor cobertura e com maior impacto):

   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (59.2%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (87.3%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (90.9%)
   - `Eaf.Middleware.Ldap.Configuration.LdapSettings` (91.8%)
   - `Eaf.Middleware.Authorization.Permissions.PermissionAppService` (92.5%)
   - `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (92.8%)
   - `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` (93.5%)
   - `Abp.Runtime.Caching.Sqlite.EafSqliteCache` (96.6%)
   - `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` (96.7%)
   - `Eaf.Middleware.MiddlewareAppServiceBase` (97.3%)
   - `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` (99.2%)

2. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.4%
   - Method coverage >= 99.8%

3. Build Release e rodar `run-tests-with-coverage.sh`:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
   ```

4. Build dos templates:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Templates/Api/Eaf.ProjectName.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Templates/Worker/Eaf.ProjectName.WorkerService.sln --configuration Release
   cd Templates/Angular/Eaf.ProjectName.UI && source /home/ubuntu/.nvm/nvm.sh && nvm use 20 && npm install --legacy-peer-deps && npx ng build --configuration=production
   ```

5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p63.md`, `docs/development/session-summaries/eaf-next-session-prompt-p64.md` e `.agents/MEMORY.md` com as métricas finais e notas.

6. Criar PR para `main`.

## Restrições

- Não modificar código de produção, exceto bugs bloqueantes documentados.
- Não modificar `.github/workflows/`.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`).
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P62 (aprendizados)

- `SerilogLogger` possui um construtor interno sem parâmetros usado por reflection do Castle Windsor; testar via reflection cobre 100% da classe.
- `ChatMessageManager.HandleSenderToReceiverAsync` retorna precocemente quando uma amizade está `FriendshipState.Blocked`; invocar o método privado com uma `Friendship` nesse estado cobre o ramo.
- `EafSqlServerCache.CompressBytesAsync` captura exceções de compressão e retorna os bytes originais; passar `null` dispara `ArgumentNullException` e cobre o `catch`.
- `EafHangfireAuthorizationFilter.IsPermissionGranted` tem retornos defensivos para `userIdentifier == null` e `requiredPermissionName` vazio; ambos são alcançáveis por reflection.
- `Templates/Worker` foi corrigido de namespaces/tipos legados `Eaf.*` para `Abp.*` e de pacotes `Microsoft.Extensions.*` desatualizados, permitindo build com 0 erros.
- `Templates/Api` e `Templates/Angular/Eaf.ProjectName.UI` buildam com sucesso (apenas warnings já conhecidos).
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafSqliteCache`, `ServiceBusQueueAppender`, `TokenAuthController`, `DefaultExternalLoginInfoManager`, `OpenIdConnectAuthProviderApi`, `EafHangfireApplicationBuilderExtensions`, `MiddlewareWebCoreModule`, `LdapSettings`, `UserEmailer` e `AzureActiveDirectoryAuthenticationSource` ainda possuem branches dependentes de infraestrutura real (LDAP, AD, Redis, Hangfire/SQL Server, SignalR, MSAL/Graph) ou são defensivos inalcançáveis em Linux. Documentar como inalcançáveis quando apropriado.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p62.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
