# EAF — Eaf.Webhooks Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.Webhooks Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Integrations / Webhooks |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-webhooks` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF does not provide a first-class outgoing webhook subsystem. Applications that need to notify external systems of domain events must implement ad-hoc HTTP clients, losing retries, signatures, and delivery tracking.

### Objective

Create `Eaf.Webhooks` middleware module that allows subscription to webhook events, delivery via HTTP, retries with backoff, and HMAC signatures.

### Expected outcome

- New `src/Eaf.Webhooks/` project.
- `WebhookSubscription`, `WebhookPayload`, `IWebhookPublisher`, `IWebhookSubscriptionManager`.
- `WebhookSender` background job.
- `WebhookSubscriptionAppService` and Angular admin UI spec reference.
- Tests for payload signing, retry, and subscription CRUD.

### Out of scope

- Incoming webhooks (receive external systems).
- Webhook marketplace or public subscription portal.

## 2. Agent Role

Senior .NET/ABP engineer. Implement outgoing webhook infrastructure with delivery guarantees and security.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not modify `IEventBus`; do not push directly to remote.

## 4. Product Context

### Functional context

Subscribe external URLs to domain events (e.g., `User.Created`, `Payment.Succeeded`) and deliver signed JSON payloads.

### Technical context

- ABP `EventBus` / `IEventData`.
- `HttpClient` + `IHttpClientFactory`.
- Hangfire or Quartz for background delivery.
- HMAC-SHA256 signatures.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- EF Core 10
- Hangfire / Quartz
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Webhooks/
src/Eaf.Middleware.Core/
src/Eaf.Middleware.Application/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/Webhooks/` (if any)
- `src/Eaf.Middleware.Application/Webhooks/` (if any)
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.Webhooks` module and tests.

### Subtasks

1. Create `src/Eaf.Webhooks/` project.
2. Define `WebhookSubscription` and `WebhookPayload` entities.
3. Implement `IWebhookSubscriptionManager` for CRUD.
4. Implement `IWebhookPublisher` that enqueues payloads for events.
5. Implement `WebhookSender` background job with retries and HMAC.
6. Create `EafWebhooksModule`.
7. Add `Eaf.Webhooks.Tests`.
8. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not modify `IEventBus`.
- Do not implement incoming webhooks.
- Do not add Angular UI in this backend module.

## 6. Functional Requirements

### FR-001: Webhook subscription CRUD

**Description:** Manage subscriptions with URL, event names, secret, and active flag.

**Rules:**

- Subscription is tenant-scoped.
- URL must be absolute HTTPS unless explicitly allowed otherwise.
- Secret is encrypted at rest using `Eaf.KeyVault` or ABP `SettingManager`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `webhookSubscriptionId` | `Guid` | no | Generated on create |
| `tenantId` | `int?` | no | Multi-tenant scope |
| `url` | `string` | yes | Absolute URI |
| `eventNames` | `List<string>` | yes | At least one event |
| `secret` | `string` | no | Used for HMAC signature |
| `isActive` | `bool` | yes | Default true |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `subscription` | `WebhookSubscription` | Persisted subscription |

**Acceptance criteria:**

- [ ] Create, update, delete, list subscriptions.
- [ ] Duplicate URL+event combinations per tenant are prevented.

### FR-002: Webhook payload signing and delivery

**Description:** Publish events, queue HTTP deliveries, sign payloads, retry on failure.

**Rules:**

- Payload JSON includes `eventName`, `timestamp`, `payload`.
- HMAC-SHA256 signature in `X-Eaf-Signature-256` header.
- Retry up to N times with exponential backoff.
- Store delivery attempt status and response.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `eventName` | `string` | yes | Registered event name |
| `payload` | `object` | yes | Serializable event data |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `deliveryAttempts` | `List<WebhookDelivery>` | Attempt records |

**Acceptance criteria:**

- [ ] Payload is signed with secret.
- [ ] Delivery attempts are persisted.
- [ ] Retries respect configured count and backoff.
- [ ] Inactive subscriptions are skipped.

## 7. Business Rules

### BR-001: HTTPS by default

Webhook URLs must use `https://` unless `EafWebhooksOptions.AllowHttp` is explicitly enabled.

### BR-002: Tenant isolation

Subscriptions are isolated by tenant. Host admin can manage all tenants' subscriptions.

### BR-003: Secret encryption

Webhook secrets are encrypted at rest using `Eaf.KeyVault` or ABP `SettingManager`.

## 8. Domain Modeling

### Bounded Context

Integrations / Webhooks

### Aggregates

| Aggregate | Responsibility | Invariants |
|---|---|---|
| `WebhookSubscription` | Stores endpoint, events, secret, active flag | Unique per tenant + URL + event |

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `WebhookSubscription` | `Guid` | Subscription aggregate root |
| `WebhookDelivery` | `Guid` | Delivery attempt record |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `WebhookPayload` | `EventName`, `Timestamp`, `PayloadJson` | Non-null event and payload |

### Domain Events

| Event | When it occurs | Payload |
|---|---|---|
| `WebhookDelivered` | Delivery attempt completed | `WebhookDeliveryId`, `Success` |

## 9. Expected Architecture

### Architectural style

ABP modular monolith.

### Layers

```text
src/Eaf.Webhooks/
  Domain/
    WebhookSubscription.cs
    WebhookDelivery.cs
    WebhookPayload.cs
    IWebhookSubscriptionRepository.cs
  Application/
    IWebhookSubscriptionManager.cs
    WebhookSubscriptionManager.cs
    IWebhookPublisher.cs
    WebhookPublisher.cs
    WebhookSender.cs
    WebhookSubscriptionAppService.cs
    Dto/
  EntityFrameworkCore/
    WebhooksDbContext.cs
    EfCoreWebhookSubscriptionRepository.cs
  Web/
    WebhooksWebModule.cs
  EafWebhooksModule.cs
  README.md
  Eaf.Webhooks.csproj
test/Eaf.Webhooks.Tests/
  WebhookSubscriptionManager_Tests.cs
  WebhookSender_Tests.cs
```

### Allowed dependencies

- `Abp`
- `Eaf.KeyVault` (optional for encryption)
- `Eaf.Middleware.Core` (for tenant/user context)

### Forbidden dependencies

- UI frameworks.
- Specific cloud providers.

## 10. API Contracts

### Subscription CRUD

```http
[POST]   /api/services/app/WebhookSubscription/Create
[PUT]    /api/services/app/WebhookSubscription/Update
[DELETE] /api/services/app/WebhookSubscription/Delete
[GET]    /api/services/app/WebhookSubscription/GetAll
```

### Request

```json
{
  "url": "https://example.com/webhook",
  "eventNames": ["User.Created"],
  "secret": "whsec_...",
  "isActive": true
}
```

### Success response

```json
{
  "id": "00000000-0000-0000-0000-000000000000"
}
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public class CreateWebhookSubscriptionInput
{
    public string Url { get; set; }
    public List<string> EventNames { get; set; }
    public string Secret { get; set; }
    public bool IsActive { get; set; } = true;
}

public class WebhookSubscriptionDto : EntityDto<Guid>
{
    public string Url { get; set; }
    public List<string> EventNames { get; set; }
    public bool IsActive { get; set; }
}
```

## 12. Persistence and Data

### Persisted entities

| Table | Purpose |
|---|---|
| `WebhookSubscriptions` | Store subscriptions |
| `WebhookDeliveries` | Store delivery attempts |

### Migration required

Yes.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_WebhookSubscriptions_TenantId` | `TenantId` | Tenant-scoped lookups |
| `IX_WebhookDeliveries_SubscriptionId_Created` | `SubscriptionId`, `CreationTime` | Delivery history queries |

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `IEventBus` | Listen for domain events | In-process | — | no |
| HTTP endpoint | Deliver webhooks | HTTPS | 30s | exponential backoff |
| `Eaf.KeyVault` | Encrypt secrets | In-process | — | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| HTTP 4xx | delivery POST | Mark failed; do not retry unless configured |
| HTTP 5xx / timeout | delivery POST | Retry with backoff; mark exhausted after max |
| Invalid URL | `not-a-url` | Return 400 validation error |
| Duplicate subscription | same tenant + URL + event | Return 409 conflict |

## 15. Few-Shot Examples

### Example 1: Happy path

```csharp
await _publisher.PublishAsync("User.Created", new UserCreatedEvent { Id = 1 });
// WebhookSubscription matching event is queued and delivered to HTTPS URL.
```

## 16. Non-Functional Requirements

### Performance

- Webhook delivery < 5 s per attempt.
- Queue latency < 1 s for enqueued events.

### Security

- HMAC-SHA256 signatures.
- Secrets encrypted at rest.
- HTTPS by default; allow HTTP only via explicit flag.

### Observability

- Structured logs for delivery attempts and failures.
- OpenTelemetry HTTP client instrumentation.

## 17. Mandatory Guardrails

- Do not modify `IEventBus`.
- Do not expose secrets in logs.
- Do not allow HTTP by default.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `WebhookSubscriptionManager` | CRUD, duplicate detection |
| `WebhookSender` | HMAC signing, retry, status mapping |

### Integration tests

| Flow | Validation |
|---|---|
| Publish event | Matching subscription receives HTTP POST |
| Failed delivery | Retry attempts persisted |

### xUnit example

```csharp
public class WebhookSender_Tests : AbpIntegratedTestBase<EafWebhooksModule>
{
    [Fact]
    public void Dado_PayloadESecret_Quando_Assinar_Entao_GeraHmac256()
    {
        var payload = "{\"eventName\":\"User.Created\"}";
        var secret = "secret";
        var signature = WebhookSender.SignPayload(payload, secret);
        signature.ShouldNotBeNullOrEmpty();
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.Webhooks` compiles and packs as NuGet.
- [ ] Subscription CRUD app service works in integration tests.
- [ ] Webhooks are delivered and signed.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — inspect ABP event bus and existing `Webhook` code.
2. Design — define entities, app service, background sender.
3. Project setup — create `src/Eaf.Webhooks/` and `test/Eaf.Webhooks.Tests/`.
4. Implementation — domain, application, EF, web module, tests.
5. Tests — unit and integration.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafWebhooksModule))]`.
- Drop migration tables if not used.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Delivery retry storm | High | Medium | Configurable max retries, circuit breaker |
| Secret exposure | High | Low | Encrypt at rest, never log |
| Event payload serialization mismatch | Medium | Medium | Use ABP `IJsonSerializer` with type info |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> Outgoing webhooks only. Keep events generic and do not expose sensitive data in payloads.
