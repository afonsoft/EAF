# EAF — SMS and Push Notifications Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | SMS and Push Notifications Module |
| Product / System | EAF Middleware / Angular UI |
| Module / Bounded Context | Notifications / Real-time |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-sms-push` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF has email (`Abp.MailKit`) and in-app/SignalR notifications, but it lacks a reusable SMS sending abstraction and Web Push notification support for PWA/mobile scenarios. ASP.NET Zero supports SMS and push notifications as part of the notification system.

### Objective

Create `Eaf.Notifications.Sms` and `Eaf.Notifications.Push` modules (or a combined `Eaf.Notifications.Channels` module) that provide provider-agnostic SMS and Web Push APIs, plus Angular service-worker integration.

### Expected outcome

- `ISmsSender` / `ISmsProvider` abstraction and default Twilio/Amazon SNS providers.
- `IPushNotificationSender` / `IPushNotificationProvider` abstraction and Web Push provider using VAPID.
- Settings management for SMS and push providers.
- Angular service to request push permission and store subscription tokens.
- Backend notification publisher can target SMS/Push channels.

### Out of scope

- iOS/Android native push (use Web Push for PWA).
- Voice calls.

## 2. Agent Role

Senior .NET + Angular engineer. Build provider abstractions; concrete providers can be optional packages.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not break existing `IRealTimeNotifier`; do not require native mobile SDKs.

## 4. Product Context

### Functional context

SMS is used for 2FA and alerts. Web Push is used for real-time notifications on PWA/offline clients.

### Technical context

- `Eaf.Middleware.Core` has notifications.
- Angular PWA has `@angular/service-worker` and `manifest.json`.
- `Eaf.MailKit` spec provides pattern for settings and providers.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- Twilio SDK or Amazon SDK (optional providers)
- Web Push client (`PushSubscription`, VAPID)
- Angular 20 / TypeScript 5.8

### Relevant files or directories

```text
src/Eaf.Middleware.Core/Notifications/
Templates/Angular/Eaf.ProjectName.UI/ngsw-config.json
Templates/Angular/Eaf.ProjectName.UI/src/manifest.json
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/Notifications/`
- `.specs/eaf-angular-pwa-offline.spec.md`

## 5. Task Definition

### Main task

Create SMS and Web Push notification abstractions and optional providers.

### Subtasks

1. Define `ISmsSender`, `ISmsProvider`, `SmsMessage`.
2. Implement `TwilioSmsProvider` and `SnsSmsProvider` (optional).
3. Define `IPushNotificationSender`, `IPushNotificationProvider`, `PushNotificationMessage`.
4. Implement `WebPushNotificationProvider` using VAPID.
5. Add settings providers for SMS and Web Push.
6. Integrate with `IRealTimeNotifier` to target SMS/Push channels.
7. Add Angular `PushNotificationService` and UI permission flow.
8. Add tests and README.

### Do not do

- Do not break existing notification delivery.
- Do not require native mobile SDKs.
- Do not store VAPID private keys in source.

## 6. Functional Requirements

### FR-001: SMS sending

**Description:** Send SMS via configured provider.

**Rules:**

- `ISmsSender.SendAsync(SmsMessage message)`.
- `SmsMessage` has `PhoneNumber`, `Body`.
- Provider selected by settings.

**Acceptance criteria:**

- [ ] SMS can be sent via Twilio (or SNS) provider.
- [ ] Invalid phone number returns validation error.

### FR-002: Web Push sending

**Description:** Send Web Push notifications to subscribed browsers.

**Rules:**

- `IPushNotificationSender.SendAsync(PushNotificationMessage message, List<PushSubscription> subscriptions)`.
- Use VAPID keys for signing.
- Payload is JSON with title, message, icon, url.

**Acceptance criteria:**

- [ ] Subscribed browsers receive push notification.
- [ ] Failed subscriptions are removed.

### FR-003: User subscription storage

**Description:** Store and manage browser push subscriptions per user/tenant.

**Rules:**

- Table `EafPushSubscriptions` with `UserId`, `TenantId`, `Endpoint`, `P256dh`, `Auth`.
- Endpoint exposed to register/unregister.

**Acceptance criteria:**

- [ ] Angular can register subscription.
- [ ] Backend can query subscriptions by user/tenant.

### FR-004: Notification channel integration

**Description:** `IRealTimeNotifier` can route notifications to SMS or Push channels based on user settings.

**Rules:**

- `NotificationAppService` checks `NotificationSetting` for SMS/Push enabled.
- Target channels if enabled.

**Acceptance criteria:**

- [ ] User receives SMS when SMS channel is enabled.
- [ ] User receives Web Push when browser subscribed.

## 7. Business Rules

### BR-001: Opt-in

SMS and Push require user consent and settings enabled.

### BR-002: No credential exposure

Provider keys and VAPID private keys stored in settings/KeyVault; never logged.

### BR-003: Tenant isolation

Subscriptions and settings isolated by tenant.

## 8. Domain Modeling

### Bounded Context

Notifications

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `PushSubscription` | `long` or `Guid` | Stores browser subscription per user |
| `SmsMessage` | value object | SMS payload |
| `PushNotificationMessage` | value object | Push payload |

## 9. Expected Architecture

```text
src/Eaf.Notifications.Sms/
  EafNotificationsSmsModule.cs
  ISmsSender.cs
  ISmsProvider.cs
  SmsMessage.cs
  Providers/
    TwilioSmsProvider.cs
    SnsSmsProvider.cs
src/Eaf.Notifications.Push/
  EafNotificationsPushModule.cs
  IPushNotificationSender.cs
  IPushNotificationProvider.cs
  PushNotificationMessage.cs
  PushSubscription.cs
  Providers/
    WebPushNotificationProvider.cs
test/
```

## 10. API Contracts

### Register push subscription

```http
POST /api/services/app/PushNotification/RegisterSubscription
```

```json
{
  "endpoint": "https://fcm...",
  "p256dh": "...",
  "auth": "..."
}
```

### Send SMS (admin only)

```http
POST /api/services/app/SmsSender/Send
```

```json
{
  "phoneNumber": "+15551234567",
  "body": "Your code is 123456"
}
```

## 11. Application Contracts

```csharp
public interface ISmsSender
{
    Task SendAsync(SmsMessage message);
}

public interface IPushNotificationSender
{
    Task SendAsync(PushNotificationMessage message, List<PushSubscription> subscriptions);
}
```

## 12. Persistence and Data

### Persisted entities

| Table | Purpose |
|---|---|
| `EafPushSubscriptions` | Browser push subscriptions per user |

### Migration required

Yes — `EafPushSubscriptions`.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_EafPushSubscriptions_UserId_TenantId` | `UserId`, `TenantId` | Fast lookup |

## 13. Integrations

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| Twilio | phone number, body | message id | API key via settings |
| Amazon SNS | phone number, body | message id | AWS credentials via settings |
| Web Push endpoint (FCM/APNs/Edge) | encrypted payload | status | VAPID keys |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Invalid phone number | malformed | Validation error |
| Push subscription expired | 410 response | Remove subscription from DB |
| Provider unavailable | timeout | Log and retry once for SMS |

## 15. Few-Shot Examples

### Example 1: Send SMS

```csharp
await _smsSender.SendAsync(new SmsMessage
{
    PhoneNumber = "+15551234567",
    Body = "Your verification code is 123456"
});
```

### Example 2: Web Push

```csharp
await _pushSender.SendAsync(
    new PushNotificationMessage { Title = "Alert", Message = "Server restarted" },
    subscriptions
);
```

## 16. Non-Functional Requirements

### Performance

- SMS send < 2 s.
- Push batch send < 1 s per 100 subscriptions.

### Security

- No keys in logs.
- Validate phone numbers.
- Encrypt VAPID private key at rest.

### Observability

- Logs for send attempts, failures, expired subscriptions.

## 17. Mandatory Guardrails

- Do not store keys in source.
- Do not send SMS without user consent.
- Do not break existing notifications.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `WebPushNotificationProvider` | Payload encryption, expired subscription |
| `TwilioSmsProvider` | Send, invalid number |

### Integration tests

| Flow | Validation |
|---|---|
| Register subscription | Stored in DB |
| Send push | Subscribed client receives (mock) |

## 19. Acceptance Criteria

- [ ] SMS and Web Push abstractions implemented.
- [ ] At least one concrete provider each works.
- [ ] Angular registers push subscriptions.
- [ ] `IRealTimeNotifier` can route to channels.
- [ ] Tests pass.

## 20. Implementation Plan

1. Create SMS module and Twilio provider.
2. Create Push module and Web Push provider.
3. Add `EafPushSubscriptions` migration.
4. Integrate with notification system.
5. Add Angular service and permission UI.
6. Add tests and README.
7. Update index.

## 21. Rollback Strategy

- Disable modules via `[DependsOn]` removal.
- Delete `EafPushSubscriptions` table if not used.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Provider keys exposed | High | Low | Use settings/KeyVault |
| Push subscriptions privacy | Medium | Medium | Encrypt `auth`/`p256dh` at rest |
| Twilio SDK breaking changes | Medium | Low | Pin version |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Modules and Angular service implemented.
- [ ] Tests pass.
- [ ] Index updated.

## 24. Key Reminder

> Abstractions first. Concrete providers are optional but must be secure. Web Push is the primary push target; native mobile is out of scope.
