# Contratos EAF para consumidores realtime e sociais

## Escopo

Este guia registra a evolução necessária no EAF para consumidores como o
GameHub. Ele não autoriza alterações incompatíveis, não cria uma segunda
persistência no consumidor e não transforma o GameHub em owner de chat,
notificações, amizades, block, mute ou auditoria.

O EAF continua sendo a fonte de verdade para identidade, tenant, autorização,
cache, persistência, entrega SignalR e auditoria. O consumidor pode manter uma
fachada SDK própria, desde que a fachada delegue para os contratos EAF e não
replique entidades ou tabelas.

## Matriz de ownership

| Capacidade | Fonte de verdade | Fachada permitida no consumidor |
| --- | --- | --- |
| Identidade e tenant | EAF/ABP | DTOs próprios sem claims internas |
| Chat e histórico | EAF | Cliente SDK compatível |
| Notificações | EAF | DTOs públicos versionados |
| Amizade, block e mute | EAF | Operações delegadas |
| Rate limit | EAF | Política específica do consumidor |
| Auditoria de moderação | EAF | Consulta administrativa agregada |
| Presença e entrega realtime | EAF/SignalR | Estado coarse-grained |

## Versionamento e compatibilidade

1. Campos de contexto são opcionais no contrato atual.
2. Clientes antigos continuam enviando e recebendo mensagens sem
   `ConversationId`, `GameId`, `MatchId` ou `ContextType`.
3. Mudanças incompatíveis exigem novo contrato ou versão de endpoint.
4. O consumidor deve consultar capabilities/feature flags antes de exibir uma
   funcionalidade opcional.
5. `service-proxies.ts` é gerado pelo build e nunca deve ser editado manualmente.

Uma capacidade deve ser exposta com um identificador estável, por exemplo:

```json
{
  "chat.contextual": true,
  "chat.markRead": true,
  "social.mute": false,
  "notifications.metadataVersion": 1,
  "realtime.backplane": true
}
```

## Chat contextual

### Modelo público

O contrato de mensagem deve evoluir por extensão, sem criar uma segunda
entidade:

```csharp
public class ChatMessage
{
    public Guid? ConversationId { get; set; }
    public Guid? GameId { get; set; }
    public Guid? MatchId { get; set; }
    public string ContextType { get; set; }
}
```

`MatchId` é opcional. `ContextType` deve ser uma string versionável, como
`direct`, `game` ou `match`; valores desconhecidos devem ser tratados como
contexto não suportado, nunca como autorização implícita.

### Histórico e leitura

O contrato de aplicação deve manter uma única entidade de mensagem e oferecer:

```csharp
Task<ListResultDto<ChatMessageDto>> GetHistoryAsync(
    GetChatHistoryInput input);

Task MarkReadAsync(
    MarkChatReadInput input);
```

`GetChatHistoryInput` deve aceitar tenant, jogo, partida, conversa, período,
cursor/página e tamanho máximo. A ordenação deve ser estável por timestamp UTC
e identificador. `MarkReadAsync` deve ser idempotente e validar o usuário
autenticado e a autorização da conversa.

Índices compostos devem acompanhar os filtros efetivamente usados, sem
permitir enumeração entre tenants. O endpoint e o hub `/signalr-chat` devem
continuar disponíveis durante a migração.

### Segurança e privacidade

- A identidade vem da sessão/token, nunca de um campo enviado pelo iframe.
- `TenantId`, `GameId` e `MatchId` são filtros de autorização, não apenas filtros
  de consulta.
- O contrato público não expõe e-mail, claims internas, IP ou connection ID.
- Mensagens têm limite de tamanho, normalização e política de retenção.
- `clientMessageId` pode ser usado para idempotência antes da persistência.
- Usuários bloqueados devem ser filtrados antes da persistência e da entrega.

## Notificações

Consumidores registram definições e payloads no `INotificationPublisher` do EAF:

```csharp
public interface INotificationPublisher
{
    Task PublishAsync(
        string notificationName,
        NotificationData data,
        UserIdentifier[] userIds);
}
```

O payload deve ser JSON versionado e conter somente metadata pública, como
`GameId`, `MatchId`, `InviteId`, severidade e expiração. A entrega para usuário
online usa SignalR; usuário offline recebe o fallback persistido pelo EAF.
Leitura individual e em lote deve respeitar tenant, usuário e autorização.

O GameHub pode conservar DTOs públicos como fachada de compatibilidade, mas não
deve manter uma tabela social paralela para notificações EAF.

## Amizade, block e mute

Operações de consumidor devem delegar aos serviços EAF:

```csharp
Task BlockUser(BlockUserInput input);
Task UnblockUser(UnblockUserInput input);
Task<ListResultDto<FriendshipDto>> GetFriends(GetFriendsInput input);
Task MuteUser(MuteUserInput input);
Task UnmuteUser(UnmuteUserInput input);
```

Block, mute e amizade são relações distintas. A precedência recomendada para
entrega é:

1. block impede interação e entrega;
2. mute impede a entrega de notificações/mensagens no escopo definido;
3. amizade concede somente as capacidades explicitamente autorizadas.

Block/unblock/mute/unmute são idempotentes, tenant-aware e emitem evento para
invalidação de cache. Mute pode possuir expiração e motivo. O contrato deve
deixar explícito se a relação vale para host, tenant ou ambos.

## Rate limit compartilhado

O EAF deve fornecer uma abstração comum, com chave tenant-aware e sem payload
privado:

```csharp
public interface IRateLimitManager
{
    Task<RateLimitDecision> CheckAsync(
        string policy,
        string subject,
        TimeSpan window,
        int limit,
        CancellationToken cancellationToken = default);
}
```

`RateLimitDecision` informa apenas `Allowed`, `Limit`, `Current`,
`RetryAfterSeconds` e `Policy`. Redis deve usar operação atômica. Providers não
distribuídos devem declarar o fallback operacional e não podem ser apresentados
como proteção global. A idempotency key deve ser avaliada antes da contagem.

Não usar `ICacheManager` como fila, lock distribuído ou Pub/Sub. Cache, storage
do Hangfire e backplane SignalR podem compartilhar Redis, mas precisam de
prefixos, TTLs, métricas e políticas de falha separados.

## Auditoria de moderação

Decisões de moderação, block e reports devem usar um writer uniforme:

```csharp
public interface IModerationAuditWriter
{
    Task WriteAsync(ModerationAuditEntry entry);
}
```

`ModerationAuditEntry` contém tenant, usuário executor, ação, alvo
anonimizado, motivo, decisão, correlation ID, data UTC e referências opcionais
para jogo, partida e report. A consulta administrativa deve paginar e filtrar
por ação, usuário, período e tenant, usando as permissões EAF/ABP. Logs e
respostas não podem vazar o alvo bruto ou PII.

## Erros públicos e observabilidade

O envelope público é:

```json
{
  "code": "rate_limited",
  "message": "Tente novamente mais tarde.",
  "retryable": true,
  "correlationId": "..."
}
```

Códigos estáveis: `not_authenticated`, `not_authorized`,
`feature_disabled`, `rate_limited`, `invalid_context`,
`temporarily_unavailable` e `validation_failed`. Stack trace e detalhes
internos ficam somente nos logs estruturados.

Métricas mínimas: mensagens enviadas/bloqueadas, reports abertos/resolvidos,
notificações publicadas/entregues, block/mute, rate limits, falhas de backplane
e latência de cache/persistência. Logs devem mascarar payload de chat e
identificadores conforme a política de retenção do ambiente.

## SignalR e execução em múltiplas instâncias

- `/signalr-chat` permanece estável.
- `ChannelPrefix` é obrigatório e deve incluir aplicação e ambiente.
- Redis indisponível deve produzir estado degradado explícito; não deve
  converter Pub/Sub em cache local silencioso.
- Health checks devem separar banco, cache e Pub/Sub.
- Data Protection Keys devem ser compartilhadas em produção.
- Shutdown deve parar novas operações, drenar conexões e encerrar Hangfire
  graciosamente.
- A compatibilidade de versões deve ser testada com instâncias antigas e novas.

Runbook mínimo:

1. iniciar duas instâncias com o mesmo tenant e `ChannelPrefix`;
2. conectar dois clientes em instâncias diferentes;
3. validar grupo de partida, presença coarse-grained, notificação e reconexão;
4. desligar Redis e confirmar health status degradado e erro público estável;
5. reativar Redis e confirmar recuperação sem duplicação de mensagens.

## Configuração dos templates

### API

`AddEafConfigurer`, `AddEafHealthChecks`, `AddEafOpenTelemetry` e o mapeamento
de `ChatHub` continuam sendo os pontos centrais do template. Os métodos
esperados para uma futura API de conveniência são:

```csharp
public static IServiceCollection AddEafRealtime(
    this IServiceCollection services,
    IConfiguration configuration);

public static IServiceCollection AddEafObservability(
    this IServiceCollection services,
    IConfiguration configuration);
```

Eles devem ser idempotentes e delegar aos módulos EAF; não podem registrar uma
segunda implementação de cache, chat ou notificações. Até a disponibilização
dessas extensões no módulo, o template deve manter as chamadas existentes e
documentar a composição manual.

### Angular

O template deve consumir `EafError`, preservar o interceptor de autenticação e
correlation ID, delegar notificações/chat/block/mute aos proxies gerados e
repetir somente falhas transitórias. Estados loading, empty, error e retry
devem ser acessíveis, com foco visível, teclado e `aria-live`.

## Rollout e migration guide

1. Publicar contratos opcionais e capabilities.
2. Adicionar metadata de chat sem tornar colunas obrigatórias.
3. Criar índices em rollout separado e medir latência.
4. Ativar histórico contextual e mark-read por feature flag.
5. Migrar notificações sociais para o publisher EAF.
6. Ativar block/mute integrado ao filtro de entrega.
7. Ativar rate limit distribuído e observar fallback.
8. Habilitar auditoria, métricas e backplane em duas instâncias.
9. Remover duplicações do consumidor somente após confirmar paridade.

Rollback deve desativar as capabilities novas sem remover imediatamente os
campos opcionais. Migrações de schema devem ser reversíveis e compatíveis com
uma instância anterior durante a janela de rollout.

## Checklist de aceite

- [ ] Nenhuma persistência paralela de chat ou notificações no consumidor.
- [ ] Clientes EAF antigos continuam compilando e funcionando.
- [ ] Contratos novos são opcionais ou versionados.
- [ ] Tenant, usuário, jogo e partida são validados na autorização.
- [ ] Nenhum payload público expõe PII, claims ou connection ID.
- [ ] Testes cobrem autorização, concorrência, expiração, idempotência e Redis.
- [ ] Templates documentam EAF versus código específico do consumidor.
- [ ] Runbook de duas instâncias e migration guide estão publicados.
