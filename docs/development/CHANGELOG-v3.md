# CHANGELOG-v3 — Backport GameHub para EAF

## Resumo

Backport das correções validadas no GameHub para o template EAF, cobrindo CORS, tratamento de erros públicos, login multi-tenant, parsing JWT no frontend, modernização do SignalR, responsividade mobile e alinhamento do header de tenant com o ABP 10.5.

## Mudanças

### Backend

- `Eaf.Middleware.Web.Core/Configuration/EafCorsConfiguration.cs`
  - Extensão `AddEafCors` que reflete a origem real do caller, permite wildcards de subdomínio e expõe os headers enviados pelo `EafHttpInterceptor`.
  - `Regex.IsMatch` recebe `TimeSpan.FromSeconds(1)` para atender regras de segurança do Sonar (S6444).
- `Eaf.Middleware.Web.Core/Middleware/EafPublicErrorMiddleware.cs`
  - Middleware que captura exceções não tratadas e retorna `PublicErrorContract` em JSON com status 400/403/500 apropriados.
  - `WriteAsJsonAsync` recebe `context.RequestAborted` para permitir cancelamento (S8949).
- `Eaf.Middleware.Web.Core/Filters/EafExceptionFilter.cs`
  - Filtro MVC (`IExceptionFilter` + `IAsyncExceptionFilter`) com `IOrderedFilter.Order = 1000` para mapear `UserFriendlyException` em 400 antes do filtro padrão do ABP (os filtros de exceção executam em ordem reversa).
- `Eaf.Middleware.Web.Core/Configuration/AuthConfigurer.cs`
  - `QueryStringTokenResolver` aceita `access_token` na query string para conexões SignalR (`/signalr*`).
- `Eaf.Middleware.Core/EafErrorCodes.cs`
  - Códigos de erro estáveis para respostas públicas.
- `Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`
  - `CreateJwtClaims` emite claim `tenantid` quando o usuário pertence a um tenant.

### Frontend

- `TokenService`
  - Novos métodos: `getPayload`, `isValid`, `getUserId`, `getTenantId`, `getUserName`, `getRoles`, `isInRole`.
- `SignalRHelper`
  - Usa `@microsoft/signalr` `HubConnectionBuilder` com `accessTokenFactory` e `withAutomaticReconnect`.
- `ChatSignalrService` e `app.component.ts`
  - Removem dependência do script `eaf.signalr-client.js` e inicializam conexão moderna.
- `login.component.ts` e `select-tenant.component.*`
  - Fallback para autenticação direta quando o usuário host não tem tenants vinculados.
  - "Login como Host" limpa o cookie de tenant antes de chamar `authenticate`.
- Header/cookie de tenant (`Abp-TenantId`)
  - Todos os clientes e o backend agora usam `Abp-TenantId` (dash), alinhado com o `TenantIdResolveKey` padrão do ABP 10.5.
  - Arquivos ajustados: `EafHttpInterceptor`, `AppPreBootstrap`, `app-auth.service`, `eaf.js`, `MiddlewareControllerBase`, `ConsoleApiClient` e `EafCorsConfiguration`.
  - Não envia mais header/cookie de tenant quando o cookie está ausente ou com valor `null`, evitando fallback acidental para o tenant `1`.
- `EafHttpConfiguration`
  - `handleNonEafErrorResponse` detecta `PublicErrorContract` e exibe `message`/`code` em vez do modal genérico.
- `topbar.component.ts`
  - `setCurrentLoginInformations` protege o acesso a `appSession.user` para evitar topbar em branco.
- Tabelas admin (`tenants.component.html`, `users.component.html`)
  - `p-table` com `[loading]`, `emptyMessage`, `app-status-badge` e `app-empty-state`.
- Responsividade
  - CSS mobile com `100dvh`, touch targets e drawer para sidebars em telas pequenas.

### Testes

- Backend: `EafCorsConfigurationBddTests`, `EafPublicErrorMiddlewareBddTests`, `EafExceptionFilterBddTests`, `AuthConfigurerBddTests`.
- Frontend: `token.service.spec.ts`, `login.component.spec.ts`, `login.service.spec.ts`.

## Referências

- Plano de execução: `feature/eaf-gamehub-backport`
- Branch base: `origin/main`
- Guia detalhado: [`docs/development/gamehub-backport-usage.md`](./gamehub-backport-usage.md)
