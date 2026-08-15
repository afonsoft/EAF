# Eaf.Notifications.Sms

Módulo de envio de SMS para o Enterprise Application Foundation (EAF). Fornece abstrações e providers configuráveis para gateways como Zenvia, Twilio, AWS SNS e qualquer provedor HTTP genérico.

## Dependências

- `Abp` (>= 10.5.0)
- `Microsoft.Extensions.Http` (>= 10.0.9)

## Configuração

```json
{
  "Eaf": {
    "Sms": {
      "Provider": "GenericHttp",
      "DefaultFrom": "EAF",
      "GenericHttp": {
        "BaseUrl": "https://api.zenvia.com",
        "Endpoint": "/services/send-sms",
        "AuthenticationType": "Basic",
        "Username": "zenvia-user",
        "Password": "zenvia-pass",
        "ContentType": "Json",
        "Template": "{\"sendSmsRequest\":{\"from\":\"{{from}}\",\"to\":\"{{phoneNumber}}\",\"msg\":\"{{body}}\"}}"
      }
    }
  }
}
```

Exemplo para Twilio:

```json
{
  "Eaf": {
    "Sms": {
      "Provider": "Twilio",
      "DefaultFrom": "+15551234567",
      "Twilio": {
        "AccountSid": "AC...",
        "AuthToken": "...",
        "From": "+15551234567"
      }
    }
  }
}
```

## Uso

```csharp
public class MyAppService : ApplicationService
{
    private readonly ISmsSender _smsSender;

    public MyAppService(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }

    public async Task SendAsync()
    {
        await _smsSender.SendAsync(new SmsMessage
        {
            PhoneNumber = "+5511987654321",
            Body = "Código de verificação: 123456"
        });
    }
}
```

## Providers

- `GenericHttpSmsProvider` — provider genérico para APIs REST de SMS (Zenvia, customizadas etc.).
- `TwilioSmsProvider` — provider nativo para a API REST do Twilio usando HTTP básico.

## Extensão

Implemente `ISmsProvider` e registre-o no `IocManager` para adicionar novos gateways:

```csharp
public class MyProvider : ISmsProvider
{
    public string Name => "MyProvider";

    public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken)
    {
        // implementação
    }
}
```

## Testes

Execute os testes do módulo com:

```bash
dotnet test test/Eaf.Notifications.Sms.Tests --configuration Release
```
