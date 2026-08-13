# EAF — Eaf.SignalR Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.SignalR Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Real-time / Notifications / Chat |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-signalr` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF has real-time features (chat, notifications) wired directly in `Eaf.Middleware.Web.Core`, but there is no dedicated `Eaf.SignalR` module. This makes it hard to add a Redis backplane, reuse hubs across templates, and centralize online client management.

### Objective

Create `Eaf.SignalR` as a reusable ABP SignalR module that provides hub base classes, `IRealTimeNotifier` integration, online client tracking, and optional Redis backplane support.

### Expected outcome

- `src/Eaf.SignalR/` project.
- `EafSignalRModule`, `EafHubBase`, `RealTimeNotifier`.
- `IOnlineClientManager` implementation.
- Unit/integration tests and README.md.

### Out of scope

- End-to-end chat UI redesign (covered by `eaf-angular-remaining-modernization-features.spec.md`).
- Video/voice streaming.

## 2. Agent Role

Senior .NET/ABP engineer. Reuse `Abp.AspNetCore.SignalR` and EAF notification/chat services.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not change existing notification contracts; do not break chat consumers.

## 4. Product Context

### Functional context

Notifications, mass notifications, and chat use real-time delivery. A centralized SignalR module allows scaling out with a Redis backplane and consistent hub patterns.

### Technical context

- `Eaf.Middleware.Web.Core` currently registers SignalR.
- `NotificationAppService` and `ChatAppService` push messages.
- `Eaf.RedisCache` can share Redis connection for backplane.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- SignalR (ASP.NET Core)
- `Microsoft.AspNetCore.SignalR.StackExchangeRedis` (optional backplane)
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Middleware.Web.Core/
src/Eaf.Middleware.Core/Notifications
src/Eaf.Middleware.Core/Chat
Templates/Api/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Web.Core/EafMiddlewareWebCoreModule.cs`
- `src/Eaf.Middleware.Core/Notifications/NotificationAppService.cs`
- `src/Eaf.Middleware.Core/Chat/ChatAppService.cs`

## 5. Task Definition

### Main task

Create `Eaf.SignalR` module and migrate real-time infrastructure from `Eaf.Middleware.Web.Core`.

### Subtasks

1. Create `src/Eaf.SignalR/` project.
2. Implement `EafSignalRModule`.
3. Implement `EafHubBase` and `EafHubBase<TClient>`.
4. Implement `EafRealTimeNotifier` implementing `IRealTimeNotifier`.
5. Implement `EafOnlineClientManager`.
6. Add Redis backplane configuration.
7. Add unit/integration tests.
8. Add README.md.
9. Refactor `Eaf.Middleware.Web.Core` to depend on `Eaf.SignalR`.

### Do not do

- Do not change notification DTO contracts.
- Do not remove chat features.
- Do not add UI code.

## 6. Functional Requirements

### FR-001: Centralized SignalR hubs

**Description:** Provide base hub classes for EAF real-time features.

**Rules:**

- `EafHubBase` extends `AbpHubBase`.
- Injects `IAbpSession` and `IOnlineClientManager`.
- Handles connection/disconnection events.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `userId` / `tenantId` | `long?` / `int?` | from session | Injected from `IAbpSession` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `Clients` / `Groups` | SignalR primitives | Standard SignalR API |

**Acceptance criteria:**

- [ ] New hubs can inherit `EafHubBase`.
- [ ] Connection events update online client manager.

### FR-002: Real-time notifier

**Description:** Implement `IRealTimeNotifier` to deliver ABP notifications via SignalR.

**Rules:**

- Replaces or supplements existing notifier.
- Sends to user, tenant, or all online clients based on notification severity.
- Falls back to silent if no connections.

**Acceptance criteria:**

- [ ] `NotificationAppService` can use `EafRealTimeNotifier`.
- [ ] Notifications are received in Angular app via SignalR.

### FR-003: Online client manager

**Description:** Track online users and allow querying.

**Rules:**

- Add/remove clients on connect/disconnect.
- Expose `GetOnlineClients`, `GetByUserId`, `IsOnline`.
- Thread-safe in-memory store; optionally backed by Redis for scale-out.

**Acceptance criteria:**

- [ ] `IOnlineClientManager` resolves to `EafOnlineClientManager`.
- [ ] Online clients are tracked per user/tenant.

### FR-004: Redis backplane

**Description:** Support Redis backplane for multi-instance SignalR.

**Rules:**

- Enabled via `EafSignalROptions:UseRedisBackplane`.
- Uses `Eaf.RedisCache` connection if available.

**Acceptance criteria:**

- [ ] Messages reach clients connected to a different server instance.

## 7. Business Rules

### BR-001: Backward compatibility

Existing chat and notification endpoints must continue to work without client-side changes.

### BR-002: Multi-tenancy

Online client tracking must respect tenant isolation.

### BR-003: Authentication

Hubs require JWT authentication; anonymous connections are ignored for user-specific messages.

## 8. Domain Modeling

### Bounded Context

Real-time / Notifications

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `OnlineClient` | `ConnectionId` | Represents a connected SignalR client |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `OnlineClient` | `ConnectionId`, `UserId`, `TenantId`, `ClientIpAddress` | Non-empty `ConnectionId` |

### Domain Events

- `ClientConnectedEvent`
- `ClientDisconnectedEvent`

## 9. Expected Architecture

```text
src/Eaf.SignalR/
  EafSignalRModule.cs
  HubCommon/
    EafHubBase.cs
    EafHubBase_TClient.cs
  Notifiers/
    EafRealTimeNotifier.cs
  OnlineClients/
    IOnlineClientManager.cs
    EafOnlineClientManager.cs
    OnlineClient.cs
  Configuration/
    EafSignalROptions.cs
test/Eaf.SignalR.Tests/
```

## 10. API Contracts

No new HTTP endpoints. Hubs expose SignalR methods:

```csharp
public interface IEafClient
{
    Task ReceiveNotification(UserNotification notification);
    Task ReceiveMessage(ChatMessage message);
}
```

## 11. Application Contracts

```csharp
public interface IOnlineClientManager
{
    void Add(IOnlineClient client);
    void Remove(string connectionId);
    IOnlineClient GetByConnectionId(string connectionId);
    List<IOnlineClient> GetOnlineClients();
    List<IOnlineClient> GetAllByUserId(long userId);
    bool IsOnline(long userId);
}
```

## 12. Persistence and Data

### Persisted entities

N/A — online clients are in-memory or Redis-backed.

### Migration required

No.

### Compatibility

- [ ] No database migration.
- [ ] Existing notification tables unchanged.

## 13. Integrations

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `IRealTimeNotifier` | Deliver notifications | SignalR | 30s | no |
| `IOnlineClientManager` | Track clients | In-memory / Redis | — | no |

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| Redis (backplane) | SignalR messages | SignalR messages | TLS optional |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| No SignalR connections | offline user | Notification stored in DB; delivery attempted on next connect |
| Multi-instance without backplane | scaled-out deployment | Messages only reach local clients; backplane config recommended |
| Anonymous connection | no token | Allow connection but not user-specific delivery |
| Hub exception | client sends invalid payload | Log and disconnect gracefully |

## 15. Few-Shot Examples

### Example 1: Send notification to online user

```csharp
await _realTimeNotifier.SendAsync(
    new UserNotification
    {
        UserId = 1,
        Text = "Welcome",
        Severity = NotificationSeverity.Info
    }
);
```

### Example 2: Query online clients

```csharp
var clients = _onlineClientManager.GetAllByUserId(1);
```

## 16. Non-Functional Requirements

### Performance

- Online client operations O(1) on average.
- SignalR message delivery P95 < 100 ms local.

### Security

- Authenticate hubs.
- Do not expose connection IDs to unauthorized users.
- Sanitize payloads.

### Observability

- Log connects/disconnects.
- OpenTelemetry spans for notification delivery.

### Maintainability

- README.md with hub setup and backplane config.

## 17. Mandatory Guardrails

- Do not break existing chat/notification contracts.
- Do not expose connection IDs.
- Do not bypass authentication.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafRealTimeNotifier` | Send to user/tenant/all |
| `EafOnlineClientManager` | Add/remove/lookup |
| `EafHubBase` | Connect/disconnect events |

### Integration tests

| Flow | Validation |
|---|---|
| Notification delivery via SignalR | Client receives notification |
| Redis backplane | Two server instances deliver cross-instance |

## 19. Acceptance Criteria

- [ ] `Eaf.SignalR` compiles and packs.
- [ ] Existing real-time features continue to work.
- [ ] Redis backplane integration tested.
- [ ] README.md updated.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Create `src/Eaf.SignalR/` and tests.
2. Implement hubs, notifier, online client manager.
3. Add Redis backplane configuration.
4. Refactor `Eaf.Middleware.Web.Core` to use `Eaf.SignalR`.
5. Add tests.
6. Add README.md and update index.
7. Build and test.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafSignalRModule))]` and keep SignalR directly in Web.Core.
- Disable Redis backplane.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Breaking chat/notifications | High | Low | Preserve contracts and add integration tests |
| Redis backplane complexity | Medium | Low | Make it opt-in |
| Scale-out without backplane | Medium | Medium | Document requirement |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] `Eaf.Middleware.Web.Core` refactored safely.
- [ ] README.md and index updated.

## 24. Key Reminder

> Centralize but do not break existing real-time features. Preserve contracts and authentication.
