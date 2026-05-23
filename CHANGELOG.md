# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Added

*   docs: Documentação XML `/// <summary>` adicionada a todas as classes e métodos públicos (476 arquivos)
*   feat: add repo summary to .openhands/microagents/repo.md
*   Readme (openhands)

### Fixed

*   fix(CA2254): Corrigir interpolação de strings em mensagens de log (AzureActiveDirectory, LDAP)
*   fix(NU1504): Remover PackageReference duplicados em projetos de teste
*   fix(CS0169): Remover campo não utilizado em KeyVaultSecretManagerBddTests
*   fix(CS1587): Remover comentários XML posicionados incorretamente (99 arquivos)
*   fix(CS1572/CS1573): Corrigir nomes de parâmetros em documentação XML
*   fix(test): Corrigir teste PathNavigatesAboveRoot para compatibilidade Linux
*   **Test Coverage Framework**: Implementação completa de testes BDD em português com xUnit.Net v2
*   **Coverage Reports**: Sistema de relatórios com dotnet-reportgenerator-globaltool
*   **Test Metrics**: Seção completa de métricas no README
*   **Compiler Warnings**: Correção de warnings em projetos de teste
*   **xUnit Warnings**: Correção de xUnit1012 (null parameters) e xUnit1048 (async void)
*   **Code Smells**: Remoção de package references desnecessários (System.*)
*   **Nullable Types**: Ajuste de assinaturas para aceitar parâmetros null
*   **CS8600 Warnings**: Correção de conversões null literais
*   **Statistics**: 1492 tests (1491 passing, 100% success rate), 24.1% line coverage, 54.5% method coverage
*   Fix issue #478: Build All and Test workflow failing due to Coverlet error
*   Fix issue #475: Corrigir testes unitários com falhas e atualizar README
*   Fix: Resolve Coverlet path errors in CI workflow
*   Fix: Address some SqliteCache test failures
*   Fix: Output PATH environment variable
*   Fix: Build solution in both Release and Debug configurations
*   Fix: Scale profile picture proportionally when smaller than container

### Changed

*   refactor: Atualizar xunit.runner.visualstudio para 3.1.4 em todos os projetos de teste
*   refactor: Padronizar PackageReference em Directory.Build.props
*   docs: Atualizar README com métricas de cobertura atualizadas (Maio 2026)
*   Update ABP to Version="10.2.0"
*   Refatorar install.sh para melhor verificação, tratamento de erros e feedback
*   Feat: Optimize CI pipelines and automate PR creation
*   Feat: Adicionar testes unitários para StringExtensions e corrigir lógica
*   docs: Add comprehensive XML documentation to public methods
*   docs: Update README with XML documentation progress
*   feat: Add comprehensive SerilogLoggerFactory tests with BDD pattern
*   README: Atualizar métricas com correção do SqliteCache
*   Adicionar documentação XML em português para interfaces e classes principais
*   Update and rename ci.yml to coverage-reports.yml

### Removed

*   Remove Edition from EAF

## [10.0.0] - 2025-02-14

### ⚡ BREAKING CHANGES


# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Added

*   feat: add repo summary to .openhands/microagents/repo.md
*   Readme (openhands)

### Fixed
n*   **Compiler Warnings**: Correção de 62 warnings em projetos de teste
*   **xUnit Warnings**: Correção de xUnit1012 (null parameters) e xUnit1048 (async void)
*   **Code Smells**: Remoção de package references desnecessários (System.*)
*   **Nullable Types**: Ajuste de assinaturas para aceitar parâmetros null
*   **CS8600 Warnings**: Correção de conversões null literais
*   **Statistics**: 63 tests passing (100% success rate), 28.3% line coverage

*   Fix issue #478: Build All and Test workflow failing due to Coverlet error
*   Fix issue #475: Corrigir testes unitários com falhas e atualizar README
*   Fix: Resolve Coverlet path errors in CI workflow
*   Fix: Address some SqliteCache test failures
*   Fix: Output PATH environment variable
*   Fix: Build solution in both Release and Debug configurations
*   Fix: Scale profile picture proportionally when smaller than container

### Changed

*   Update ABP to Version="10.2.0"
*   Refatorar install.sh para melhor verificação, tratamento de erros e feedback
*   Feat: Optimize CI pipelines and automate PR creation
*   Feat: Adicionar testes unitários para StringExtensions e corrigir lógica
*   docs: Add comprehensive XML documentation to public methods
*   docs: Update README with XML documentation progress
*   feat: Add comprehensive SerilogLoggerFactory tests with BDD pattern
*   📊 README: Atualizar métricas com correção do SqliteCache
*   📝 Adicionar documentação XML em português para interfaces e classes principais
*   Update and rename ci.yml to coverage-reports.yml


# [9.0.2](https://github.com/afonsoft/EAF/releases/tag/9.0.2)
> 05/31/2025 02:55:20 UTC
##### ``9.0.2``
Update ABP to 10.2.0
# [9.0.1](https://github.com/afonsoft/EAF/releases/tag/9.0.1)
> 04/16/2025 03:02:27 UTC
##### ``9.0.1``
9.0.1
# [9.0.0](https://github.com/afonsoft/EAF/releases/tag/9.0.0)
> 03/05/2025 19:25:01 UTC
##### ``9.0.0``
Removendo o EAF old e usando o ABP como base para o Middle.
# [6.1.9](https://github.com/afonsoft/EAF/releases/tag/6.1.9)
> 01/25/2024 20:15:38 UTC
##### ``6.1.9``
6.1.9
# [6.1.8](https://github.com/afonsoft/EAF/releases/tag/6.1.8)
> 01/11/2024 15:01:57 UTC
##### ``6.1.8``
6.1.8
# [6.1.7](https://github.com/afonsoft/EAF/releases/tag/6.1.7)
> 12/19/2023 13:13:36 UTC
##### ``6.1.7``
Corre&#231;&#245;es do assembly names duplicados e atualiza&#231;&#245;es dos pacotes nugets
# [6.1.6](https://github.com/afonsoft/EAF/releases/tag/6.1.6)
> 12/18/2023 20:13:18 UTC
##### ``6.1.6``
6.1.6
Corre&#231;&#245;es do EF Plus e de refer&#234;ncias do nuget
# [6.1.5](https://github.com/afonsoft/EAF/releases/tag/6.1.5)
> 12/18/2023 13:51:23 UTC
##### ``6.1.5``
6.1.5
Implementa&#231;&#227;o do MySqlServer
Update do Template NET8
Incluido o EafTenantAddress
# [6.1.4](https://github.com/afonsoft/EAF/releases/tag/6.1.4)
> 12/15/2023 12:34:23 UTC
##### ``6.1.4``
6.1.4
# [6.1.3](https://github.com/afonsoft/EAF/releases/tag/6.1.3)
> 11/16/2023 18:05:20 UTC
##### ``6.1.3``
6.1.3

NET7 and Suport a NET8
# [6.1.2](https://github.com/afonsoft/EAF/releases/tag/6.1.2)
> 09/27/2023 18:42:50 UTC
##### ``6.1.2``
6.1.2
# [6.1.1](https://github.com/afonsoft/EAF/releases/tag/6.1.1)
> 09/19/2023 13:23:04 UTC
##### ``6.1.1``
6.1.1
# [6.1.0](https://github.com/afonsoft/EAF/releases/tag/6.1.0)
> 08/08/2023 18:06:27 UTC
##### ``6.1.0``
6.1.0
# [6.0.14](https://github.com/afonsoft/EAF/releases/tag/6.0.14)
> 07/31/2023 14:42:00 UTC
##### ``6.0.14``
6.0.14
# [6.0.13](https://github.com/afonsoft/EAF/releases/tag/6.0.13)
> 07/28/2023 21:27:02 UTC
##### ``6.0.13``
6.0.13
# [6.0.11](https://github.com/afonsoft/EAF/releases/tag/6.0.11)
> 06/19/2023 12:14:04 UTC
##### ``6.0.11``
6.0.11
# [6.0.10](https://github.com/afonsoft/EAF/releases/tag/6.0.10)
> 06/15/2023 13:25:47 UTC
##### ``6.0.10``
6.0.10
# [6.0.9](https://github.com/afonsoft/EAF/releases/tag/6.0.9)
> 05/11/2023 20:51:19 UTC
##### ``6.0.9``
6.0.9
# [6.0.8](https://github.com/afonsoft/EAF/releases/tag/6.0.8)
> 04/20/2023 18:54:22 UTC
##### ``6.0.8``
6.0.8
# [6.0.7](https://github.com/afonsoft/EAF/releases/tag/6.0.7)
> 03/23/2023 19:10:13 UTC
##### ``6.0.7``
6.0.7
# [6.0.6](https://github.com/afonsoft/EAF/releases/tag/6.0.6)
> 03/15/2023 17:37:16 UTC
##### ``6.0.6``
Fix AddStackExchangeRedisCache
# [v6.0.5](https://github.com/afonsoft/EAF/releases/tag/6.0.5)
> 02/08/2023 12:48:47 UTC
##### ``6.0.5``
v6.0.5
# [TAG 6.0.4](https://github.com/afonsoft/EAF/releases/tag/6.0.4)
> 01/31/2023 19:16:18 UTC
##### ``6.0.4``
TAG 6.0.4
# [V6.0.3](https://github.com/afonsoft/EAF/releases/tag/6.0.3)
> 12/14/2022 19:20:21 UTC
##### ``6.0.3``
V6.0.3
# [V6.0.2](https://github.com/afonsoft/EAF/releases/tag/6.0.2.1)
> 11/17/2022 12:19:52 UTC
##### ``6.0.2.1``
V6.0.2
Angular 8
# [V6.0.2](https://github.com/afonsoft/EAF/releases/tag/6.0.2)
> 11/16/2022 20:23:52 UTC
##### ``6.0.2``
V6.0.2
Angular 8
# [V6.0.1](https://github.com/afonsoft/EAF/releases/tag/6.0.1)
> 11/10/2022 15:09:03 UTC
##### ``6.0.1``
V6.0.1
# [v6.0.0](https://github.com/afonsoft/EAF/releases/tag/6.0.0)
> 11/09/2022 16:05:26 UTC
##### ``6.0.0``
v6.0.0
# [First Release 5.0.0](https://github.com/afonsoft/EAF/releases/tag/5.0.0)
> 11/04/2021 16:38:40 UTC
##### ``5.0.0``
First Release 5.0.0
# [release candidate 4](https://github.com/afonsoft/EAF/releases/tag/5.0.0-rc.4)
> 10/22/2021 16:28:26 UTC
##### ``5.0.0-rc.4``
release candidate 4

