---
name: '.NET Project Configuration'
description: 'Padrões de configuração de projetos .NET para o EAF, incluindo TargetFramework, NuGet packaging e build settings'
applyTo: '**/*.csproj,**/*.props,**/*.targets'
---

# .NET Project Configuration

## Target Framework

- Principal: `net10.0`
- LangVersion: `14.0`
- Nullable: desabilitado no EAF

## Build

- Deterministic builds habilitados
- SourceLink configurado para GitHub
- `GenerateDocumentationFile`: true para bibliotecas
- `common.props` centraliza versões — alterar com cuidado

## NuGet

- Pacotes publicados via CI/CD (`publish-all.yml`)
- SymbolPackageFormat: `snupkg`
- ContinuousIntegrationBuild: true
- Não adicionar pacotes sem verificar compatibilidade

## Testes

- coverlet para cobertura de código
- xUnit como framework de testes
- Shouldly para assertions
- NSubstitute para mocking

## Warnings

- `NoWarn` configurado para suprimir warnings conhecidos
- Não suprimir novos warnings sem justificativa
- Tratar warnings como errors em Release
