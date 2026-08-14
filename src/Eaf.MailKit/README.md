# Eaf.MailKit

Módulo EAF para envio de e-mails baseado em [MailKit](https://github.com/jstedfast/MailKit), estendendo as abstrações `Abp.MailKit` com retry, templates e configurações específicas do EAF.

## Funcionalidades

- Envio de e-mails via `IEmailSender` (MailKit).
- Retry automático em falhas transitórias do SMTP (até 3 tentativas com backoff exponencial).
- Templates de e-mail com placeholders `{{Nome}}` e fallback por tenant.
- Configurações de certificado SSL/TLS e validação desabilitada para desenvolvimento.
- Spans do `ActivitySource` "Eaf.MailKit" para observabilidade (OpenTelemetry).

## Instalação

Adicione a dependência ao módulo ABP:

```csharp
[DependsOn(typeof(EafMailKitModule))]
public class MyProjectModule : AbpModule
{
}
```

## Configuração

As configurações podem ser definidas via `ISettingManager` ou injetando `EafMailKitConfiguration`:

| Setting | Padrão | Descrição |
|---|---|---|
| `Eaf.MailKit.RetryCount` | `3` | Número máximo de tentativas. |
| `Eaf.MailKit.RetryDelayMilliseconds` | `500` | Tempo base entre tentativas (ms). |
| `Eaf.MailKit.DisableCertificateValidation` | `false` | Desabilita validação do certificado SMTP. |

Configure as credenciais SMTP padrão do ABP (`Abp.Net.Mail.EmailSettingNames.Smtp.Host`, `Port`, `UserName`, `Password` etc.).

## Uso

```csharp
public class MyService
{
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateManager _templateManager;

    public MyService(IEmailSender emailSender, IEmailTemplateManager templateManager)
    {
        _emailSender = emailSender;
        _templateManager = templateManager;
    }

    public async Task SendWelcomeEmail(string email, string name)
    {
        var body = await _templateManager.RenderAsync("Welcome", new { Name = name });
        await _emailSender.SendAsync(email, "Bem-vindo", body, isBodyHtml: true);
    }
}
```

## Templates

Armazene templates em memória (padrão) ou substitua `IEmailTemplateStore` por uma implementação que use banco de dados:

```csharp
var store = Resolve<IEmailTemplateStore>();
```

O formato dos placeholders é `{{Propriedade}}`. Propriedades ausentes são substituídas por vazio.

## E-mail com anexos

```csharp
using var message = new MailMessage("from@example.com", "to@example.com", "Subject", "<h1>Body</h1>")
{
    IsBodyHtml = true
};
message.Attachments.Add(new Attachment("caminho/para/arquivo.pdf"));
await _emailSender.SendAsync(message);
```

---

# Eaf.MailKit (English)

EAF module for email sending based on [MailKit](https://github.com/jstedfast/MailKit), extending `Abp.MailKit` abstractions with retry, templates, and EAF-specific configuration.

## Features

- Email sending via `IEmailSender` (MailKit).
- Automatic retry on transient SMTP failures (up to 3 attempts with exponential backoff).
- Email templates with `{{Name}}` placeholders and tenant fallback.
- SSL/TLS and certificate validation settings.
- `ActivitySource` "Eaf.MailKit" spans for observability (OpenTelemetry).

## Configuration

| Setting | Default | Description |
|---|---|---|
| `Eaf.MailKit.RetryCount` | `3` | Maximum retry attempts. |
| `Eaf.MailKit.RetryDelayMilliseconds` | `500` | Base delay between retries (ms). |
| `Eaf.MailKit.DisableCertificateValidation` | `false` | Disables SMTP certificate validation. |

Also configure ABP SMTP settings (`Abp.Net.Mail.EmailSettingNames.Smtp.*`).
