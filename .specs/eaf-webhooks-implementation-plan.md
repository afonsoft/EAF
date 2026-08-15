# Plano de Implementação — Eaf.Webhooks

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:subagent-driven-development` (recomendado) ou `superpowers:executing-plans` para implementar este plano task-a-task. Steps usam checkbox (`- [ ]`) para tracking.

**Goal:** Criar o módulo `Eaf.Webhooks` reutilizando o subsistema completo de webhooks do ABP 10.5 (`Abp.Webhooks` / `Abp.AspNetCore.Webhook`), aplicando configuração, HMAC e segurança específicas do EAF, sem reimplementar o que o ABP já fornece.

**Architecture:** O ABP já entrega `IWebhookPublisher`, `IWebhookSubscriptionManager`, `IWebhookSender`, `WebhookSenderJob`, `IWebhooksConfiguration` e as entidades/tabelas EF (`WebhookSubscriptionInfo`, `WebhookEvent`, `WebhookSendAttempt`) via `AbpZeroCommonDbContext`. O `Eaf.Webhooks` herda e substitui (`IsDefault()`) `IWebhookManager`, `IWebhookSubscriptionManager` e `IWebhookSender` para aplicar header `X-Eaf-Signature-256`, payload `{eventName, timestamp, payload}`, guarda HTTPS, criptografia do segredo, deduplicação de assinaturas e persistência correta de `IsActive`/`Secret`.

**Tech Stack:** .NET 10, ABP 10.5.0, Castle Windsor, `System.Net.Http`, ASP.NET Core Data Protection, xUnit + Shouldly + NSubstitute.

---

## O que já existe e será reaproveitado

| Onde hoje | O que é | Destino / Uso |
|---|---|---|
| `Abp.Webhooks` (pacote `Abp` 10.5.0) | `IWebhookPublisher`, `IWebhookSubscriptionManager`, `DefaultWebhookPublisher`, `WebhookSubscriptionManager`, `WebhookManager`, `WebhookSenderJob`, `WebhookSubscriptionInfo`, `WebhookEvent`, `WebhookSendAttempt` | Reutilizado diretamente; implementações do ABP são substituídas pelo EAF como `IsDefault()` |
| `Abp.AspNetCore.Webhook` (pacote `Abp.AspNetCore` 10.5.0) | `AspNetCoreWebhookSender`, `IHttpClientFactory` nomeado `WebhookSenderHttpClient` | Herdado por `EafWebhookSender` |
| `AbpZeroCommonDbContext` | Mapeamento EF de `AbpWebhookSubscriptions`, `AbpWebhookEvents`, `AbpWebhookSendAttempts` | Nenhuma nova migration necessária — reutilizar tabelas existentes |
| `src/Eaf.Middleware.Application/WebHooks/WebhookSubscriptionAppService.cs` | App service que usa `IWebhookSubscriptionManager` | Sem alteração de contrato; passa a usar `EafWebhookSubscriptionManager` por DI |
| `src/Eaf.Middleware.Application/WebHooks/WebhookSendAttemptAppService.cs` | Reenvia webhook via `WebhookSenderJob` | Sem alteração; `Secret` cifrado é desprotegido em `EafWebhookManager.SignWebhookRequest` |
| `src/Eaf.Middleware.Web.Core/WebHooks/EafWebhookDefinitionProvider.cs` | Provedor de definições de webhook do EAF | **Permanece** em `Eaf.Middleware.Web.Core` porque é específico do domínio EAF (`NewUserRegistered`) |
| `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs` | `services.AddHttpClient(); services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);` | Substituído por `services.AddEafWebhooks(configuration);` |

## Notas sobre a spec `eaf-module-webhooks.spec.md`

- A spec prevê criação de entidades `WebhookSubscription`/`WebhookDelivery` e migration. Como o ABP 10.5 já fornece `WebhookSubscriptionInfo` e `WebhookSendAttempt` e o `AbpZeroCommonDbContext` já mapeia as tabelas, **não haverá nova migration**.
- `WebhookDelivery` da spec mapeia para `WebhookSendAttempt` do ABP.
- A camada de aplicação (`WebhookSubscriptionAppService`) já existe no `Eaf.Middleware.Application` e será mantida; o novo módulo fornece as implementações de domínio/infrastructure por baixo.
- `Eaf.KeyVault` pode ser integrado posteriormente implementando `IWebhookSubscriptionSecretProtector`; a implementação padrão usa ASP.NET Core Data Protection, que o template já configura com chaves persistentes em disco.

---

## Estrutura de arquivos

```text
src/Eaf.Webhooks/
  Eaf.Webhooks.csproj
  EafWebhooksModule.cs
  README.md
  IWebhookSubscriptionSecretProtector.cs
  EafDataProtectionWebhookSecretProtector.cs
  EafPlainWebhookSecretProtector.cs
  EafWebhookManager.cs
  EafWebhookSubscriptionManager.cs
  EafWebhookSender.cs
  Configuration/
    EafWebhooksOptions.cs
    EafWebhooksServiceCollectionExtensions.cs

test/Eaf.Webhooks.Tests/
  Eaf.Webhooks.Tests.csproj
  EafWebhooksTestModule.cs
  EafWebhookManagerTests.cs
  EafWebhookSubscriptionManagerTests.cs
  EafWebhookSenderTests.cs
  EafWebhooksModuleTests.cs
  Fakes/
    FakeUnitOfWorkManager.cs
    InMemoryWebhookSubscriptionsStore.cs
    TestHttpHandler.cs
```

---

## Task 1: Criar o projeto `src/Eaf.Webhooks/Eaf.Webhooks.csproj`

**Files:**
- Create: `src/Eaf.Webhooks/Eaf.Webhooks.csproj`
- Create: `src/Eaf.Webhooks/README.md` (conteúdo no final desta task)

- [ ] **Step 1: Criar csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Import Project="..\..\common.props" />

  <PropertyGroup>
    <RootNamespace>Eaf</RootNamespace>
    <TargetFrameworks>net10.0</TargetFrameworks>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <AssemblyName>Eaf.Webhooks</AssemblyName>
    <PackageId>Eaf.Webhooks</PackageId>
    <PackageTags>asp.net;asp.net mvc;application framework;web framework;framework;domain driven design;webhooks;Eaf;Boilerplate;NET10</PackageTags>
    <Description>Enterprise Application Foundation - Webhooks module</Description>
    <PackageReadmeFile>README.md</PackageReadmeFile>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Abp" Version="10.5.0" />
    <PackageReference Include="Abp.AspNetCore" Version="10.5.0" />
  </ItemGroup>

  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
  </ItemGroup>

  <ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
      <_Parameter1>Eaf.Webhooks.Tests</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Update="SourceLink.Create.CommandLine" Version="2.8.3" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Criar README.md**

```markdown
# Eaf.Webhooks

Módulo EAF para envio de webhooks HTTP. Reutiliza o subsistema `Abp.Webhooks` e aplica assinatura HMAC-SHA256, guarda HTTPS, criptografia do segredo e deduplicação de assinaturas.

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
```
```

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Webhooks/Eaf.Webhooks.csproj src/Eaf.Webhooks/README.md
git commit -m "feat(webhooks): add Eaf.Webhooks project and README"
```

---

## Task 2: Criar configuração e extensão de DI

**Files:**
- Create: `src/Eaf.Webhooks/Configuration/EafWebhooksOptions.cs`
- Create: `src/Eaf.Webhooks/Configuration/EafWebhooksServiceCollectionExtensions.cs`

- [ ] **Step 1: Criar `EafWebhooksOptions.cs`**

```csharp
using System.Text.Json;

namespace Eaf.Webhooks.Configuration
{
    /// <summary>
    /// Opções de configuração do módulo Eaf.Webhooks.
    /// </summary>
    public class EafWebhooksOptions
    {
        /// <summary>
        /// Permite URLs HTTP não seguras. O padrão é false.
        /// </summary>
        public bool AllowHttp { get; set; }

        /// <summary>
        /// Timeout das requisições HTTP em segundos.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Número máximo de tentativas de envio (incluindo a primeira).
        /// </summary>
        public int MaxSendAttemptCount { get; set; } = 5;

        /// <summary>
        /// Desativa automaticamente assinaturas que falham consecutivamente.
        /// </summary>
        public bool IsAutomaticSubscriptionDeactivationEnabled { get; set; } = true;

        /// <summary>
        /// Número máximo de falhas consecutivas antes de desativar a assinatura.
        /// </summary>
        public int MaxConsecutiveFailCountBeforeDeactivateSubscription { get; set; } = 10;

        /// <summary>
        /// Nome do header de assinatura HMAC.
        /// </summary>
        public string SignatureHeaderName { get; set; } = "X-Eaf-Signature-256";

        /// <summary>
        /// Template do valor do header de assinatura.
        /// </summary>
        public string SignatureValueTemplate { get; set; } = "sha256={0}";

        /// <summary>
        /// Propósito usado pelo ASP.NET Core Data Protection para criptografar segredos.
        /// </summary>
        public string DataProtectionPurpose { get; set; } = "eaf-webhooks-subscription-secret";

        /// <summary>
        /// Opções de serialização JSON usadas no payload do webhook.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
```

- [ ] **Step 2: Criar `EafWebhooksServiceCollectionExtensions.cs`**

```csharp
using System;
using Abp.AspNetCore.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Webhooks.Configuration
{
    /// <summary>
    /// Extensões para registro dos serviços Eaf.Webhooks no IServiceCollection.
    /// </summary>
    public static class EafWebhooksServiceCollectionExtensions
    {
        /// <summary>
        /// Registra opções e HttpClient nomeado usado pelo sender de webhooks.
        /// </summary>
        public static IServiceCollection AddEafWebhooks(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.Configure<EafWebhooksOptions>(configuration.GetSection("EafWebhooks"));
            services.AddHttpClient();
            services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);

            return services;
        }
    }
}
```

- [ ] **Step 3: Build rápido do novo projeto**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

Expected: build OK (0 erros, 0 warnings).

- [ ] **Step 4: Commit**

```bash
git add src/Eaf.Webhooks/Configuration
git commit -m "feat(webhooks): add EafWebhooksOptions and AddEafWebhooks extension"
```

---

## Task 3: Criar abstração e implementações de proteção de segredo

**Files:**
- Create: `src/Eaf.Webhooks/IWebhookSubscriptionSecretProtector.cs`
- Create: `src/Eaf.Webhooks/EafDataProtectionWebhookSecretProtector.cs`
- Create: `src/Eaf.Webhooks/EafPlainWebhookSecretProtector.cs`

- [ ] **Step 1: Criar interface**

```csharp
namespace Eaf.Webhooks
{
    /// <summary>
    /// Protege e recupera segredos de assinaturas de webhook.
    /// </summary>
    public interface IWebhookSubscriptionSecretProtector
    {
        /// <summary>
        /// Criptografa o segredo em texto plano.
        /// </summary>
        string Protect(string plainText);

        /// <summary>
        /// Descriptografa o segredo previamente protegido.
        /// </summary>
        string Unprotect(string cipherText);
    }
}
```

- [ ] **Step 2: Criar implementação com Data Protection**

```csharp
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Protetor de segredos usando ASP.NET Core Data Protection.
    /// </summary>
    internal class EafDataProtectionWebhookSecretProtector : IWebhookSubscriptionSecretProtector
    {
        private readonly IDataProtector _protector;

        public EafDataProtectionWebhookSecretProtector(IDataProtectionProvider dataProtectionProvider, IOptions<EafWebhooksOptions> optionsAccessor)
        {
            if (dataProtectionProvider == null)
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            if (optionsAccessor?.Value == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _protector = dataProtectionProvider.CreateProtector(optionsAccessor.Value.DataProtectionPurpose);
        }

        public string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            return _protector.Protect(plainText);
        }

        public string Unprotect(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            return _protector.Unprotect(cipherText);
        }
    }
}
```

- [ ] **Step 3: Criar implementação plain (fallback para testes)**

```csharp
namespace Eaf.Webhooks
{
    /// <summary>
    /// Protetor de segredos sem criptografia (fallback para testes ou ambientes sem Data Protection).
    /// </summary>
    internal class EafPlainWebhookSecretProtector : IWebhookSubscriptionSecretProtector
    {
        public string Protect(string plainText) => plainText;

        public string Unprotect(string cipherText) => cipherText;
    }
}
```

- [ ] **Step 4: Build rápido**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

- [ ] **Step 5: Commit**

```bash
git add src/Eaf.Webhooks/IWebhookSubscriptionSecretProtector.cs src/Eaf.Webhooks/EafDataProtectionWebhookSecretProtector.cs src/Eaf.Webhooks/EafPlainWebhookSecretProtector.cs
git commit -m "feat(webhooks): add secret protector abstraction and Data Protection fallback"
```

---

## Task 4: Criar `EafWebhookManager` (HMAC e payload EAF)

**Files:**
- Create: `src/Eaf.Webhooks/EafWebhookManager.cs`

- [ ] **Step 1: Criar manager**

```csharp
using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Webhooks;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Gerenciador de webhooks EAF. Reutiliza <see cref="WebhookManager"/> do ABP e aplica HMAC e payload no formato EAF.
    /// </summary>
    public class EafWebhookManager : WebhookManager
    {
        private readonly IWebhookSubscriptionSecretProtector _secretProtector;
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly EafWebhooksOptions _options;

        public EafWebhookManager(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSendAttemptStore webhookSendAttemptStore,
            IWebhookSubscriptionSecretProtector secretProtector,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(webhooksConfiguration, webhookSendAttemptStore)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _secretProtector = secretProtector;
            _options = optionsAccessor.Value;
        }

        public override string GetSerializedBody(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));

            if (webhookSenderArgs.SendExactSameData)
                return webhookSenderArgs.Data;

            var payload = base.GetWebhookPayload(webhookSenderArgs);
            return SerializeEafPayload(payload);
        }

        public override async Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));

            if (webhookSenderArgs.SendExactSameData)
                return webhookSenderArgs.Data;

            var payload = await base.GetWebhookPayloadAsync(webhookSenderArgs);
            return SerializeEafPayload(payload);
        }

        public override void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(serializedBody))
                throw new ArgumentNullException(nameof(serializedBody));

            var plainSecret = _secretProtector.Unprotect(secret);

            if (string.IsNullOrWhiteSpace(plainSecret))
                throw new ArgumentException("O segredo do webhook está ausente ou não pôde ser descriptografado.", nameof(secret));

            var secretBytes = Encoding.UTF8.GetBytes(plainSecret);

            using (var hasher = new HMACSHA256(secretBytes))
            {
                request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

                var data = Encoding.UTF8.GetBytes(serializedBody);
                var hash = hasher.ComputeHash(data);
                var headerValue = string.Format(
                    CultureInfo.InvariantCulture,
                    _options.SignatureValueTemplate,
                    Convert.ToHexString(hash).ToLowerInvariant());

                request.Headers.Add(_options.SignatureHeaderName, headerValue);
            }
        }

        private string SerializeEafPayload(WebhookPayload payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var eafPayload = new
            {
                eventName = payload.WebhookEvent,
                timestamp = payload.CreationTimeUtc,
                payload = payload.Data
            };

            return _webhooksConfiguration.JsonSerializerOptions != null
                ? eafPayload.ToJsonString(_webhooksConfiguration.JsonSerializerOptions)
                : eafPayload.ToJsonString();
        }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Webhooks/EafWebhookManager.cs
git commit -m "feat(webhooks): add EafWebhookManager with HMAC SHA-256 and EAF payload shape"
```

---

## Task 5: Criar `EafWebhookSubscriptionManager` (CRUD, validação e criptografia)

**Files:**
- Create: `src/Eaf.Webhooks/EafWebhookSubscriptionManager.cs`

- [ ] **Step 1: Criar manager**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.UI;
using Abp.Webhooks;
using Abp.Webhooks.Extensions;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Gerenciador de assinaturas de webhook EAF. Reutiliza o ABP e ajusta persistência de IsActive e segredo.
    /// </summary>
    public class EafWebhookSubscriptionManager : WebhookSubscriptionManager
    {
        private const string WebhookSubscriptionSecretPrefix = "whs_";

        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebhookSubscriptionSecretProtector _secretProtector;
        private readonly EafWebhooksOptions _options;

        public EafWebhookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebhookDefinitionManager webhookDefinitionManager,
            IWebhookSubscriptionSecretProtector secretProtector,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(guidGenerator, webhookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _secretProtector = secretProtector;
            _options = optionsAccessor.Value;
        }

        public override async Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            Validate(webhookSubscription);
            await CheckDuplicateAsync(webhookSubscription);

            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await CheckIfPermissionsGrantedAsync(webhookSubscription);

                if (webhookSubscription.Id == default)
                    await CreateSubscriptionAsync(webhookSubscription);
                else
                    await UpdateSubscriptionAsync(webhookSubscription);
            });
        }

        public override void AddOrUpdateSubscription(WebhookSubscription webhookSubscription)
        {
            Validate(webhookSubscription);
            CheckDuplicate(webhookSubscription);

            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                CheckIfPermissionsGranted(webhookSubscription);

                if (webhookSubscription.Id == default)
                    CreateSubscription(webhookSubscription);
                else
                    UpdateSubscription(webhookSubscription);
            });
        }

        private async Task CreateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            webhookSubscription.Id = _guidGenerator.Create();

            if (string.IsNullOrWhiteSpace(webhookSubscription.Secret))
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");

            webhookSubscription.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            await WebhookSubscriptionsStore.InsertAsync(webhookSubscription.ToWebhookSubscriptionInfo());
        }

        private async Task UpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            var info = await WebhookSubscriptionsStore.GetAsync(webhookSubscription.Id);

            info.WebhookUri = webhookSubscription.WebhookUri;
            info.Webhooks = webhookSubscription.Webhooks.ToJsonString();
            info.Headers = webhookSubscription.Headers != null
                ? webhookSubscription.Headers.ToJsonString()
                : "{}";
            info.IsActive = webhookSubscription.IsActive;

            if (!string.IsNullOrWhiteSpace(webhookSubscription.Secret) && webhookSubscription.Secret != info.Secret)
                info.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            await WebhookSubscriptionsStore.UpdateAsync(info);
        }

        private void CreateSubscription(WebhookSubscription webhookSubscription)
        {
            webhookSubscription.Id = _guidGenerator.Create();

            if (string.IsNullOrWhiteSpace(webhookSubscription.Secret))
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");

            webhookSubscription.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            WebhookSubscriptionsStore.Insert(webhookSubscription.ToWebhookSubscriptionInfo());
        }

        private void UpdateSubscription(WebhookSubscription webhookSubscription)
        {
            var info = WebhookSubscriptionsStore.Get(webhookSubscription.Id);

            info.WebhookUri = webhookSubscription.WebhookUri;
            info.Webhooks = webhookSubscription.Webhooks.ToJsonString();
            info.Headers = webhookSubscription.Headers != null
                ? webhookSubscription.Headers.ToJsonString()
                : "{}";
            info.IsActive = webhookSubscription.IsActive;

            if (!string.IsNullOrWhiteSpace(webhookSubscription.Secret) && webhookSubscription.Secret != info.Secret)
                info.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            WebhookSubscriptionsStore.Update(info);
        }

        protected virtual void Validate(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription == null)
                throw new ArgumentNullException(nameof(webhookSubscription));
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
                throw new UserFriendlyException("Pelo menos um evento de webhook é obrigatório.");
            if (string.IsNullOrWhiteSpace(webhookSubscription.WebhookUri))
                throw new UserFriendlyException("A URI do webhook é obrigatória.");
            if (!Uri.TryCreate(webhookSubscription.WebhookUri, UriKind.Absolute, out var uri))
                throw new UserFriendlyException("A URI do webhook não é válida.");
            if (!_options.AllowHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new UserFriendlyException("A URI do webhook deve usar HTTPS.");
        }

        protected virtual async Task CheckDuplicateAsync(WebhookSubscription webhookSubscription)
        {
            var existing = await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(webhookSubscription.TenantId);
            ThrowIfDuplicate(webhookSubscription, existing);
        }

        protected virtual void CheckDuplicate(WebhookSubscription webhookSubscription)
        {
            var existing = WebhookSubscriptionsStore.GetAllSubscriptions(webhookSubscription.TenantId);
            ThrowIfDuplicate(webhookSubscription, existing);
        }

        private void ThrowIfDuplicate(WebhookSubscription webhookSubscription, List<WebhookSubscriptionInfo> existing)
        {
            var inputEvents = new HashSet<string>(webhookSubscription.Webhooks);

            foreach (var item in existing.Where(x => x.Id != webhookSubscription.Id))
            {
                if (item.WebhookUri != webhookSubscription.WebhookUri)
                    continue;

                var itemEvents = item.GetSubscribedWebhooks();
                if (inputEvents.Any(e => itemEvents.Contains(e)))
                    throw new UserFriendlyException("Já existe uma assinatura com a mesma URL e evento para este tenant.");
            }
        }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Webhooks/EafWebhookSubscriptionManager.cs
git commit -m "feat(webhooks): add EafWebhookSubscriptionManager with validation, deduplication and secret encryption"
```

---

## Task 6: Criar `EafWebhookSender` (guarda HTTPS)

**Files:**
- Create: `src/Eaf.Webhooks/EafWebhookSender.cs`

- [ ] **Step 1: Criar sender**

```csharp
using System;
using System.Net.Http;
using Abp.AspNetCore.Webhook;
using Abp.UI;
using Abp.Webhooks;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Sender de webhooks EAF baseado em <see cref="AspNetCoreWebhookSender"/> com guarda HTTPS.
    /// </summary>
    public class EafWebhookSender : AspNetCoreWebhookSender
    {
        private readonly EafWebhooksOptions _options;

        public EafWebhookSender(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookManager webhookManager,
            IHttpClientFactory clientFactory,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(webhooksConfiguration, webhookManager, clientFactory)
        {
            _options = optionsAccessor.Value;
        }

        protected override HttpRequestMessage CreateWebhookRequestMessage(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));
            if (string.IsNullOrWhiteSpace(webhookSenderArgs.WebhookUri))
                throw new ArgumentException("A URI do webhook é obrigatória.", nameof(webhookSenderArgs));

            if (!_options.AllowHttp)
            {
                if (!Uri.TryCreate(webhookSenderArgs.WebhookUri, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                    throw new UserFriendlyException("A URI do webhook deve usar HTTPS.");
            }

            return base.CreateWebhookRequestMessage(webhookSenderArgs);
        }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Webhooks/EafWebhookSender.cs
git commit -m "feat(webhooks): add EafWebhookSender with HTTPS enforcement"
```

---

## Task 7: Criar `EafWebhooksModule`

**Files:**
- Create: `src/Eaf.Webhooks/EafWebhooksModule.cs`

- [ ] **Step 1: Criar módulo**

```csharp
using System;
using Abp.AspNetCore;
using Abp.Modules;
using Abp.Webhooks;
using Castle.MicroKernel.Registration;
using Eaf.Webhooks.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Módulo ABP que configura e inicializa o Eaf.Webhooks.
    /// </summary>
    [DependsOn(
        typeof(AbpKernelModule),
        typeof(AbpAspNetCoreModule)
    )]
    public class EafWebhooksModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IWebhookSubscriptionSecretProtector>()
                    .UsingFactoryMethod(() =>
                    {
                        var options = IocManager.Resolve<IOptions<EafWebhooksOptions>>();

                        if (IocManager.IsRegistered<IDataProtectionProvider>())
                        {
                            var provider = IocManager.Resolve<IDataProtectionProvider>();
                            return new EafDataProtectionWebhookSecretProtector(provider, options);
                        }

                        return new EafPlainWebhookSecretProtector();
                    })
                    .LifestyleTransient()
                    .IsDefault());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafWebhooksModule).GetAssembly());

            IocManager.IocContainer.Register(
                Component.For<IWebhookManager>().ImplementedBy<EafWebhookManager>().LifestyleTransient().IsDefault(),
                Component.For<IWebhookSubscriptionManager>().ImplementedBy<EafWebhookSubscriptionManager>().LifestyleTransient().IsDefault(),
                Component.For<IWebhookSender>().ImplementedBy<EafWebhookSender>().LifestyleTransient().IsDefault()
            );
        }

        public override void PostInitialize()
        {
            var options = IocManager.Resolve<IOptions<EafWebhooksOptions>>().Value;
            var webhooksConfiguration = Configuration.Webhooks;

            if (options.TimeoutSeconds > 0)
                webhooksConfiguration.TimeoutDuration = TimeSpan.FromSeconds(options.TimeoutSeconds);

            if (options.MaxSendAttemptCount > 0)
                webhooksConfiguration.MaxSendAttemptCount = options.MaxSendAttemptCount;

            webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled = options.IsAutomaticSubscriptionDeactivationEnabled;

            if (options.MaxConsecutiveFailCountBeforeDeactivateSubscription > 0)
                webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription = options.MaxConsecutiveFailCountBeforeDeactivateSubscription;

            webhooksConfiguration.JsonSerializerOptions = options.JsonSerializerOptions;
        }
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Eaf.Webhooks/Eaf.Webhooks.csproj --configuration Release
```

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Webhooks/EafWebhooksModule.cs
git commit -m "feat(webhooks): add EafWebhooksModule wiring options and default services"
```

---

## Task 8: Integrar `Eaf.Webhooks` no `Eaf.Middleware.Web.Core`

**Files:**
- Modify: `src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj`
- Modify: `src/Eaf.Middleware.Web.Core/MiddlewareWebCoreModule.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs`

- [ ] **Step 1: Adicionar ProjectReference**

Em `src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj`, adicionar dentro de `<ItemGroup>` (próximo a `Eaf.SignalR`):

```xml
<ProjectReference Include="..\Eaf.Webhooks\Eaf.Webhooks.csproj" />
```

- [ ] **Step 2: Adicionar dependência de módulo**

Em `src/Eaf.Middleware.Web.Core/MiddlewareWebCoreModule.cs`, adicionar em `[DependsOn(...)]`:

```csharp
typeof(Eaf.Webhooks.EafWebhooksModule),
```

Adicionar `using Eaf.Webhooks;` no topo do arquivo se preferir referenciar sem namespace completo.

- [ ] **Step 3: Substituir registro de HttpClient por `AddEafWebhooks`**

Em `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs`:

Remover:

```csharp
using Abp.AspNetCore.Webhook;
```

Adicionar:

```csharp
using Eaf.Webhooks.Configuration;
```

Substituir:

```csharp
services.AddHttpClient();
services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);
```

Por:

```csharp
services.AddEafWebhooks(configuration);
```

- [ ] **Step 4: Build do Middleware.Web.Core**

```bash
dotnet build src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj --configuration Release
```

- [ ] **Step 5: Commit**

```bash
git add src/Eaf.Middleware.Web.Core
git commit -m "feat(webhooks): wire Eaf.Webhooks into MiddlewareWebCore"
```

---

## Task 9: Criar projeto de testes e testes

**Files:**
- Create: `test/Eaf.Webhooks.Tests/Eaf.Webhooks.Tests.csproj`
- Create: `test/Eaf.Webhooks.Tests/EafWebhooksTestModule.cs`
- Create: `test/Eaf.Webhooks.Tests/Fakes/FakeUnitOfWorkManager.cs`
- Create: `test/Eaf.Webhooks.Tests/Fakes/InMemoryWebhookSubscriptionsStore.cs`
- Create: `test/Eaf.Webhooks.Tests/Fakes/TestHttpHandler.cs`
- Create: `test/Eaf.Webhooks.Tests/EafWebhookManagerTests.cs`
- Create: `test/Eaf.Webhooks.Tests/EafWebhookSubscriptionManagerTests.cs`
- Create: `test/Eaf.Webhooks.Tests/EafWebhookSenderTests.cs`
- Create: `test/Eaf.Webhooks.Tests/EafWebhooksModuleTests.cs`

- [ ] **Step 1: Criar csproj de testes**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>annotations</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="Shouldly" Version="4.3.0" />
    <PackageReference Include="NSubstitute" Version="5.3.0" />
    <PackageReference Include="Abp" Version="10.5.0" />
    <PackageReference Include="Abp.AspNetCore" Version="10.5.0" />
    <PackageReference Include="Abp.TestBase" Version="10.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Eaf.Webhooks\Eaf.Webhooks.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Update="coverlet.collector" Version="10.0.1" />
    <PackageReference Update="coverlet.msbuild" Version="8.0.0" />
    <PackageReference Update="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Criar fakes**

`test/Eaf.Webhooks.Tests/Fakes/FakeUnitOfWorkManager.cs`:

```csharp
using System;
using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace Eaf.Webhooks.Tests
{
    public class FakeUnitOfWorkManager : IUnitOfWorkManager
    {
        public IActiveUnitOfWork Current => null;

        public IUnitOfWorkCompleteHandle Begin()
        {
            return new FakeUnitOfWorkCompleteHandle();
        }

        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return new FakeUnitOfWorkCompleteHandle();
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            return new FakeUnitOfWorkCompleteHandle();
        }
    }

    public class FakeUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        public void Complete() { }

        public Task CompleteAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}
```

`test/Eaf.Webhooks.Tests/Fakes/InMemoryWebhookSubscriptionsStore.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Webhooks;

namespace Eaf.Webhooks.Tests
{
    public class InMemoryWebhookSubscriptionsStore : IWebhookSubscriptionsStore
    {
        private readonly List<WebhookSubscriptionInfo> _items = new();

        public Task<WebhookSubscriptionInfo> GetAsync(Guid id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                throw new Exception($"Subscription {id} not found.");
            return Task.FromResult(item);
        }

        public WebhookSubscriptionInfo Get(Guid id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                throw new Exception($"Subscription {id} not found.");
            return item;
        }

        public Task InsertAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            _items.Add(webhookSubscription);
            return Task.CompletedTask;
        }

        public void Insert(WebhookSubscriptionInfo webhookSubscription)
        {
            _items.Add(webhookSubscription);
        }

        public Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            var idx = _items.FindIndex(x => x.Id == webhookSubscription.Id);
            if (idx >= 0)
                _items[idx] = webhookSubscription;
            return Task.CompletedTask;
        }

        public void Update(WebhookSubscriptionInfo webhookSubscription)
        {
            var idx = _items.FindIndex(x => x.Id == webhookSubscription.Id);
            if (idx >= 0)
                _items[idx] = webhookSubscription;
        }

        public Task DeleteAsync(Guid id)
        {
            _items.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public void Delete(Guid id)
        {
            _items.RemoveAll(x => x.Id == id);
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return Task.FromResult(_items.Where(x => x.TenantId == tenantId).ToList());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptions(int? tenantId)
        {
            return _items.Where(x => x.TenantId == tenantId).ToList();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId, string webhookName)
        {
            return Task.FromResult(_items.Where(x => x.TenantId == tenantId).ToList());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptions(int? tenantId, string webhookName)
        {
            return _items.Where(x => x.TenantId == tenantId).ToList();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfTenantsAsync(int?[] tenantIds)
        {
            return Task.FromResult(_items.Where(x => tenantIds.Contains(x.TenantId)).ToList());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptionsOfTenants(int?[] tenantIds)
        {
            return _items.Where(x => tenantIds.Contains(x.TenantId)).ToList();
        }

        public Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfTenantsAsync(int?[] tenantIds, string webhookName)
        {
            return Task.FromResult(_items.Where(x => tenantIds.Contains(x.TenantId)).ToList());
        }

        public List<WebhookSubscriptionInfo> GetAllSubscriptionsOfTenants(int?[] tenantIds, string webhookName)
        {
            return _items.Where(x => tenantIds.Contains(x.TenantId)).ToList();
        }

        public Task<bool> IsSubscribedAsync(int? tenantId, string webhookName)
        {
            return Task.FromResult(_items.Any(x => x.TenantId == tenantId));
        }

        public bool IsSubscribed(int? tenantId, string webhookName)
        {
            return _items.Any(x => x.TenantId == tenantId);
        }
    }
}
```

`test/Eaf.Webhooks.Tests/Fakes/TestHttpHandler.cs`:

```csharp
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Webhooks.Tests
{
    public class TestHttpHandler : HttpMessageHandler
    {
        public HttpRequestMessage LastRequest { get; private set; }
        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage(HttpStatusCode.OK);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Response);
        }
    }
}
```

- [ ] **Step 3: Criar `EafWebhooksTestModule.cs`**

```csharp
using System.Reflection;
using System.Threading.Tasks;
using Abp.Guids;
using Abp.Modules;
using Abp.TestBase;
using Abp.Webhooks;
using Castle.MicroKernel.Registration;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Eaf.Webhooks.Tests
{
    [DependsOn(
        typeof(EafWebhooksModule),
        typeof(AbpTestBaseModule)
    )]
    public class EafWebhooksTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            var options = Options.Create(new EafWebhooksOptions());

            IocManager.IocContainer.Register(
                Component.For<IOptions<EafWebhooksOptions>>().Instance(options).LifestyleSingleton()
            );

            IocManager.IocContainer.Register(
                Component.For<IWebhookSubscriptionSecretProtector>()
                    .ImplementedBy<EafPlainWebhookSecretProtector>()
                    .LifestyleTransient()
                    .IsDefault()
            );

            IocManager.IocContainer.Register(
                Component.For<IWebhookSubscriptionsStore>()
                    .ImplementedBy<InMemoryWebhookSubscriptionsStore>()
                    .LifestyleSingleton()
            );

            var definitionManager = Substitute.For<IWebhookDefinitionManager>();
            definitionManager.IsAvailable(Arg.Any<int?>(), Arg.Any<string>()).Returns(true);
            definitionManager.IsAvailableAsync(Arg.Any<int?>(), Arg.Any<string>()).Returns(Task.FromResult(true));
            IocManager.IocContainer.Register(
                Component.For<IWebhookDefinitionManager>().Instance(definitionManager).LifestyleSingleton()
            );

            var sendAttemptStore = Substitute.For<IWebhookSendAttemptStore>();
            IocManager.IocContainer.Register(
                Component.For<IWebhookSendAttemptStore>().Instance(sendAttemptStore).LifestyleSingleton()
            );

            IocManager.IocContainer.Register(
                Component.For<IUnitOfWorkManager>()
                    .ImplementedBy<FakeUnitOfWorkManager>()
                    .LifestyleSingleton()
            );

            var handler = new TestHttpHandler();
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(handler));
            IocManager.IocContainer.Register(
                Component.For<IHttpClientFactory>().Instance(httpClientFactory).LifestyleSingleton()
            );

            IocManager.IocContainer.Register(
                Component.For<IGuidGenerator>()
                    .Instance(SequentialGuidGenerator.Instance)
                    .LifestyleSingleton()
                    .IsDefault()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
```

- [ ] **Step 4: Criar `EafWebhookManagerTests.cs`**

```csharp
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    public class EafWebhookManagerTests
    {
        private readonly IWebhooksConfiguration _config;
        private readonly IWebhookSendAttemptStore _store;
        private readonly EafWebhookManager _manager;
        private readonly IWebhookSubscriptionSecretProtector _protector;

        public EafWebhookManagerTests()
        {
            _config = Substitute.For<IWebhooksConfiguration>();
            _config.JsonSerializerOptions.Returns(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            _store = Substitute.For<IWebhookSendAttemptStore>();
            _store.GetSendAttemptCountAsync(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(0));

            _protector = new EafPlainWebhookSecretProtector();
            _manager = new EafWebhookManager(_config, _store, _protector, Options.Create(new EafWebhooksOptions()));
        }

        [Fact]
        public void Dado_WebhookArgs_Quando_GetSerializedBody_Entao_RetornaPayloadNoFormatoEaf()
        {
            var args = new WebhookSenderArgs
            {
                WebhookName = "User.Created",
                Data = "{\"id\":1,\"name\":\"John\"}",
                SendExactSameData = false
            };

            var json = _manager.GetSerializedBody(args);

            json.ShouldContain("\"eventName\":\"User.Created\"");
            json.ShouldContain("\"timestamp\"");
            json.ShouldContain("\"payload\":");
        }

        [Fact]
        public void Dado_SendExactSameDataTrue_Quando_GetSerializedBody_Entao_RetornaDataOriginal()
        {
            var args = new WebhookSenderArgs
            {
                Data = "{\"raw\":true}",
                SendExactSameData = true
            };

            _manager.GetSerializedBody(args).ShouldBe("{\"raw\":true}");
        }

        [Fact]
        public void Dado_PayloadESecretPlano_Quando_SignWebhookRequest_Entao_AdicionaHeaderXEafSignature256()
        {
            var secret = "my-secret";
            var payload = "{\"eventName\":\"User.Created\",\"timestamp\":\"2026-08-14T00:00:00Z\",\"payload\":{\"id\":1}}";
            var request = new HttpRequestMessage();

            _manager.SignWebhookRequest(request, payload, _protector.Protect(secret));

            request.Content.ShouldNotBeNull();
            request.Headers.ShouldContainKey("X-Eaf-Signature-256");

            var header = request.Headers.GetValues("X-Eaf-Signature-256").Single();
            var expected = ComputeSignature(payload, secret);
            header.ShouldBe(expected);
        }

        [Fact]
        public void Dado_SegredoCifradoComDataProtection_Quando_SignWebhookRequest_Entao_AssinaComSegredoOriginal()
        {
            var options = Options.Create(new EafWebhooksOptions());
            var dpProvider = new EphemeralDataProtectionProvider();
            var dpProtector = new EafDataProtectionWebhookSecretProtector(dpProvider, options);

            var secret = "segredo-dp";
            var encrypted = dpProtector.Protect(secret);
            var payload = "{\"eventName\":\"Payment.Succeeded\"}";
            var request = new HttpRequestMessage();

            var manager = new EafWebhookManager(_config, _store, dpProtector, options);
            manager.SignWebhookRequest(request, payload, encrypted);

            var header = request.Headers.GetValues("X-Eaf-Signature-256").Single();
            var expected = ComputeSignature(payload, secret);
            header.ShouldBe(expected);
        }

        private string ComputeSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
```

- [ ] **Step 5: Criar `EafWebhookSubscriptionManagerTests.cs`**

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Guids;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    public class EafWebhookSubscriptionManagerTests
    {
        private readonly EafWebhookSubscriptionManager _manager;
        private readonly InMemoryWebhookSubscriptionsStore _store;
        private readonly IWebhookSubscriptionSecretProtector _protector;

        public EafWebhookSubscriptionManagerTests()
        {
            _store = new InMemoryWebhookSubscriptionsStore();
            _protector = new EafPlainWebhookSecretProtector();

            var definitionManager = Substitute.For<IWebhookDefinitionManager>();
            definitionManager.IsAvailable(Arg.Any<int?>(), Arg.Any<string>()).Returns(true);
            definitionManager.IsAvailableAsync(Arg.Any<int?>(), Arg.Any<string>()).Returns(Task.FromResult(true));

            _manager = new EafWebhookSubscriptionManager(
                SequentialGuidGenerator.Instance,
                definitionManager,
                _protector,
                Options.Create(new EafWebhooksOptions()))
            {
                WebhookSubscriptionsStore = _store,
                UnitOfWorkManager = new FakeUnitOfWorkManager()
            };
        }

        [Fact]
        public async Task Dado_SubscriptionValida_Quando_Adicionar_Entao_GeraESalvaSegredoProtegido()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" },
                IsActive = true
            };

            await _manager.AddOrUpdateSubscriptionAsync(subscription);

            subscription.Id.ShouldNotBe(Guid.Empty);
            subscription.Secret.ShouldNotBeNullOrWhiteSpace();
            subscription.Secret.ShouldStartWith("whs_");

            var stored = await _store.GetAsync(subscription.Id);
            stored.ShouldNotBeNull();
            stored.Secret.ShouldBe(subscription.Secret);
        }

        [Fact]
        public async Task Dado_SubscriptionComUrlHttp_Quando_AdicionarComAllowHttpFalse_Entao_LancaUserFriendlyException()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "http://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            var ex = await Should.ThrowAsync<UserFriendlyException>(() => _manager.AddOrUpdateSubscriptionAsync(subscription));
            ex.Message.ShouldContain("HTTPS");
        }

        [Fact]
        public async Task Dado_SubscriptionComUrlHttp_Quando_AdicionarComAllowHttpTrue_Entao_Cria()
        {
            var manager = new EafWebhookSubscriptionManager(
                SequentialGuidGenerator.Instance,
                Substitute.For<IWebhookDefinitionManager>(),
                _protector,
                Options.Create(new EafWebhooksOptions { AllowHttp = true }))
            {
                WebhookSubscriptionsStore = _store,
                UnitOfWorkManager = new FakeUnitOfWorkManager()
            };

            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "http://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await manager.AddOrUpdateSubscriptionAsync(subscription);
            subscription.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task Dado_DuasSubscriptionsComMesmaUrlEEEvento_Quando_Adicionar_Entao_LancaUserFriendlyException()
        {
            var s1 = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await _manager.AddOrUpdateSubscriptionAsync(s1);

            var s2 = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await Should.ThrowAsync<UserFriendlyException>(() => _manager.AddOrUpdateSubscriptionAsync(s2));
        }

        [Fact]
        public async Task Dado_SubscriptionAtiva_Quando_DesativarViaUpdate_Entao_IsActivePersistido()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await _manager.AddOrUpdateSubscriptionAsync(subscription);

            subscription.IsActive = false;
            await _manager.AddOrUpdateSubscriptionAsync(subscription);

            var stored = await _store.GetAsync(subscription.Id);
            stored.IsActive.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_SegredoNaoAlterado_Quando_Atualizar_Entao_MantemSegredoOriginal()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await _manager.AddOrUpdateSubscriptionAsync(subscription);
            var originalSecret = subscription.Secret;

            subscription.IsActive = false;
            await _manager.AddOrUpdateSubscriptionAsync(subscription);

            var stored = await _store.GetAsync(subscription.Id);
            stored.Secret.ShouldBe(originalSecret);
        }

        [Fact]
        public async Task Dado_NovoSegredoPlano_Quando_Atualizar_Entao_ProtegeNovoSegredo()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "User.Created" }
            };

            await _manager.AddOrUpdateSubscriptionAsync(subscription);
            var originalSecret = subscription.Secret;

            subscription.Secret = "whs_mynewsecret";
            await _manager.AddOrUpdateSubscriptionAsync(subscription);

            var stored = await _store.GetAsync(subscription.Id);
            stored.Secret.ShouldNotBe(originalSecret);
            stored.Secret.ShouldBe(_protector.Protect("whs_mynewsecret"));
        }
    }
}
```

- [ ] **Step 6: Criar `EafWebhookSenderTests.cs`**

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.Webhook;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    public class EafWebhookSenderTests
    {
        private readonly IWebhooksConfiguration _config;
        private readonly IWebhookManager _webhookManager;
        private readonly TestHttpHandler _handler;
        private readonly IHttpClientFactory _clientFactory;

        public EafWebhookSenderTests()
        {
            _config = Substitute.For<IWebhooksConfiguration>();
            _config.TimeoutDuration.Returns(TimeSpan.FromSeconds(30));
            _config.MaxSendAttemptCount.Returns(5);

            _webhookManager = Substitute.For<IWebhookManager>();
            _handler = new TestHttpHandler();
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _clientFactory.CreateClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName).Returns(new HttpClient(_handler));
        }

        [Fact]
        public async Task Dado_UrlHttpQuandoAllowHttpFalse_Quando_SendWebhookAsync_Entao_LancaUserFriendlyException()
        {
            var sender = new EafWebhookSender(_config, _webhookManager, _clientFactory, Options.Create(new EafWebhooksOptions()));
            var args = new WebhookSenderArgs
            {
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                WebhookUri = "http://example.com/webhook"
            };

            var ex = await Should.ThrowAsync<UserFriendlyException>(() => sender.SendWebhookAsync(args));
            ex.Message.ShouldContain("HTTPS");
            _handler.LastRequest.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UrlHttps_Quando_SendWebhookAsync_Entao_EnviaRequestComCabecalhos()
        {
            var sender = new EafWebhookSender(_config, _webhookManager, _clientFactory, Options.Create(new EafWebhooksOptions()));
            var args = new WebhookSenderArgs
            {
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                WebhookUri = "https://example.com/webhook",
                Headers = new Dictionary<string, string> { { "X-Custom", "value" } },
                Secret = "secret"
            };

            _webhookManager.InsertAndGetIdWebhookSendAttemptAsync(args).Returns(Guid.NewGuid());
            _webhookManager.GetSerializedBodyAsync(args).Returns("{}");
            _webhookManager.When(x => x.SignWebhookRequest(Arg.Any<HttpRequestMessage>(), "{}", "secret"))
                .Do(x => x.Arg<HttpRequestMessage>().Content = new StringContent("{}", Encoding.UTF8, "application/json"));

            var id = await sender.SendWebhookAsync(args);

            id.ShouldNotBe(Guid.Empty);
            _handler.LastRequest.ShouldNotBeNull();
            _handler.LastRequest.RequestUri.ToString().ShouldBe("https://example.com/webhook");
        }

        [Fact]
        public async Task Dado_Resposta500_Quando_SendWebhookAsync_Entao_LancaExcecao()
        {
            var sender = new EafWebhookSender(_config, _webhookManager, _clientFactory, Options.Create(new EafWebhooksOptions()));
            var args = new WebhookSenderArgs
            {
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                WebhookUri = "https://example.com/webhook",
                Secret = "secret"
            };

            _webhookManager.InsertAndGetIdWebhookSendAttemptAsync(args).Returns(Guid.NewGuid());
            _webhookManager.GetSerializedBodyAsync(args).Returns("{}");
            _webhookManager.When(x => x.SignWebhookRequest(Arg.Any<HttpRequestMessage>(), "{}", "secret"))
                .Do(x => x.Arg<HttpRequestMessage>().Content = new StringContent("{}", Encoding.UTF8, "application/json"));

            _handler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("fail") };

            await Should.ThrowAsync<Exception>(() => sender.SendWebhookAsync(args));
        }
    }
}
```

- [ ] **Step 7: Criar `EafWebhooksModuleTests.cs`**

```csharp
using Abp.TestBase;
using Abp.Webhooks;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    public class EafWebhooksModuleTests : AbpIntegratedTestBase<EafWebhooksTestModule>
    {
        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverServicos_Entao_UsaImplementacoesEaf()
        {
            var webhookManager = Resolve<IWebhookManager>();
            webhookManager.ShouldBeOfType<EafWebhookManager>();

            var subscriptionManager = Resolve<IWebhookSubscriptionManager>();
            subscriptionManager.ShouldBeOfType<EafWebhookSubscriptionManager>();

            var sender = Resolve<IWebhookSender>();
            sender.ShouldBeOfType<EafWebhookSender>();
        }
    }
}
```

- [ ] **Step 8: Build de testes**

```bash
dotnet build test/Eaf.Webhooks.Tests/Eaf.Webhooks.Tests.csproj --configuration Release
```

- [ ] **Step 9: Commit**

```bash
git add test/Eaf.Webhooks.Tests
git commit -m "test(webhooks): add Eaf.Webhooks test project with manager, sender and module tests"
```

---

## Task 10: Adicionar projetos à solução e atualizar índice de specs

**Files:**
- Modify: `Eaf.sln`
- Modify: `.specs/eaf-implementation-plan-q3-2026.spec.md`

- [ ] **Step 1: Adicionar projetos à solução**

```bash
dotnet sln add src/Eaf.Webhooks/Eaf.Webhooks.csproj
dotnet sln add test/Eaf.Webhooks.Tests/Eaf.Webhooks.Tests.csproj
```

- [ ] **Step 2: Atualizar status no plano Q3**

Em `.specs/eaf-implementation-plan-q3-2026.spec.md`, na tabela Wave 2, alterar:

```markdown
| 5 | `Eaf.Webhooks` | P2 | `eaf-module-webhooks.spec.md` | In progress | Outgoing webhooks; depends on events/jobs |
```

- [ ] **Step 3: Commit**

```bash
git add Eaf.sln .specs/eaf-implementation-plan-q3-2026.spec.md
git commit -m "chore(sln): add Eaf.Webhooks and test projects, update Q3 plan status"
```

---

## Task 11: Verificação final

**Files:** todos

- [ ] **Step 1: Build da solução**

```bash
dotnet build Eaf.sln --configuration Release
```

Expected: 0 erros, 0 warnings.

- [ ] **Step 2: Testes do novo módulo**

```bash
dotnet test test/Eaf.Webhooks.Tests --configuration Release --no-build
```

Expected: todos passam.

- [ ] **Step 3: Regressão completa**

```bash
dotnet test Eaf.sln --configuration Release --no-build
```

Expected: todos os testes existentes continuam passando.

- [ ] **Step 4: Commit final se necessário**

Se houver ajustes:

```bash
git commit -m "fix(webhooks): final adjustments after build and test"
```

---

## Configuração sugerida em `appsettings.json`

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

---

## Riscos e mitigação

| Risco | Mitigação |
|---|---|
| `EafWebhookSubscriptionManager` não manter `IsActive`/`Secret` em updates | Override de `AddOrUpdateSubscriptionAsync`/`AddOrUpdateSubscription` persiste ambos |
| Segredo duplamente criptografado ao reenviar | Update branch compara `webhookSubscription.Secret` com `info.Secret` e só protege se for novo |
| `IDataProtectionProvider` não disponível em testes | `EafPlainWebhookSecretProtector` como fallback no factory do módulo; `EafWebhooksTestModule` registra plain explicitamente |
| URLs HTTP vazando em produção | `AllowHttp` default false + validação em `EafWebhookSubscriptionManager` e `EafWebhookSender` |
| Conflito de DI com implementações ABP | Registros explicitamente marcados como `IsDefault()` no `EafWebhooksModule.Initialize` |
| Cobertura diminuir | Incluir testes de manager, sender, proteção e ciclo de vida do módulo |

---

## Branch sugerida

`feature/eaf-webhooks`
