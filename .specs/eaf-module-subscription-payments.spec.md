# EAF — Subscription Payment Lifecycle

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Subscription Payment Lifecycle |
| Product / System | EAF Middleware / Templates |
| Module / Bounded Context | Payments / Subscriptions |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-subscription-lifecycle` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF already has `PaymentAppService`, `SubscriptionPayment` entity, and multiple payment gateways (Stripe, PayPal, MercadoPago, PagSeguro). However, the end-to-end subscription lifecycle is incomplete: recurring payments, proration on upgrades/downgrades, invoice numbers, trial handling, and a unified payment redirect flow are not implemented.

### Objective

Extend the existing payment system to support the full subscription lifecycle as documented in ASP.NET Zero: create payment, gateway selection, success/error redirects, recurring billing, proration, upgrade/downgrade, and tenant subscription activation.

### Expected outcome

- Enhanced `SubscriptionPayment` and new `SubscriptionPaymentProduct` entity.
- `IPaymentManager` / `PaymentManager` to orchestrate the flow.
- Recurring and proration support in Stripe gateway.
- Background job to process renewals and expirations.
- Angular `account/gateway-selection` and `admin/subscriptions` pages.

### Out of scope

- Tax calculation.
- Multi-currency pricing beyond current edition amount.
- Proprietary ASP.NET Zero UI assets.

## 2. Agent Role

Senior .NET/ABP engineer and full-stack Angular developer. Implement the subscription lifecycle on top of the existing payment system without breaking current `PaymentAppService` contracts.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not break existing payment gateway DTOs; do not remove existing payment pages; do not push NuGet packages.

## 4. Product Context

### Functional context

Tenants choose an edition and payment period, are redirected to a gateway, complete payment, and return to a success or error URL. Host admins manage subscriptions and see renewal/expiry status.

### Technical context

- `SubscriptionPayment` and `PaymentPeriodType` exist in `Eaf.Middleware.Core`.
- `PaymentAppService` creates and processes payments.
- `IPaymentGateway` has `CreatePaymentAsync` and `ProcessPaymentAsync`.
- Stripe gateway supports one-time payments; recurring needs expansion.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- Stripe.net / PayPal SDK / MercadoPago / PagSeguro
- Angular 20 / TypeScript 5.8 / PrimeNG 17
- Hangfire for background jobs

### Relevant files or directories

```text
src/Eaf.Middleware.Core/Payments/
src/Eaf.Middleware.Application/Payments/
Templates/Angular/Eaf.ProjectName.UI/src/app/admin/payments/
Templates/Angular/Eaf.ProjectName.UI/src/app/account/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/Payments/SubscriptionPayment.cs`
- `src/Eaf.Middleware.Application/Payments/PaymentAppService.cs`
- `src/Eaf.Middleware.Application/Payments/Gateways/StripePaymentGateway.cs`
- `src/Eaf.Middleware.Application/Payments/IPaymentGateway.cs`

## 5. Task Definition

### Main task

Implement the full subscription payment lifecycle on top of the existing EAF payment infrastructure.

### Subtasks

1. Extend `SubscriptionPayment` with `IsRecurring`, `IsProrationPayment`, `InvoiceNo`, `ExtraProperties`, products.
2. Create `SubscriptionPaymentProduct` entity.
3. Create `PaymentManager` (or `IPaymentManager`) with `CreatePayment`, `ProcessPayment`, `ExtendSubscription`, `UpgradeSubscription`.
4. Implement recurring Stripe support.
5. Add proration calculation for upgrades.
6. Add Hangfire job for renewal reminders and expiry checks.
7. Update Angular payment flow (`gateway-selection`, subscription management).
8. Add integration tests.

### Do not do

- Do not break `IPaymentGateway` contract.
- Do not remove `PaymentAppService` existing methods.
- Do not implement tax calculation.

## 6. Functional Requirements

### FR-001: Payment request creation

**Description:** Create a `SubscriptionPayment` with products and redirect the user to gateway selection.

**Rules:**

- `PaymentManager.CreatePayment` creates `SubscriptionPayment` and returns `PaymentRequestDto`.
- `SuccessUrl` and `ErrorUrl` are stored on the entity.
- Products (`SubscriptionPaymentProduct`) are persisted with the payment.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `EditionId` | `int` | yes | Existing subscribable edition |
| `PaymentPeriodType` | `PaymentPeriodType` | yes | Monthly/Annual/etc. |
| `Gateway` | `string` | yes | One of configured gateways |
| `IsRecurring` | `bool` | no | Stripe only at first |
| `SuccessUrl` / `ErrorUrl` | `string` | no | Valid absolute or relative URL |
| `Products` | `List<SubscriptionPaymentProductInput>` | yes | At least one product |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `PaymentId` | `long` | Created subscription payment id |
| `GatewayUrl` or `PaymentToken` | `string` | Redirect or token to continue |

**Acceptance criteria:**

- [ ] `PaymentManager.CreatePayment` creates a pending payment.
- [ ] Products total amount equals payment amount.
- [ ] Angular redirects to `account/gateway-selection?paymentId={id}`.

### FR-002: Gateway callback processing

**Description:** Process gateway callbacks and activate the tenant subscription.

**Rules:**

- `PaymentAppService.ProcessPaymentAsync` extended to handle recurring and proration.
- On success, set status `Completed`, set `PaymentTime`, calculate subscription dates, activate tenant edition.
- On failure, set status `Failed` and redirect to `ErrorUrl`.
- Generate `InvoiceNo` sequence.

**Acceptance criteria:**

- [ ] Successful payment updates `Tenant.SubscriptionEndDateUtc`.
- [ ] `InvoiceNo` generated and stored.
- [ ] User redirected to `SuccessUrl`.

### FR-003: Recurring payments (Stripe)

**Description:** Support recurring subscription charges via Stripe.

**Rules:**

- Create Stripe customer and subscription.
- Store `ExternalPaymentId` (Stripe subscription id).
- Webhook handler for `invoice.paid`/`payment_failed`.
- Background job extends `SubscriptionEndDateUtc` on successful webhook.

**Acceptance criteria:**

- [ ] Recurring Stripe payment creates a subscription.
- [ ] Webhook updates payment and tenant dates.
- [ ] Failed payment sets tenant into grace period (optional).

### FR-004: Proration and upgrade/downgrade

**Description:** When a tenant changes edition mid-cycle, calculate proration and create a proration payment.

**Rules:**

- `IsProrationPayment = true` for mid-cycle changes.
- Calculate unused days and new edition price difference.
- Create a one-time payment for the difference.

**Acceptance criteria:**

- [ ] Upgrade triggers proration payment.
- [ ] Tenant gets new edition immediately after successful proration payment.
- [ ] Downgrade effective at next billing cycle.

### FR-005: Background renewal/expiry job

**Description:** Hangfire job checks subscriptions daily and sends reminders / expires tenants.

**Rules:**

- Run once per day.
- Send notification 7, 3, 1 days before expiry.
- Disable tenant features after expiry (soft, not deletion).

**Acceptance criteria:**

- [ ] Job runs daily and logs actions.
- [ ] Reminder notifications created.
- [ ] Expired tenants see downgrade/renewal prompt.

## 7. Business Rules

### BR-001: One active subscription per tenant

A tenant can have one active subscription at a time. Upgrade/downgrade transitions must be atomic.

### BR-002: Invoice numbers are sequential

`InvoiceNo` is generated from a global sequence and is immutable.

### BR-003: Payment amount matches products

The sum of `SubscriptionPaymentProduct.TotalAmount` must equal `SubscriptionPayment.Amount`.

### BR-004: Backward compatibility

Existing `PaymentAppService.CreatePaymentAsync` and `ProcessPaymentAsync` continue to work.

## 8. Domain Modeling

### Bounded Context

Payments / Subscriptions

### Aggregates

| Aggregate | Responsibility | Invariants |
|---|---|---|
| `SubscriptionPayment` | Payment request and lifecycle | Amount equals sum of products; status transitions valid |

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `SubscriptionPayment` | `long` | Payment request, status, gateway, subscription dates |
| `SubscriptionPaymentProduct` | `long` | Line item of a payment |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `PaymentPeriodType` | enum | Valid period values |
| `SubscriptionPaymentStatus` | enum | Pending/Completed/Failed/Refunded/Cancelled |

### Domain Events

- `SubscriptionPaymentCompletedEvent`
- `SubscriptionPaymentFailedEvent`
- `SubscriptionExpiredEvent`

## 9. Expected Architecture

```text
src/Eaf.Middleware.Core/Payments/
  SubscriptionPayment.cs (extended)
  SubscriptionPaymentProduct.cs (new)
  SubscriptionPaymentStatus.cs (existing)
src/Eaf.Middleware.Application/Payments/
  PaymentManager.cs (new)
  IPaymentManager.cs (new)
  PaymentAppService.cs (extended)
  Gateways/StripePaymentGateway.cs (extended)
  Jobs/
    SubscriptionExpiryJob.cs (new)
Templates/Angular/Eaf.ProjectName.UI/
  account/gateway-selection/ (new/updated)
  admin/payments/ (updated)
  admin/subscriptions/ (new)
```

## 10. API Contracts

### Create payment

```http
POST /api/services/app/Payment/CreatePayment
```

```json
{
  "editionId": 1,
  "paymentPeriodType": "Monthly",
  "gateway": "Stripe",
  "isRecurring": true,
  "successUrl": "/payment/success",
  "errorUrl": "/payment/error",
  "products": [
    { "description": "Pro Edition Monthly", "amount": 29.99, "count": 1 }
  ]
}
```

### Process payment

```http
POST /api/services/app/Payment/ProcessPayment
```

```json
{
  "paymentId": 123,
  "gateway": "Stripe",
  "token": "pi_..."
}
```

## 11. Application Contracts

```csharp
public interface IPaymentManager
{
    Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input);
    Task<SubscriptionPaymentDto> ProcessPaymentAsync(long paymentId, ProcessPaymentInput input);
    Task<PaymentRequestDto> UpgradeSubscriptionAsync(UpgradeSubscriptionInput input);
    Task<SubscriptionPaymentDto> CancelRecurringAsync(long paymentId);
}
```

## 12. Persistence and Data

### Persisted entities

| Table | Purpose |
|---|---|
| `EafSubscriptionPayments` | Payment requests (extend) |
| `EafSubscriptionPaymentProducts` | Line items (new) |

### Migration required

Yes.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_EafSubscriptionPayments_TenantId_Status` | `TenantId`, `Status` | Renewal/expiry queries |
| `IX_EafSubscriptionPaymentProducts_PaymentId` | `SubscriptionPaymentId` | Product lookup |

### Compatibility

- [ ] Existing payment data preserved.
- [ ] New columns nullable to avoid breaking existing rows.

## 13. Integrations

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| Stripe | payment intent/subscription | confirmation | Webhook signature validation |
| PayPal | payment request | order status | Client secret / OAuth |
| MercadoPago / PagSeguro | payment request | status | Access token |

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `IEmailSender` | Send invoice/reminder | In-process | 30s | retry 3 |
| `IRealTimeNotifier` | Notify admins | SignalR | 30s | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Duplicate payment for same tenant and edition | concurrent requests | Serialize via database unique constraint or lock |
| Gateway webhook arrives before user returns | async event | Update payment status and tenant dates idempotently |
| Proration amount is negative | downgrade | Credit balance or extend subscription; do not charge negative |
| Recurring cancellation | cancel request | Mark `IsRecurring=false`, keep current subscription until end date |

## 15. Few-Shot Examples

### Example 1: Create and complete monthly subscription

```csharp
var request = await _paymentManager.CreatePaymentAsync(input);
// redirect user to gateway-selection page
var payment = await _paymentManager.ProcessPaymentAsync(request.PaymentId, token);
```

### Example 2: Upgrade with proration

```csharp
var request = await _paymentManager.UpgradeSubscriptionAsync(new UpgradeSubscriptionInput
{
    TenantId = 1,
    NewEditionId = 2
});
```

## 16. Non-Functional Requirements

### Performance

- Payment create endpoint P95 < 200 ms.
- Background job processes 10k tenants under 5 minutes.

### Security

- Validate webhook signatures.
- Do not log gateway tokens.
- Idempotency keys for gateway calls.

### Observability

- Logs for payment status changes.
- OpenTelemetry spans for gateway calls.

### Maintainability

- README.md for webhook setup.
- Clear status transition rules.

## 17. Mandatory Guardrails

- Do not change `IPaymentGateway` without a new major version note.
- Do not expose gateway secrets.
- Do not delete existing payment data.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `PaymentManager` | Create, process, proration, upgrade |
| `SubscriptionPayment` | Status transitions, amount/product invariants |

### Integration tests

| Flow | Validation |
|---|---|
| Stripe one-time payment | Full redirect/callback simulation |
| Stripe recurring webhook | Subscription extension |
| Upgrade with proration | Amount and tenant edition updated |

## 19. Acceptance Criteria

- [ ] `SubscriptionPayment` extended and `SubscriptionPaymentProduct` created.
- [ ] `PaymentManager` orchestrates create/process/upgrade/cancel.
- [ ] Stripe recurring and webhooks work.
- [ ] Angular `gateway-selection` and subscription pages present.
- [ ] Background expiry job runs and notifies.
- [ ] Existing tests pass.

## 20. Implementation Plan

1. Extend domain model and run migration.
2. Implement `PaymentManager`.
3. Extend Stripe gateway for recurring.
4. Add proration and upgrade logic.
5. Add Hangfire expiry job.
6. Build Angular pages.
7. Add tests.
8. Update documentation and index.

## 21. Rollback Strategy

- Revert migration if not deployed.
- Disable recurring feature via setting.
- Keep existing payment endpoints as fallback.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Webhook replay / forgery | High | Medium | Validate signatures and idempotency |
| Proration math errors | High | Low | Unit tests with known date/amount scenarios |
| Gateway API changes | Medium | Medium | Pin SDK versions in `common.props` |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Backend and Angular implemented.
- [ ] Tests pass.
- [ ] Webhooks and background job validated.
- [ ] Index updated.

## 24. Key Reminder

> Build on top of the existing `PaymentAppService` and gateways. Do not break existing payment contracts; extend them.
