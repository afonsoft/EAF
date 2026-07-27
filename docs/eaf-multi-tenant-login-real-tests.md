# Plano de Testes Reais — Login Multi-Tenant em Duas Etapas

Este documento descreve como executar testes reais do fluxo multi-tenant usando Docker Compose, bem como os cenários validados e os resultados esperados.

## Infraestrutura

O EAF utiliza SQL Server por padrão no template API. O ambiente de teste sobe:

- `sqlserver` (mcr.microsoft.com/mssql/server:2022-latest)
- `redis` (redis:7-alpine)
- `eaf.projectname.web.host` (API EAF com migration automática)

### Arquivo `Templates/Api/docker-compose.real-tests.yml`

```yaml
version: '3.4'

services:
  eaf.projectname.web.host:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - App__CorsOrigins=*
      - RedisCache__IsEnabled=true
      - RedisCache__ConnectionString=redis:6379
    depends_on:
      - sqlserver
      - redis
```

### Subir o ambiente

```bash
cd Templates/Api
export ConnectionStrings__Default='Server=sqlserver,1433;Database=EafProjectNameDb;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=true'
docker compose -f docker-compose.yml -f docker-compose.infra.yml -f docker-compose.override.yml -f docker-compose.real-tests.yml up -d --build
```

Aguardar `docker ps` mostrar todos os containers `healthy`.

## Cenários de teste

### CT-01: Primeiro acesso do admin host

**Passos:**
1. `POST /api/TokenAuth/Authenticate` com `admin` / `123qwe`.
2. Esperar `shouldResetPassword = true` e obter `passwordResetCode`.
3. `POST /api/services/app/Account/ResetPassword` com a nova senha.
4. `POST /api/TokenAuth/Authenticate` com a nova senha e obter `accessToken`.

**Resultado esperado:** Token host gerado com sucesso.

### CT-02: Criar múltiplos tenants

**Passos:**
1. Com token do admin host, chamar `POST /api/services/app/Tenant/CreateTenant` para `EmpresaA` e `EmpresaB`.
2. `GET /api/services/app/Tenant/GetTenants` e anotar os IDs (ex.: `EmpresaA` = 2, `EmpresaB` = 3).

**Resultado esperado:** Ambos os tenants criados e ativos.

### CT-03: Criar usuário host

**Passos:**
1. `POST /api/services/app/User/CreateOrUpdateUser` com `tenantId` nulo (host) e `assignedRoleNames: ["Admin"]`.

**Resultado esperado:** Usuário host criado com sucesso.

### CT-04: Usuário host sem memberships

**Passos:**
1. `POST /api/TokenAuth/GetAvailableTenants` com o usuário host.

**Resultado esperado:** Lista vazia (`[]`).

### CT-05: Selecionar tenant

**Passos:**
1. `POST /api/TokenAuth/SelectTenant` com `tenantId` de `EmpresaA`.
2. Repetir para `EmpresaB`.

**Resultado esperado:**
- Sucesso em ambas.
- Cada resposta retorna `accessToken` e `userId` diferentes (shadow users distintos).

### CT-06: Listar memberships após seleção

**Passos:**
1. `POST /api/TokenAuth/GetAvailableTenants` novamente.

**Resultado esperado:** Lista contém `EmpresaA` e `EmpresaB`.

### CT-07: Token escopado funciona dentro do tenant

**Passos:**
1. Usar o `accessToken` retornado por `SelectTenant` para `EmpresaA`.
2. Chamar `GET /api/services/app/User/GetUsers` com header `Abp.TenantId: 2`.
3. Tentar `GET /api/services/app/Tenant/GetTenants` com o mesmo token.

**Resultado esperado:**
- `GetUsers` retorna os usuários do tenant (`admin@empresaa.com` e shadow `shareduser`).
- `GetTenants` retorna 403/401 de autorização, porque a permissão `Tenants` é host-only.

### CT-08: Performance

**Passos:**
1. Executar 10 chamadas sequenciais de `GetAvailableTenants`.
2. Executar 10 chamadas sequenciais de `SelectTenant` para cada tenant.

**Resultado esperado:**
- `GetAvailableTenants` média abaixo de 300 ms.
- `SelectTenant` reutilizando shadow user média abaixo de 600 ms.
- Primeira seleção de um novo tenant pode passar de 1 s por causa da criação do shadow user.

## Checklist de regressão

- [ ] Login normal (dropdown de tenant) continua funcionando.
- [ ] `ng build --configuration=production` passa.
- [ ] `dotnet test Eaf.sln` passa.
- [ ] Docker Compose sobe sem erros.

## Problemas encontrados em execuções anteriores

| Problema | Causa | Correção |
|---|---|---|
| `SelectTenant` falhava com `Passwords must have at least one non alphanumeric character` | `User.CreateRandomPassword()` gera apenas hex | `TenantUserManager.GenerateShadowPassword()` gera senha com todos os requisitos do Identity |
| Container API reiniciava em produção | `App:CorsOrigins` = `*` não é permitido em produção | Usar `ASPNETCORE_ENVIRONMENT=Development` para testes ou definir origens explícitas |

## Próximos testes sugeridos

1. Testar com PostgreSQL adicionando provider `Npgsql.EntityFrameworkCore.PostgreSQL` e um `docker-compose.postgres.yml`.
2. Validar cache Redis para permissões após `SelectTenant`.
3. Testar logout e troca de tenant sem recarregar o Angular.
4. Medir tempo com `dotnet-counters` e `wrk` para carga.
