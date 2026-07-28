# Otimizações de Performance e Memória

Este documento descreve as melhorias aplicadas no EAF para reduzir alocações, evitar vazamentos de memória e diminuir a materialização de dados em memória.

## Rate limiting (`RateLimitManager`)

- Substituição do dicionário estático de `SemaphoreSlim` por `SemaphoreHolder` com carimbo de último uso.
- Timer de limpeza periódica remove entradas ociosas e dispara `Dispose()` dos semáforos, evitando crescimento ilimitado do dicionário.
- `CanCleanup` considera `2 × window` (limitado entre 1 min e 1 h) e só remove semáforos livres (`CurrentCount == MaxConcurrency`).

## Leitura de logs (`WebLogAppService`)

- `GetLatestWebLogs` não carrega mais o arquivo inteiro com `File.ReadLines(...).Reverse()`.
- Novo leitor `ReadTailLines` posiciona o stream próximo ao final do arquivo, lê no máximo 1 MB e descarta a primeira linha truncada.
- Redução de alocação: de ~19,85 MB para ~2,53 MB por requisição (13× menor).
- Filtro de níveis de log usa `StringComparison.Ordinal`.

## Chat (`ChatAppService`)

- `MarkUserMessagesAsReadAsync` e `MarkGroupMessagesAsReadAsync` não carregam todas as mensagens não lidas de uma só vez.
- Novo helper `MarkMessagesAsReadInBatchesAsync` processa em lotes de 1.000 registros, chamando `ChangeReadState` / `ChangeReceiverReadState` e deixando o Unit of Work persistir as alterações.
- Adicionado filtro `ReceiverReadState == Unread` na consulta reversa de mensagens de usuário, evitando reprocessamento desnecessário.

## Cache SQL Server (`EafSqlServerCache`)

- `ObjectToByteArray` foi reescrito com `ArrayBufferWriter<byte>` e `Utf8JsonWriter`, eliminando a alocação intermediária de `jsonBytes` e o `Buffer.BlockCopy` final.
- A serialização continua prefixada com o nome do tipo e `\n`, mantendo compatibilidade com dados já armazenados.

## Autenticação (`TokenAuthController`)

- Removido bloco duplicado de `_userManager.UpdateAsync(user)` / `SaveChangesAsync()` em `CreateJwtClaims`.
- A atualização do usuário é feita uma única vez, reduzindo roundtrips ao banco.

## E-mail (`UserEmailer`)

- Construção de links com `StringBuilder.Replace` em vez de `string.Replace` encadeado, evitando strings intermediárias.
- Corpo do e-mail montado com `Append(...).AppendLine(...)` sem concatenação interna, reduzindo alocações no `StringBuilder`.

## Filtros de auditoria (`AuditLogAppService`)

- `CreateAuditLogAndUsersQuery` normaliza os filtros em variáveis locais e usa `ToLower()` no lado da entidade (traduzível pelo EF Core).
- `GetEntityTypeChanges` aplica o filtro de `EntityTypeFullName` / `EntityId` antes do join, reduzindo o conjunto de dados processado.
- Adicionado `item.User != null` no filtro de `UserName` para evitar `NullReferenceException`.

## Benchmarks

O projeto `test/Eaf.Middleware.Application.Benchmarks` (BenchmarkDotNet com `MemoryDiagnoser`) mede os ganhos:

| Cenário | Legado | Otimizado | Razão tempo | Razão memória |
|---------|--------|-----------|-------------|---------------|
| Leitura de cauda de logs (100k linhas) | 9.689 ms | 0.729 ms | 0,08× | 0,13× |

Para executar:

```bash
dotnet run --project test/Eaf.Middleware.Application.Benchmarks -c Release
```

## Validação

- `dotnet test Eaf.sln -c Release` passa (exceto teste pré-existente de infraestrutura Hangfire).
- `docker compose -f docker-compose.all.yml up --build -d` sobe API, Worker, Angular e SQL Server.
- Testes reais via `curl` validam login host/tenant, criação de tenant, CORS preflight, SignalR negotiate e public error contract.
