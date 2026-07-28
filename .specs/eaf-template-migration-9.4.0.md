# Spec de Migração — Templates EAF 9.3.1 → 9.4.0

## Objetivo

Este documento descreve como atualizar projetos que já foram gerados a partir dos templates EAF (API .NET 10 e Angular) da versão **9.3.1** para **9.4.0**. A versão 9.4.0 agrega as funcionalidades do backport GameHub (CORS, public errors, login multi-tenant, SignalR moderno, responsividade), otimizações de performance/memória e a versão 9.4.0 dos pacotes/módulos EAF.

## Escopo

- **Backend:** projetos baseados em `Templates/Api` (ASP.NET Core 10 + ABP 10.5).
- **Frontend:** projetos baseados em `Templates/Angular/Eaf.ProjectName.UI` (Angular 18/19/20/21 — o spec foca nas mudanças EAF, não em upgrade de versão do Angular).
- **Banco de dados:** migrations EF Core obrigatórias para os novos recursos.

## Pré-requisitos

- [ ] Backup do código e do banco de dados de produção.
- [ ] Branch de migração: `git checkout -b migration/eaf-9.4.0`.
- [ ] .NET 10 SDK e Node.js 18+.
- [ ] Banco de dados SQL Server/PostgreSQL acessível.
- [ ] Solução compila na versão atual (`dotnet build Eaf.ProjectName.sln`).

---

## 1. Atualização de versão dos pacotes EAF

A centralização da versão acontece em `common.props` do repositório EAF. Em projetos gerados que consomem os pacotes NuGet, atualize os comentários de referência (ou as referências reais caso usem NuGet):

```xml
<!-- Templates/Api/src/Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj -->
<!--<PackageReference Include="Eaf.Castle.Serilog" Version="9.4.0" />
<PackageReference Include="Eaf.Middleware.Web.Core" Version="9.4.0" />
<PackageReference Include="Eaf.OpenTelemetry" Version="9.4.0" />-->

<!-- Templates/Api/src/Eaf.ProjectName.Core/Eaf.ProjectName.Core.csproj -->
<!--<PackageReference Include="Eaf.Middleware.Core" Version="9.4.0" />-->

<!-- Templates/Api/src/Eaf.ProjectName.Application/Eaf.ProjectName.Application.csproj -->
<!--<PackageReference Include="Eaf.Middleware.Application" Version="9.4.0" />-->
```

> Projetos que referenciam os projetos-fonte do EAF via `<ProjectReference>` continuam usando a origem local e herdam a versão `9.4.0` do `common.props`.

---

## 2. Backend — API .NET

### 2.1 CORS seguro (`AddEafCors`)

O EAF 9.4.0 substitui a configuração CORS aberta por uma política refletiva de origem, suporte a wildcard de subdomínio e exposição dos headers usados pelo interceptor Angular.

**Ações:**

1. Defina `DefaultCorsPolicyName` em `ProjectNameConsts.cs` (caso ainda não exista):

```csharp
public const string DefaultCorsPolicyName = "ProjectNameCorsPolicy";
```

2. Em `Startup.cs` (`ConfigureServices`), remova `services.AddCors(...)` anterior e registre:

```csharp
services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(EafExceptionFilter), 1000);
    // ... outros filtros
});

services.AddEafCors(
    _appConfiguration,
    _hostingEnvironment.IsDevelopment(),
    ProjectNameConsts.DefaultCorsPolicyName);
```

3. Em `Startup.cs` (`Configure`), após `UseExceptionHandler` e antes de `UseJwtTokenMiddleware`:

```csharp
app.UseEafPublicErrorMiddleware();
app.UseCors(ProjectNameConsts.DefaultCorsPolicyName);
```

4. Configure `App:CorsOrigins` nos `appsettings*.json`:

```json
{
  "App": {
    "CorsOrigins": "https://*.example.com;https://app.example.com"
  }
}
```

- Em `Local`/`Development` pode ser `*`.
- Em `Staging`/`Production` deve ser uma lista explícita; `*` lança `InvalidOperationException`.

### 2.2 Erros públicos (`PublicErrorContract`)

O middleware `EafPublicErrorMiddleware` e o filtro `EafExceptionFilter` padronizam retornos de erro sem stack trace.

**Ações:**

- Certifique-se de que `Startup.cs` inclui:

```csharp
services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(EafExceptionFilter), 1000);
}).AddNewtonsoftJson();
```

```csharp
app.UseEafPublicErrorMiddleware();
```

- Caso tenha copiado `EafExceptionFilter` para customizar, alinhe com a implementação atual em `src/Eaf.Middleware.Web.Core/Filters/EafExceptionFilter.cs` e `EafPublicErrorMiddleware.cs`.

- O mapeamento de status permanece:

| Exceção | Status |
|---------|--------|
| `UserFriendlyException`, `AbpValidationException`, `ArgumentException`, `FormatException` | 400 |
| `AbpAuthorizationException` | 403 |
| Demais | 500 |

### 2.3 Header/cookie de tenant (`Abp-TenantId`)

A chave padrão de resolução de tenant no ABP 10.5 é `Abp-TenantId` (traço). Todos os clientes e o backend devem usar o mesmo nome.

**Ações no backend:**

- `MiddlewareControllerBase.cs`: gravar o cookie de tenant com `eaf.multiTenancy.tenantIdCookieName` (que é `Abp-TenantId`).
- `EafCorsConfiguration.cs` já expõe o header `Abp-TenantId`.
- `AuthConfigurer.SetToken` lê `access_token` da query string para conexões SignalR (`/signalr*`).

### 2.4 Login multi-tenant em duas etapas

Novos endpoints no `TokenAuthController`:

- `GetAvailableTenants` — retorna os tenants vinculados ao usuário host.
- `SelectTenant` — autentica em um tenant específico.

**Ações:**

- Verifique se `ProjectNameCoreModule` depende de `MiddlewareCoreModule`:

```csharp
[DependsOn(typeof(MiddlewareCoreModule))]
public class ProjectNameCoreModule : AbpModule
```

- `ProjectNameDbContext` deve conter:

```csharp
public virtual DbSet<UserTenantMembership> UserTenantMemberships { get; set; }
```

```csharp
modelBuilder.Entity<UserTenantMembership>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
    b.HasIndex(e => e.TenantUserId);
});
```

### 2.5 Migrations EF Core obrigatórias

O template 9.4.0 inclui duas migrations. Para projetos existentes, gere equivalentes ou aplique as migrations do template (se o modelo coincidir):

```bash
# Na pasta do projeto EntityFrameworkCore
dotnet ef migrations add AddContextualChatFields --project src/Eaf.ProjectName.EntityFrameworkCore
dotnet ef migrations add AddUserTenantMembership --project src/Eaf.ProjectName.EntityFrameworkCore

# Em seguida, atualize o banco
dotnet ef database update --project src/Eaf.ProjectName.EntityFrameworkCore
```

**Campos adicionados em `EafChatMessages`:**

- `ClientMessageId` (`nvarchar(64)`, nullable)
- `ContextType` (`nvarchar(64)`, nullable)
- `ConversationId` (`uniqueidentifier`, nullable)
- `GameId` (`uniqueidentifier`, nullable)
- `MatchId` (`uniqueidentifier`, nullable)

**Tabela criada:** `AbpUserTenantMemberships` (`Id`, `UserId`, `TenantId`, `TenantUserId`, `IsDefault`, `CreationTime`, `CreatorUserId`).

### 2.6 Contratos compartilhados para consumidores (opcional)

Se o projeto usa chat/notificações com contexto de jogo/partida, adicione os contratos no `Application/Contracts` do template:

- `ContextualChatMessageContract`
- `RateLimitContract`
- `ModerationAuditContract`
- `PublicErrorContract`

Consulte `docs/integration/gamehub-consumer-contracts.md` para regras de versionamento e uso.

### 2.7 Otimizações de performance e memória

A versão 9.4.0 inclui otimizações nos módulos EAF. Projetos gerados não precisam de mudanças de código, apenas atualizar a versão dos pacotes/referências. Detalhes técnicos estão em `docs/performance-memory-optimizations.md`.

Pontos principais:

- `WebLogAppService.GetLatestWebLogs` lê no máximo 1 MB do arquivo.
- `ChatAppService` processa mensagens não lidas em lotes de 1.000.
- `EafSqlServerCache` usa `ArrayBufferWriter<byte>` + `Utf8JsonWriter`.
- `ProjectNameDbContext` chama `EnsureMigrated` com lock estático e `IsDesignTime` para design-time.

---

## 3. Frontend — Angular

### 3.1 Configuração de multi-tenancy (`AppConsts`)

Adicione os novos flags em `src/shared/AppConsts.ts`:

```typescript
static readonly multiTenancy = {
  twoStepLogin: false,
};

static autoSelectSingleTenant = true;
```

- `twoStepLogin: true` ativa login em duas etapas para usuários host.
- `autoSelectSingleTenant` define se, com apenas um tenant disponível, o login é automático.

### 3.2 Cookie/header de tenant (`Abp-TenantId`)

**Ações:**

- `src/assets/lib/eaf-web-resources/eaf.js`:

```javascript
eaf.multiTenancy.tenantIdCookieName = 'Abp-TenantId';
```

- `src/assets/lib/eaf-ng2-module/src/eafHttpInterceptor.ts`:

```typescript
protected addTenantIdHeader(headers: HttpHeaders): HttpHeaders {
  const tenantIdCookieName = (window as any).eaf?.multiTenancy?.tenantIdCookieName || 'Abp-TenantId';
  const cookieTenantIdValue = this._storageService.getCookieValue(tenantIdCookieName);
  if (cookieTenantIdValue && headers && !headers.has(tenantIdCookieName)) {
    headers = headers.set(tenantIdCookieName, cookieTenantIdValue);
  }
  return headers;
}
```

- `src/AppPreBootstrap.ts` e `src/app/shared/common/auth/app-auth.service.ts`: somente enviar o header/cookie quando houver um `tenantId` válido. Não enviar `null`, `undefined` ou string vazia.

### 3.3 `TokenService` — parsing de JWT

Atualize ou substitua `src/assets/lib/eaf-ng2-module/src/auth/token.service.ts` para incluir:

```typescript
export interface TokenPayload {
  sub?: string;
  unique_name?: string;
  name?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  nameidentifier?: string;
  role?: string | string[];
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
  exp?: number;
  tenantid?: string;
}

getPayload(token?: string): TokenPayload | null { ... }
isValid(): boolean { ... }
getUserId(): number | null { ... }
getTenantId(): number | null { ... }
getUserName(): string | null { ... }
getRoles(): string[] { ... }
isInRole(role: string): boolean { ... }
```

Use como:

```typescript
import { TokenService } from '@eaf/auth/token.service';

console.log(this.tokenService.getUserId());
console.log(this.tokenService.getTenantId());
console.log(this.tokenService.getRoles());
```

### 3.4 Login em duas etapas

**Arquivos que precisam ser sincronizados com o template 9.4.0:**

- `src/account/login/login.service.ts` — adiciona `availableTenantsResult`, `availableTenants(model)`, `selectTenant(model)` e `loginTenant(result, tenantId)`.
- `src/account/login/login.component.ts` — `login()` decide entre `normalLogin()` e `twoStepLogin()`; `twoStepLogin()` trata 0, 1 ou N tenants.
- `src/account/login/select-tenant/select-tenant.component.ts` (novo) — exibe a lista e oferece `loginAsHost()`.

**Fluxo:**

1. Usuário preenche usuário/senha.
2. `twoStepLogin` chama `GetAvailableTenants`.
3. Se `length === 0`, chama `authenticate` direto (fallback host).
4. Se `length === 1 && autoSelectSingleTenant`, chama `selectTenant` e loga.
5. Caso contrário, navega para `select-tenant`.
6. `loginAsHost()` limpa o cookie `Abp-TenantId` e chama `authenticate`.

### 3.5 SignalR moderno

Atualize `src/shared/helpers/SignalRHelper.ts`:

```typescript
import * as signalR from '@microsoft/signalr';
import { TokenService } from '@eaf/auth/token.service';
import { AppConsts } from '@shared/AppConsts';

export class SignalRHelper {
  private static _tokenService: TokenService;

  static init(tokenService: TokenService): void {
    this._tokenService = tokenService;
  }

  static buildConnection(hubUrl: string = '/signalr'): signalR.HubConnection {
    const base = (AppConsts.remoteServiceBaseUrl || '').replace(/\/$/, '');
    const fullUrl = base + hubUrl;

    return new signalR.HubConnectionBuilder()
      .withUrl(fullUrl, {
        accessTokenFactory: () => this._tokenService?.getToken() ?? '',
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }
}
```

Remova o uso do script legado `eaf.signalr-client.js` de `ChatSignalrService` e `app.component.ts`.

### 3.6 Tratamento de erros públicos no frontend

Atualize `EafHttpConfiguration.handleNonEafErrorResponse` (em `src/assets/lib/eaf-ng2-module/src/eafHttpInterceptor.ts` ou arquivo equivalente) para exibir `body.message`/`body.code` quando a resposta contiver um `PublicErrorContract`:

```typescript
handleNonEafErrorResponse(response: any) {
  const body = response.error ?? response.body;
  if (body?.message) {
    // mostrar mensagem do servidor em vez de modal genérico
    this.message.error(body.message, body.code || 'Error');
    return;
  }
  // fallback anterior
}
```

### 3.7 Componentes reutilizáveis

Adicione os componentes no template do projeto e declare/exporte em `src/app/shared/common/app-common.module.ts`:

```typescript
import { EmptyStateComponent } from '../components/empty-state/empty-state.component';
import { StatusBadgeComponent } from '../components/status-badge/status-badge.component';

@NgModule({
  declarations: [..., EmptyStateComponent, StatusBadgeComponent],
  exports: [..., EmptyStateComponent, StatusBadgeComponent],
})
export class AppCommonModule {}
```

**Uso em tabelas:**

```html
<!-- status -->
<app-status-badge
  [value]="record.isActive"
  [trueLabel]="'Yes' | localize"
  [falseLabel]="'No' | localize">
</app-status-badge>

<!-- empty state -->
<app-empty-state [message]="'NoData' | localize"></app-empty-state>
```

Atualize `tenants.component.html` e `users.component.html` para usar `[loading]`, `emptyMessage`, `app-status-badge` e `app-empty-state` conforme o template 9.4.0.

### 3.8 Responsividade mobile

Sincronize `src/assets/common/styles/styles.css` com as mudanças do template:

- `min-height: 100dvh` no container principal.
- Touch targets mínimos de `44px`.
- Centralização do painel de login em telas pequenas.
- Drawer ajustável para sidebars.

### 3.9 Proteção do `TopBarComponent`

Atualize `src/app/shared/layout/topbar.component.ts`:

```typescript
setCurrentLoginInformations(): void {
  const user = this.appSession.user;
  this.shownLoginName = user ? this.appSession.getShownLoginName() : '';
  this.shownFullName = user ? `${user.name} ${user.surname || ''}`.trim() : '';
  this.tenancyName = this.appSession.tenancyName || '';
  this.userName = user ? user.userName : '';
}
```

### 3.10 Regeneração dos service proxies

Após atualizar a API, regenere os clientes TypeScript:

```bash
cd src/angular  # ou pasta do frontend
npm run service-update  # executa nswag
```

> Não edite `service-proxies.ts` manualmente.

---

## 4. Worker (quando aplicável)

Projetos baseados em `Templates/Worker` devem:

- Garantir que `ProjectNameCoreModule` dependa de `MiddlewareCoreModule`.
- Alinhar `common.props` com a versão 9.4.0 dos módulos EAF (caso referencie NuGet).
- Verificar `WorkerModule.cs` para incluir `MiddlewareWorkerModule`.

---

## 5. Validação

### 5.1 Backend

```bash
dotnet build Eaf.ProjectName.sln -c Release
dotnet test Eaf.ProjectName.sln
dotnet ef database update --project src/Eaf.ProjectName.EntityFrameworkCore
```

Verifique:

- Swagger responde em `/swagger`.
- Login com tenant e host funciona.
- CORS preflight retorna `200` com origem refletida.
- Resposta de erro inválida retorna `PublicErrorContract`.

### 5.2 Frontend

```bash
cd src/angular  # ou pasta do frontend
npm ci --legacy-peer-deps
npm run build
npm run test
```

Verifique:

- Login normal e two-step funcionam.
- Seleção de tenant aparece para usuários com múltiplos tenants.
- Login como host funciona.
- SignalR conecta (`/signalr`) com token via query string.
- Tabelas admin usam `app-status-badge` e `app-empty-state`.
- Responsividade em mobile não quebra layout.

### 5.3 Docker Compose (opcional)

```bash
docker compose -f docker-compose.all.yml up --build -d
# ou scripts/validate-docker-compose.sh
```

---

## 6. Rollback

1. Restaure o backup do banco.
2. Reverta `git checkout` para a branch anterior.
3. Caso migrations já tenham sido aplicadas, execute `dotnet ef database update <MigrationAnterior>`.

---

## 7. Referências

- `docs/development/gamehub-backport-usage.md` — guia de uso das funcionalidades do backport.
- `docs/development/CHANGELOG-v3.md` — resumo técnico das mudanças do backport GameHub.
- `docs/performance-memory-optimizations.md` — detalhes das otimizações de performance/memória.
- `docs/integration/gamehub-consumer-contracts.md` — contratos para consumidores realtime/sociais.
- `docs/eaf-multi-tenant-login.md` — fluxo de login multi-tenant em duas etapas.
- `docs/eaf-tenant-user-manager.md` — `TenantUserManager` e shadow users.
- `CHANGELOG.md` — histórico completo de mudanças.
