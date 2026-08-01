---
name: 'C# EAF Coding Standards'
description: 'Padrões de código C# para o projeto EAF, incluindo ABP patterns, Castle Windsor DI, e convenções .NET 10'
paths:
  - '**/*.cs'
---

# C# EAF Coding Standards

## Arquitetura ABP

- Seguir N-Layer: Domain → Application → Infrastructure → Presentation
- Módulos ABP devem herdar de `EafModule` (ou `AbpModule`)
- Declarar dependências com `[DependsOn(typeof(...))]`
- Lifecycle: `PreInitialize()` → `Initialize()` → `PostInitialize()` → `Shutdown()`

## Dependency Injection

- Usar Castle Windsor (via ABP)
- Injeção via construtor — nunca usar `new` para serviços
- Registrar em `Initialize()` do módulo
- Convenção: `IMyService` → `MyService` (auto-registered)

## Async/Await

- Usar `async/await` para toda operação I/O
- Sufixo `Async` em métodos assíncronos
- Nunca usar `.Result` ou `.Wait()` — causa deadlocks
- Usar `ConfigureAwait(false)` em bibliotecas

## Documentação XML

- Obrigatório em todas as APIs públicas (`///`)
- `<summary>`, `<param>`, `<returns>`, `<exception>`
- Idioma: português (pt-BR)

## Nomenclatura

- PascalCase: classes, métodos, propriedades
- camelCase: variáveis locais, parâmetros
- _camelCase: campos privados
- Interfaces: prefixo `I` (ex: `IUserService`)
- DTOs: sufixo `Dto` (ex: `UserDto`)
- AppServices: sufixo `AppService` (ex: `UserAppService`)

## Testes

- Framework: xUnit + Shouldly + NSubstitute
- BDD em português: `Dado_Quando_Entao` ou `[Fact] // Dado X, Quando Y, Então Z`
- Uma assertion por teste quando possível
- Cobertura mínima: 90%

## Entity Framework Core

- Usar migrations para alterações de schema
- `DbContext` registrado no módulo
- Soft-delete via `ISoftDelete`
- Multi-tenancy via `IMayHaveTenant` / `IMustHaveTenant`

## Segurança

- Nunca logar secrets ou PII
- Usar `[AbpAuthorize]` para proteger AppServices
- Validar inputs via Data Annotations ou FluentValidation
