# Eaf.Webhooks

Módulo EAF para envio de webhooks HTTP. Reutiliza `Abp.Webhooks` e `Abp.AspNetCore.Webhook` e aplica assinatura HMAC-SHA256, guarda HTTPS, criptografia do segredo e deduplicação de assinaturas.

## Funcionalidades

- Assinatura HMAC no header `X-Eaf-Signature-256`.
- Payload no formato `{ eventName, timestamp, payload }`.
- URLs HTTP bloqueadas por padrão (`EafWebhooksOptions.AllowHttp`).
- Segredo criptografado em repouso via ASP.NET Core Data Protection.
- Reutiliza persistence, background job e publisher do ABP.

## Configuração

```json
{
  "EafWebhooks": {
    "AllowHttp": false,
    "TimeoutSeconds": 30,
    "MaxSendAttemptCount": 5,
    "IsAutomaticSubscriptionDeactivationEnabled": true,
    "MaxConsecutiveFailCountBeforeDeactivateSubscription": 10,
    "SignatureHeaderName": "X-Eaf-Signature-256",
    "SignatureValueTemplate": "sha256={0}",
    "DataProtectionPurpose": "eaf-webhooks-subscription-secret"
  }
}
```

## Dependências

- `Abp` 10.5.0
- `Abp.AspNetCore` 10.5.0
