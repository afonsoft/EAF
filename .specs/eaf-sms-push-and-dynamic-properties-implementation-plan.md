# EAF — Plano de implementação: SMS/Push e Dynamic Entity Properties

> **Status:** `Eaf.Notifications.Sms` e `Eaf.Notifications.Push` implementados (PR `feature/eaf-notifications-sms-push`). `Eaf.DynamicEntityProperties` ainda pendente.

## Escopo

Implementar os módulos pendentes selecionados:

1. `Eaf.Notifications.Sms` + `Eaf.Notifications.Push` (podem virar `Eaf.Notifications.Channels` se preferir). [Implementado]
2. `Eaf.DynamicEntityProperties` (reaproveita `Abp.DynamicEntityProperties` já ativo no EAF). [Pendente]

## 1. `Eaf.Notifications.Sms`

### Objetivo
Fornecer `ISmsSender` com providers configuráveis, incluindo um **provider HTTP genérico** que pode ser usado para Zenvia, Twilio REST, SNS HTTP, etc.

### Estrutura

```text
src/Eaf.Notifications.Sms/
  EafNotificationsSmsModule.cs
  ISmsSender.cs
  ISmsProvider.cs
  SmsMessage.cs
  SmsSendResult.cs
  SmsOptions.cs
  SmsRealTimeNotifier.cs
  Providers/
    TwilioSmsProvider.cs          (Twilio SDK 7.x - opcional)
    AwsSnsSmsProvider.cs          (AWSSDK.SNS - opcional)
    GenericHttpSmsProvider.cs     (configurável para Zenvia e outros)
```

### `GenericHttpSmsProvider`

Configuração via `IConfiguration` section `Eaf:Sms`:

- `Provider`: `GenericHttp`
- `GenericHttp:BaseUrl`: URL base (ex: `https://api-rest.zenvia.com`)
- `GenericHttp:Endpoint`: caminho (ex: `/services/send-sms`)
- `GenericHttp:AuthenticationType`: `None`, `Basic`, `Bearer`, `Header`
- `GenericHttp:Username` / `Password` (para Basic)
- `GenericHttp:Token` (para Bearer)
- `GenericHttp:ApiKeyHeaderName` / `ApiKey` (para Header)
- `GenericHttp:ContentType`: `Json` ou `Form`
- `GenericHttp:Template`: string com placeholders `{{phoneNumber}}`, `{{body}}`, `{{from}}`

Exemplo para Zenvia:

```json
{
  "Eaf:Sms:Provider": "GenericHttp",
  "Eaf:Sms:GenericHttp:BaseUrl": "https://api-rest.zenvia.com",
  "Eaf:Sms:GenericHttp:Endpoint": "/services/send-sms",
  "Eaf:Sms:GenericHttp:AuthenticationType": "Basic",
  "Eaf:Sms:GenericHttp:Username": "user",
  "Eaf:Sms:GenericHttp:Password": "pass",
  "Eaf:Sms:GenericHttp:ContentType": "Json",
  "Eaf:Sms:GenericHttp:Template": "{ \"sendSmsRequest\": { \"from\": \"{{from}}\", \"to\": \"{{phoneNumber}}\", \"msg\": \"{{body}}\" } }"
}
```

### Integração com notificações
- `SmsRealTimeNotifier : IRealTimeNotifier, ITransientDependency`
- Registrado em `EafNotificationsSmsModule` adicionando o tipo à lista `Configuration.Notifications.Notifiers`.
- Envia SMS quando `userNotification.Notification.Data` for `MessageNotificationData` e o destinatário tiver `PhoneNumber`.

### Testes
- `Eaf.Notifications.Sms.Tests`
- Testes BDD para `GenericHttpSmsProvider` (mock de `HttpMessageHandler`).
- Testes para validação de número e seleção de provider.

## 2. `Eaf.Notifications.Push`

### Objetivo
Fornecer envio de Web Push via VAPID e um **provider HTTP genérico** para gateways customizados (incluindo Zenvia, caso use WhatsApp/push via HTTP).

### Estrutura

```text
src/Eaf.Notifications.Push/
  EafNotificationsPushModule.cs
  IPushNotificationSender.cs
  IPushNotificationProvider.cs
  PushNotificationMessage.cs
  PushSubscription.cs
  PushOptions.cs
  PushRealTimeNotifier.cs
  Providers/
    WebPushNotificationProvider.cs  (pacote WebPush 1.0.12)
    GenericHttpPushProvider.cs      (POST para endpoint configurado)
```

### Entidade `PushSubscription`

```csharp
public class PushSubscription : FullAuditedEntity<long>, IMayHaveTenant
{
    public long UserId { get; set; }
    public int? TenantId { get; set; }
    public string Endpoint { get; set; }
    public string P256dh { get; set; }
    public string Auth { get; set; }
}
```

- Adicionar `DbSet<PushSubscription>` em `ProjectNameDbContext` (template) e `SampleAppDbContext` (testes) + migrations.
- Expor `IPushSubscriptionManager` para CRUD.

### Web Push
- Usar pacote `WebPush` 1.0.12 para VAPID + payload.
- Chaves VAPID em settings/KeyVault (`Eaf:Push:VapidPublicKey`, `Eaf:Push:VapidPrivateKey`, `Eaf:Push:VapidSubject`).

### `GenericHttpPushProvider`
- POST para `Eaf:Push:GenericHttp:BaseUrl/Endpoint` com template contendo `{{endpoint}}`, `{{p256dh}}`, `{{auth}}`, `{{title}}`, `{{message}}`, `{{url}}`.

### Integração com notificações
- `PushRealTimeNotifier : IRealTimeNotifier`.
- Busca subscriptions do usuário e envia.
- Remove subscriptions com HTTP 410 (expiradas).

### Testes
- `Eaf.Notifications.Push.Tests`
- Testes BDD para `WebPushNotificationProvider` com `HttpMessageHandler` mock.
- Testes para `GenericHttpPushProvider`.

## 3. `Eaf.DynamicEntityProperties`

### Objetivo
Reaproveitar `Abp.DynamicEntityProperties` (já ativo em `MiddlewareCoreModule` e com tabelas `AbpDynamic*`) e expor application services + UI Angular.

### Backend

```text
src/Eaf.DynamicEntityProperties/
  EafDynamicEntityPropertiesModule.cs
  Application/
    IDynamicPropertyAppService.cs
    DynamicPropertyAppService.cs
    IDynamicEntityPropertyAppService.cs
    DynamicEntityPropertyAppService.cs
    IDynamicEntityPropertyValueAppService.cs
    DynamicEntityPropertyValueAppService.cs
    Dto/
      DynamicPropertyDto.cs
      CreateDynamicPropertyInput.cs
      DynamicEntityPropertyValueDto.cs
      CreateOrUpdateDynamicEntityPropertyValueInput.cs
  Permissions/
    DynamicEntityPropertiesPermissionNames.cs
```

### Regras
- Usar `IDynamicPropertyManager`, `IDynamicEntityPropertyManager` e `IDynamicEntityPropertyValueManager` do ABP.
- Não reimplementar entidades/EF (já existem em `Abp` e nas migrations do template).
- Permissões:
  - `Pages_Administration_DynamicProperties`
  - `Pages_Administration_DynamicProperties_Values`

### Angular
- `admin/dynamic-properties` — CRUD de propriedades dinâmicas.
- `shared/common/dynamic-entity-property-manager` — componente reutilizável para gerenciar valores de qualquer entidade.
- Usar PrimeNG 17 + reactive forms.

### Testes
- `Eaf.DynamicEntityProperties.Tests`
- Testes BDD para app services usando `EafZeroTestBase` e `SampleAppDbContext`.

## 4. Branches e entrega

- **PR 1:** `feature/eaf-notifications-sms-push`
  - `Eaf.Notifications.Sms` + `Eaf.Notifications.Push` + tests.
  - Migrations de `PushSubscription` no template e testes.
  - `README.md` e índice de specs.
- **PR 2:** `feature/eaf-dynamic-entity-properties`
  - `Eaf.DynamicEntityProperties` backend + tests.
  - UI Angular (se aprovado neste plano).
  - `README.md` e índice de specs.

## 5. Dependências

- `Eaf.Notifications.Push` depende de `Eaf.Notifications.Sms` apenas para compartilhar `MessageNotificationData`/extensões? Pode ser evitado.
- Ambos dependem de `Abp` e `Eaf.Middleware.Core` (para `UserManager` e `MessageNotificationData`).
- `Eaf.DynamicEntityProperties` depende de `Abp` + `Abp.AutoMapper`.

## 6. Validação

- `dotnet build Eaf.sln --configuration Release`
- `dotnet test Eaf.sln --configuration Release --no-build`
- SonarCloud sem novos issues.
- Cobertura ≥ 90% nos novos módulos.

## 7. Riscos e mitigações

| Risco | Mitigação |
|---|---|
| Zenvia muda API | Provider HTTP genérico com templates configuráveis |
| WebPush package não suporta .NET 10 | Testar build; se necessário, usar implementação manual mínima |
| Migrations de `PushSubscription` conflitarem | Adicionar em ambos os templates/API e testes |
| `Abp.DynamicEntityProperties` managers mudarem | Usar interfaces públicas do ABP, não classes internas |

## 8. Definição de pronto

- Módulos compilam e geram NuGet.
- Testes passam.
- `README.md` e `docs/modules/USAGE.md` atualizados.
- Índice `.specs/eaf-specs-index-and-roadmap-2026.md` atualizado.
