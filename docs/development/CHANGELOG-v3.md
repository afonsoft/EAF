# CHANGELOG-v3 — Backport GameHub para EAF

## Resumo

Backport das correções validadas no GameHub para o template EAF, cobrindo CORS, tratamento de erros públicos, login multi-tenant, parsing JWT no frontend, modernização do SignalR e responsividade mobile.

## Mudanças

### Backend

- `Eaf.Middleware.Web.Core/Configuration/EafCorsConfiguration.cs`
  - Extensão `AddEafCors` que reflete a origem real do caller, permite wildcards de subdomínio e expõe os headers enviados pelo `EafHttpInterceptor`.
- `Eaf.Middleware.Web.Core/Middleware/EafPublicErrorMiddleware.cs`
  - Middleware que captura exceções não tratadas e retorna `PublicErrorContract` em JSON com status 400/403/500 apropriados.
- `Eaf.Middleware.Web.Core/Filters/EafExceptionFilter.cs`
  - Filtro MVC com `IOrderedFilter.Order = -1000` para mapear `UserFriendlyException` em 400 antes do filtro padrão do ABP.
- `Eaf.Middleware.Web.Core/Configuration/AuthConfigurer.cs`
  - `QueryStringTokenResolver` aceita `access_token` na query string para conexões SignalR.
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
