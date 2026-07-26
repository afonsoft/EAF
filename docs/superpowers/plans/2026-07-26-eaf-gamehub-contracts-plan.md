# EAF GameHub Contracts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Registrar no EAF os contratos, limites operacionais e exemplos de template necessários para que consumidores como o GameHub reutilizem chat, notificações, social, rate limit, auditoria, observabilidade e SignalR sem duplicar persistência.

**Architecture:** A primeira entrega é retrocompatível e orientada a contrato: a documentação define ownership, versionamento, tenant isolation, autorização, fallback e rollout; os templates recebem exemplos opt-in e idempotentes, sem editar proxies gerados nem registrar um segundo provider. APIs de domínio existentes permanecem a fonte de verdade, enquanto novos contratos opcionais são introduzidos por extensão/versionamento.

**Tech Stack:** Markdown, ASP.NET Boilerplate, ASP.NET Core, EF Core, SignalR, Redis, OpenTelemetry, Serilog, Angular 20, TypeScript, Jasmine/Karma.

---

### Task 1: Mapear contratos e riscos de compatibilidade

**Files:**
- Create: `docs/superpowers/plans/2026-07-26-eaf-gamehub-contracts-plan.md`
- Inspect: `src/Eaf.Middleware.Core`, `src/Eaf.Middleware.Application`, `src/Eaf.Middleware.Web.Core`
- Inspect: `Templates/Api`, `Templates/Angular/Eaf.ProjectName.UI`

- [ ] **Step 1: Confirmar os pontos de extensão existentes**

  Verificar `ChatMessage`, `ChatHub`, `IChatCommunicator`, `INotificationPublisher`, `FriendshipAppService`, cache, health checks, `AddEafConfigurer`, `AddEafHealthChecks`, `AddEafOpenTelemetry` e os proxies Angular.

- [ ] **Step 2: Separar comportamento já disponível de evolução necessária**

  Registrar, por contrato, o que é reutilizado hoje, o que precisa de uma interface opcional e o que não pode ser implementado no consumidor.

### Task 2: Publicar guia de contratos consumidores e migration guide

**Files:**
- Create: `docs/integration/gamehub-consumer-contracts.md`
- Modify: `docs/modules/README.md`
- Modify: `docs/templates/README.md`

- [ ] **Step 1: Documentar chat contextual**

  Definir `ConversationId`, `GameId`, `MatchId`, `ContextType`, histórico paginado, `MarkRead`, idempotência, índices, autorização e preservação de `/signalr-chat`.

- [ ] **Step 2: Documentar notificações, social, rate limit e auditoria**

  Definir payload versionado, metadata, severidade, expiração, block/mute, precedência, decisão operacional de rate limit e registro de moderação sem PII.

- [ ] **Step 3: Documentar erros, observabilidade e SignalR**

  Definir códigos públicos, correlation ID, métricas permitidas, ChannelPrefix, health checks, Data Protection Keys, graceful shutdown e procedimento de duas instâncias.

- [ ] **Step 4: Documentar versionamento e rollout**

  Incluir capabilities/feature flags, compatibilidade de clientes antigos, migrações, rollback, retenção/anonimização e critérios de aceite.

### Task 3: Atualizar o template API com exemplos opt-in

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/appsettings.json`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/appsettings.Development.json`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/appsettings.Staging.json`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/appsettings.Production.json`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/appsettings.Local.json`
- Create: `Templates/Api/src/Eaf.ProjectName.Core/Application/Contracts/ContextualChatMessageContract.cs`
- Create: `Templates/Api/src/Eaf.ProjectName.Core/Application/Contracts/RateLimitContract.cs`
- Create: `Templates/Api/src/Eaf.ProjectName.Core/Application/Contracts/ModerationAuditContract.cs`
- Create: `Templates/Api/src/Eaf.ProjectName.Core/Application/Contracts/PublicErrorContract.cs`
- Modify: `Templates/Api/README.md`
- Modify: `Templates/Api/docs/GETTING_STARTED.md`

- [ ] **Step 1: Adicionar contratos de exemplo sem persistência**

  Criar somente DTOs/interfaces opt-in com `CancellationToken`, sem substituir os serviços EAF existentes ou criar entidades paralelas.

- [ ] **Step 2: Adicionar configuração documentada de realtime e observabilidade**

  Expor opções de backplane/ChannelPrefix, Data Protection compartilhado e health checks sem impedir startup quando Redis estiver desabilitado.

- [ ] **Step 3: Atualizar o guia do template**

  Explicar cache versus Hangfire versus Pub/Sub, CORS por ambiente, secrets, logs, migrations, autorização, tenant isolation e os exemplos dos contratos.

### Task 4: Atualizar o template Angular sem editar proxies

**Files:**
- Create: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/eaf-contracts/eaf-contracts.ts`
- Create: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/eaf-contracts/eaf-error.interceptor.ts`
- Create: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/eaf-contracts/eaf-contracts.spec.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/docs/IMPLEMENTATIONS.md`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/README.md`

- [ ] **Step 1: Adicionar interfaces públicas mínimas**

  Definir `EafError` e `ContextualChatMessage` com campos opcionais e sem `any`.

- [ ] **Step 2: Adicionar interceptor opt-in para correlation ID e erros**

  Preservar `401/403`, normalizar somente o envelope público e limitar retry a falhas transitórias, sem interceptar proxies gerados de forma invasiva.

- [ ] **Step 3: Adicionar testes Jasmine**

  Cobrir sucesso, erro público, correlation ID, retry transitório, não-retry de validação e limpeza de requisição.

### Task 5: Validar templates e qualidade

**Files:**
- Inspect all modified files

- [ ] **Step 1: Validar Markdown e JSON**

  Executar validadores disponíveis e garantir que nenhum segredo ou proxy gerado foi alterado.

- [ ] **Step 2: Executar build/test/lint do template API**

  Rodar restore/build/test do solution do template, corrigindo apenas problemas introduzidos pela mudança.

- [ ] **Step 3: Executar build/test/lint Angular**

  Usar Node 20 e os comandos do blueprint; confirmar warnings de budget e browser headless.

- [ ] **Step 4: Revisar requisitos e diff**

  Conferir tenant isolation, compatibilidade, ausência de persistência paralela, health checks, observabilidade, migration guide e documentação EAF versus consumidor.
