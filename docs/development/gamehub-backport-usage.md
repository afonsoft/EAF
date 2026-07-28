# Guia de Uso — Backport GameHub para EAF

Este guia descreve as novas funcionalidades trazidas do GameHub para o template EAF, como configurá-las e como usar cada método/API público.

## 1. CORS seguro (`AddEafCors`)

### O que faz

A extensão `EafCorsConfiguration.AddEafCors` registra uma política CORS nomeada que:

- Reflete a origem real da requisição (não emite `Access-Control-Allow-Origin: *`).
- Suporta wildcard de subdomínio (ex.: `https://*.example.com`).
- Permite credenciais (`AllowCredentials`).
- Expõe os headers usados pelo `EafHttpInterceptor`.

### Localização

- Código: `src/Eaf.Middleware.Web.Core/Configuration/EafCorsConfiguration.cs`
- Registro no template API: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs`

### Uso

```csharp
services.AddEafCors(
    _appConfiguration,
    _hostingEnvironment.IsDevelopment(),
    ProjectNameConsts.DefaultCorsPolicyName);

app.UseCors(ProjectNameConsts.DefaultCorsPolicyName);
```

### Configuração (`appsettings.json`)

```json
{
  "App": {
    "CorsOrigins": "https://app.example.com;https://*.example.com"
  }
}
```

Em produção, `App:CorsOrigins` não pode ser `*` nem vazio; a extensão lança `InvalidOperationException`.

## 2. Erros públicos (`PublicErrorContract`)

### O que faz

Quando uma exceção não tratada ocorre, a API retorna um JSON padronizado com `code`, `message`, `retryable` e `correlationId`.

- Middleware: `EafPublicErrorMiddleware`
- Filtro MVC: `EafExceptionFilter` (mapeia `UserFriendlyException` para `400 Bad Request`)
- Códigos: `EafErrorCodes`

### Contrato

```json
{
  "code": "validation_failed",
  "message": "Falha no login!",
  "retryable": false,
  "correlationId": "0H..."
}
```

### Registro no template

```csharp
services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(EafExceptionFilter), 1000);
});

app.UseEafPublicErrorMiddleware();
```

### Mapeamento de status

| Exceção | Status |
|---------|--------|
| `UserFriendlyException`, `AbpValidationException`, `ArgumentException`, `FormatException` | `400` |
| `AbpAuthorizationException` | `403` |
| `InvalidOperationException`, `TimeoutException` | `500` |
| Outras | `500` |

### Consumo no Angular

O `EafHttpConfiguration.handleNonEafErrorResponse` detecta automaticamente corpos com `message` e exibe o texto do servidor em vez do modal genérico.

## 3. Header de tenant (`Abp-TenantId`)

### O que mudou

A partir do ABP 10.5, a chave padrão de resolução de tenant (`TenantIdResolveKey`) é **`Abp-TenantId`** (traço), não `Abp.TenantId` (ponto). Todos os clientes do EAF foram alinhados para usar o mesmo nome de cookie e header:

- `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-web-resources/eaf.js` define `tenantIdCookieName = 'Abp-TenantId'`
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/eafHttpInterceptor.ts` lê o cookie e envia o header usando `eaf.multiTenancy.tenantIdCookieName`
- `Templates/Angular/Eaf.ProjectName.UI/src/AppPreBootstrap.ts` só envia o header quando existe valor de tenant
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/auth/app-auth.service.ts` só envia o header no logout quando existe valor de tenant
- `src/Eaf.Middleware.Web.Core/Configuration/EafCorsConfiguration.cs` permite apenas `Abp-TenantId`
- `src/Eaf.Middleware.Web.Core/Controllers/MiddlewareControllerBase.cs` grava o cookie com nome `Abp-TenantId` (anteriormente `Eaf.TenantId`)
- `Templates/Api/test/Eaf.ProjectName.ConsoleApiClient/Program.cs` envia `Abp-TenantId` (anteriormente `Eaf.TenantId`)

### Comportamento

- Se houver cookie de tenant, o header `Abp-TenantId` é enviado com o valor do tenant.
- Se não houver cookie, **nenhum header** é enviado, mantendo o contexto de host.
- O header/cookie nunca é enviado com valor `null`, `undefined` ou string vazia.

### Problemas comuns

Se a UI fizer login como host mesmo com um tenant selecionado, verifique se o `EafHttpInterceptor` envia `Abp-TenantId` e se `eaf.js` define `tenantIdCookieName` como `Abp-TenantId`.

## 4. Login multi-tenant

### `TokenAuthController`

- `GetAvailableTenants`: retorna os tenants vinculados ao usuário (host context).
- `SelectTenant`: autentica em um tenant específico usando `tenantId` no body.
- `Authenticate`: usa o tenant resolvido pelo header `Abp-TenantId`.

### Fluxo do Angular

1. Usuário seleciona o tenant no dropdown (`login.component.ts` `selectTenant`).
2. O cookie `Abp-TenantId` é atualizado.
3. `login()` chama `authenticate`.
4. Se `twoStepLogin` estiver habilitado:
   - `availableTenants` retorna a lista de tenants do usuário.
   - Se `length === 0`, chama `authenticate` direto (fallback host).
   - Se `length === 1` e `autoSelectSingleTenant`, chama `selectTenant`.
   - Caso contrário, navega para `select-tenant`.
5. `SelectTenantComponent` oferece `loginAsHost()`, que limpa o cookie e chama `authenticate`.

### Uso programático

```typescript
// login.component.ts
this.loginService.authenticate(
  () => { /* callback */ },
  redirectUrl,
  captchaToken,
);
```

## 5. `TokenService` — parsing de JWT

### Localização

`Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts`

### Métodos

| Método | Retorno | Descrição |
|--------|---------|-----------|
| `getPayload(token?)` | `TokenPayload \| null` | Decodifica o payload do JWT sem validar assinatura. |
| `isValid()` | `boolean` | Verifica se o token decodifica e, se tiver `exp`, se ainda é válido. |
| `getUserId()` | `number \| null` | Extrai `sub` ou `nameidentifier`. |
| `getTenantId()` | `number \| null` | Extrai o claim `tenantid`. |
| `getUserName()` | `string \| null` | Extrai `unique_name`, `name` ou claim de nome. |
| `getRoles()` | `string[]` | Normaliza o claim `role` (string ou array). |
| `isInRole(role)` | `boolean` | Comparação case-insensitive com as roles. |

### Exemplo

```typescript
import { TokenService } from '@eaf/auth/token.service';

constructor(private tokenService: TokenService) {}

ngOnInit(): void {
  console.log('UserId', this.tokenService.getUserId());
  console.log('TenantId', this.tokenService.getTenantId());
  console.log('Roles', this.tokenService.getRoles());
}
```

O backend emite o claim `tenantid` em `TokenAuthController.CreateJwtClaims` quando `user.TenantId` tem valor.

## 6. SignalR moderno

### `SignalRHelper`

`Templates/Angular/Eaf.ProjectName.UI/src/shared/helpers/SignalRHelper.ts`

```typescript
SignalRHelper.init(this._tokenService);
const connection = SignalRHelper.buildConnection('/signalr-chat');
```

Características:

- Usa `@microsoft/signalr`.
- Envia o token via `accessTokenFactory`.
- Habilita `withAutomaticReconnect`.
- Permite WebSockets, Server-Sent Events e Long Polling.

### Backend

`AuthConfigurer.SetToken` lê o query string `access_token` para paths que começam com `/signalr`:

```csharp
if (path.StartsWith("/signalr"))
{
    var accessToken = context.HttpContext.Request.Query["access_token"].FirstOrDefault();
    if (!string.IsNullOrEmpty(accessToken) && accessToken != "null")
    {
        context.Token = accessToken;
        return Task.CompletedTask;
    }
}
```

## 7. Responsividade mobile

Ajustes em `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/styles.css`:

- `min-height: 100dvh` para evitar barras de rolagem extras em mobile.
- Touch targets mínimos de `44px`.
- Centralização do painel de login em telas pequenas.
- Drawer ajustável para sidebars.

## 8. Componentes reutilizáveis

### `app-status-badge`

`Templates/Angular/Eaf.ProjectName.UI/src/app/shared/components/status-badge/status-badge.component.ts`

```html
<app-status-badge
  [value]="record.isActive"
  [trueLabel]="'Yes' | localize"
  [falseLabel]="'No' | localize">
</app-status-badge>
```

### `app-empty-state`

`Templates/Angular/Eaf.ProjectName.UI/src/app/shared/components/empty-state/empty-state.component.ts`

```html
<app-empty-state [message]="'NoData' | localize"></app-empty-state>
```

Ambos são declarados/exportados em `app-common.module.ts`.

## 9. Testes e sondas

### Backend

```bash
dotnet test test/Eaf.Middleware.Web.Core.Tests/Eaf.Middleware.Web.Core.Tests.csproj -c Release
```

### Frontend

```bash
cd Templates/Angular/Eaf.ProjectName.UI
nvm use 20
npm ci --legacy-peer-deps
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
npx ng build --configuration=production
```

### Sondas Docker

Veja `.agents/skills/testing-eaf-docker/SKILL.md` para comandos de validação de login, CORS, SignalR e public errors usando `curl`.

## 10. Referências

- CHANGELOG: `CHANGELOG.md`
- Resumo do backport: `docs/development/CHANGELOG-v3.md`
- Memória cross-sessão: `.agents/MEMORY.md` (seção "GameHub backport")
