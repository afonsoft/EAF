# EAF (Enterprise Application Foundation) — System Architecture

> Arquitetura de alto nível do ecossistema EAF, abrangendo a camada de apresentação, módulos de middleware baseados em ASP.NET Boilerplate (ABP) .NET 10.0, e infraestrutura de persistência e serviços corporativos.

## 1. Visão Geral da Arquitetura (C4 / Componentes)

```mermaid
graph TB
    subgraph Presentation ["🌐 Presentation & Frontend Layer"]
        UI["👤 Angular 18 Admin UI / PrimeNG"]
        API["🌐 Eaf.Web.Host / REST API"]
        GW["🚪 Eaf.Gateways.API"]
    end

    subgraph Middleware ["⚙️ EAF Middleware Modules (.NET 10.0 / ABP 10.5.0)"]
        CORE["🧩 Eaf.Middleware.Core"]
        APP["📦 Eaf.Middleware.Application"]
        KV["🔑 Eaf.KeyVault / KeyVault.AspNetCore"]
        OTEL["📊 Eaf.OpenTelemetry"]
        CACHE["⚡ SqlServerCache / SqliteCache"]
        JOB["⚙️ Worker / Background Jobs (Hangfire)"]
        WH["📨 Webhooks & ServiceBus"]
    end

    subgraph Domain ["🏛️ Domain & Application Layer"]
        AS["🚀 Application Services & DTOs"]
        DS["⚙️ Domain Services"]
        AGG["📦 Aggregate Roots & Entities"]
    end

    subgraph Infrastructure ["💾 Infrastructure & Persistence Layer"]
        EF["🗄️ Entity Framework Core 10.0"]
        DB[("💾 SQL Server / SQLite Database")]
        REDIS[("⚡ Distributed Cache / Redis")]
        VAULT["🔐 Azure KeyVault / OCI Vault"]
    end

    UI -->|HTTP / SignalR| API
    UI -->|API Gateway| GW
    GW --> API
    
    API --> CORE
    API --> APP
    API --> KV
    API --> OTEL
    API --> CACHE
    API --> JOB
    API --> WH

    API --> AS
    AS --> DS
    DS --> AGG
    AGG --> EF
    EF --> DB
    
    CACHE --> REDIS
    KV --> VAULT

    classDef presentation fill:#90EE90,stroke:#333,stroke-width:2px,color:darkgreen
    classDef middleware fill:#87CEEB,stroke:#333,stroke-width:2px,color:darkblue
    classDef domain fill:#FFF2CC,stroke:#333,stroke-width:2px,color:black
    classDef infra fill:#E6E6FA,stroke:#333,stroke-width:2px,color:darkblue

    class UI,API,GW presentation
    class CORE,APP,KV,OTEL,CACHE,JOB,WH middleware
    class AS,DS,AGG domain
    class EF,DB,REDIS,VAULT infra
```

## 2. Fluxo de Requisição e Autenticação (Sequence)

```mermaid
sequenceDiagram
    autonumber
    actor User as 👤 Usuário / Cliente
    participant UI as 🌐 Angular UI
    participant API as 🚪 Eaf.Web.Host (API)
    participant Auth as 🔐 ABP Authorization & JWT
    participant KV as 🔑 Eaf.KeyVault / Secrets
    participant DB as 💾 SQL Server

    User->>UI: Acessa funcionalidade / login
    UI->>API: POST /api/tokenauth/authenticate
    API->>Auth: Valida credenciais & tenant
    Auth->>DB: Consulta usuário / permissões
    DB-->>Auth: Retorna dados do usuário
    Auth-->>API: Emite JWT Token + Claims
    API-->>UI: Retorna Token de Acesso
    
    Note over UI,API: Requisições subsequentes com Bearer Token
    User->>UI: Executa operação protegida
    UI->>API: GET /api/services/app/... (Bearer JWT)
    API->>Auth: Valida Token & Permissões [Authorize]
    API->>KV: Recupera segredos se necessário
    KV-->>API: Retorna chaves / conexões
    API->>DB: Consulta / Persiste dados via EF Core
    DB-->>API: Dados atualizados
    API-->>UI: Retorna DTO de Resposta
    UI-->>User: Renderiza interface atualizada
```

## 3. Módulos de Middleware EAF

O EAF fornece 14 módulos reutilizáveis organizados em camadas modulares:
1. **Eaf.Middleware.Core**: Núcleo corporativo, exceções de negócio, tempo e extensões base.
2. **Eaf.Middleware.Application**: Serviços de aplicação base, validações e DTOs padronizados.
3. **Eaf.Middleware.Web.Core**: Filtros globais, tratamento de erros HTTP e integração com ASP.NET Core.
4. **Eaf.KeyVault & KeyVault.AspNetCore**: Gerenciamento integrado de segredos (Azure KeyVault, OCI Vault).
5. **Eaf.OpenTelemetry**: Observabilidade distribuída com métricas, traces e PII redaction.
6. **Eaf.SqlServerCache / SqliteCache**: Cache distribuído de alta performance.
7. **Eaf.Worker**: Processamento de tarefas em segundo plano com Hangfire.
8. **Eaf.Log4NetServiceBus**: Log estruturado e mensageria integrada.
