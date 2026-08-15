# Eaf.Notifications.Push

Módulo de envio de notificações push para o Enterprise Application Foundation (EAF). Suporta Web Push (VAPID) e providers HTTP genéricos para gateways como Zenvia, Firebase, OneSignal etc.

## Dependências

- `Abp` (>= 10.5.0)
- `Microsoft.Extensions.Http` (>= 10.0.9)
- `WebPush` (>= 1.0.12)

## Configuração

Exemplo Web Push:

```json
{
  "Eaf": {
    "Push": {
      "Provider": "WebPush",
      "WebPush": {
        "PublicKey": "B...",
        "PrivateKey": "...",
        "Subject": "mailto:admin@example.com"
      }
    }
  }
}
```

Exemplo genérico HTTP (Zenvia ou outro gateway):

```json
{
  "Eaf": {
    "Push": {
      "Provider": "GenericHttp",
      "GenericHttp": {
        "BaseUrl": "https://api.zenvia.com",
        "Endpoint": "/services/push",
        "AuthenticationType": "Bearer",
        "Token": "...",
        "ContentType": "Json",
        "Template": "{\"to\":\"{{endpoint}}\",\"title\":\"{{title}}\",\"body\":\"{{body}}\"}"
      }
    }
  }
}
```

## Uso

```csharp
public class MyAppService : ApplicationService
{
    private readonly IPushNotificationSender _pushSender;

    public MyAppService(IPushNotificationSender pushSender)
    {
        _pushSender = pushSender;
    }

    public async Task SendAsync(PushSubscription subscription)
    {
        await _pushSender.SendAsync(subscription, new PushNotificationMessage
        {
            Title = "Alerta",
            Body = "Você tem uma nova mensagem.",
            Icon = "/assets/icon.png"
        });
    }
}
```

## Providers

- `WebPushNotificationProvider` — envio via Web Push (VAPID).
- `GenericHttpPushProvider` — envio através de qualquer API HTTP.

## Extensão

Implemente `IPushNotificationProvider` e registre-o no `IocManager` para adicionar novos gateways.
