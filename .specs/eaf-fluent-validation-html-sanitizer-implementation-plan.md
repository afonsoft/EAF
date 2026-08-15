# Plano de Implementação — Eaf.FluentValidation e Eaf.HtmlSanitizer

> **Para agentes:** implementar seguindo `executing-plans` ou `subagent-driven-development`.

## Objetivo

Entregar dois módulos middleware EAF independentes:

1. `Eaf.FluentValidation` — integra `IValidator<T>` do FluentValidation ao pipeline de validação do ABP (`IMethodParameterValidator`), mantendo DataAnnotations.
2. `Eaf.HtmlSanitizer` — fornece `IHtmlSanitizer` usando o pacote `HtmlSanitizer` (`Ganss.XSS`) com política padrão restritiva e configuração via `EafHtmlSanitizerOptions`.

## Stack

- .NET 10 / C# 14
- ABP 10.5.0 (`Abp`, `Abp.TestBase`)
- `FluentValidation` 11.11.0
- `HtmlSanitizer` 9.0.892
- Castle Windsor, xUnit, Shouldly, NSubstitute

---

## Parte 1 — Eaf.FluentValidation

### Arquivos

- Criar:
  - `src/Eaf.FluentValidation/Eaf.FluentValidation.csproj`
  - `src/Eaf.FluentValidation/EAF.ico`
  - `src/Eaf.FluentValidation/EAF.png`
  - `src/Eaf.FluentValidation/EafFluentValidationOptions.cs`
  - `src/Eaf.FluentValidation/EafFluentValidationValidatorFactory.cs`
  - `src/Eaf.FluentValidation/EafFluentValidationMethodParameterValidator.cs`
  - `src/Eaf.FluentValidation/EafFluentValidationModule.cs`
  - `src/Eaf.FluentValidation/README.md`
  - `test/Eaf.FluentValidation.Tests/Eaf.FluentValidation.Tests.csproj`
  - `test/Eaf.FluentValidation.Tests/EafFluentValidationTestModule.cs`
  - `test/Eaf.FluentValidation.Tests/SampleValidators/CreateUserInput.cs`
  - `test/Eaf.FluentValidation.Tests/SampleValidators/CreateUserInputValidator.cs`
  - `test/Eaf.FluentValidation.Tests/EafFluentValidationValidatorFactoryTests.cs`
  - `test/Eaf.FluentValidation.Tests/EafFluentValidationMethodParameterValidatorTests.cs`
  - `test/Eaf.FluentValidation.Tests/EafFluentValidationModuleTests.cs`
- Modificar:
  - `Eaf.sln`

### Tarefas

1. Criar `csproj` com `TargetFramework net10.0`, `GenerateDocumentationFile true`, pacotes `Abp` 10.5.0 e `FluentValidation` 11.11.0, `README.md` embutido.
2. Implementar `EafFluentValidationOptions` com `List<Assembly> ValidatorAssemblies`, implementando `IOptions<EafFluentValidationOptions>` (`Value => this`).
3. Implementar `EafFluentValidationValidatorFactory` (`IIocResolver`) com `IValidator GetValidator(Type type)` resolvendo `IValidator<T>` fechado ou retornando `null`.
4. Implementar `EafFluentValidationMethodParameterValidator` (`IMethodParameterValidator`, `ITransientDependency`) com `Validate(object validatingObject)`:
   - retorna lista vazia para `null`;
   - obtém validator pelo tipo;
   - cria `ValidationContext<object>(validatingObject)`;
   - mapeia `ValidationFailure` para `System.ComponentModel.DataAnnotations.ValidationResult`.
5. Implementar `EafFluentValidationModule`:
   - `DependsOn(typeof(AbpKernelModule))`;
   - `PreInitialize` registra `EafFluentValidationOptions` como singleton/`IOptions` e adiciona o validator ABP via `Configuration.Validation.Validators.Add<EafFluentValidationMethodParameterValidator>()`;
   - `Initialize` registra por convenção todos os `IValidator<T>` das assemblies configuradas em `ValidatorAssemblies`.
6. Criar testes BDD em português cobrindo resolução, execução de regras, mapeamento de erros e coexistência com DataAnnotations.

---

## Parte 2 — Eaf.HtmlSanitizer

### Arquivos

- Criar:
  - `src/Eaf.HtmlSanitizer/Eaf.HtmlSanitizer.csproj`
  - `src/Eaf.HtmlSanitizer/EAF.ico`
  - `src/Eaf.HtmlSanitizer/EAF.png`
  - `src/Eaf.HtmlSanitizer/Html/IHtmlSanitizer.cs`
  - `src/Eaf.HtmlSanitizer/Html/DefaultHtmlSanitizer.cs`
  - `src/Eaf.HtmlSanitizer/EafHtmlSanitizerOptions.cs`
  - `src/Eaf.HtmlSanitizer/EafHtmlSanitizerModule.cs`
  - `src/Eaf.HtmlSanitizer/README.md`
  - `test/Eaf.HtmlSanitizer.Tests/Eaf.HtmlSanitizer.Tests.csproj`
  - `test/Eaf.HtmlSanitizer.Tests/DefaultHtmlSanitizerTests.cs`
  - `test/Eaf.HtmlSanitizer.Tests/EafHtmlSanitizerOptionsTests.cs`
  - `test/Eaf.HtmlSanitizer.Tests/EafHtmlSanitizerModuleTests.cs`
- Modificar:
  - `Eaf.sln`

### Tarefas

1. Criar `csproj` com `TargetFramework net10.0`, `GenerateDocumentationFile true`, pacotes `Abp` 10.5.0 e `HtmlSanitizer` 9.0.892, `README.md` embutido.
2. Implementar `EafHtmlSanitizerOptions` (`IOptions<EafHtmlSanitizerOptions>`) com `ISet<string> AllowedTags`, `AllowedAttributes`, `AllowedCssProperties`, `AllowedUriSchemes`, valores padrão seguros.
3. Implementar `IHtmlSanitizer` com `string Sanitize(string html, EafHtmlSanitizerOptions options = null)`.
4. Implementar `DefaultHtmlSanitizer` (`ISingletonDependency`):
   - constrói `Ganss.XSS.HtmlSanitizer` a partir das opções;
   - `Sanitize(null)` e `Sanitize(string.Empty)` retornam `string.Empty`;
   - quando `options` fornecido, cria instância temporária mesclada com defaults;
   - remove `<script>`, `<style>`, event handlers (`on*`) e esquemas não permitidos (`javascript:`).
5. Implementar `EafHtmlSanitizerModule` (`AbpModule`) registrando `EafHtmlSanitizerOptions` singleton e `IHtmlSanitizer` -> `DefaultHtmlSanitizer`.
6. Criar testes BDD em português para scripts, atributos de evento, URIs `javascript:`, tags permitidas, DI e configuração.

---

## Validação

- `dotnet build Eaf.sln --configuration Release` (0 warnings, 0 errors)
- `dotnet test Eaf.sln --configuration Release --no-build` (todos passam)
- Verificar cobertura não reduzida.

## Riscos e Mitigações

| Risco | Mitigação |
|---|---|
| `FluentValidation` v11 exige `ValidationContext<object>` | Usar `new ValidationContext<object>(validatingObject)` e resolver `IValidator` (não genérico). |
| Conflito de nome `HtmlSanitizerOptions` | Nossa classe chama-se `EafHtmlSanitizerOptions`; pacote usa `Ganss.XSS.HtmlSanitizerOptions`. |
| `Eaf.sln` duplicar GUIDs | Gerar novos GUIDs para cada projeto. |

---

## Pós-Implementação

1. Atualizar `.specs/eaf-specs-index-and-roadmap-2026.md` e `.specs/eaf-implementation-plan-q3-2026.spec.md` marcando os módulos como entregues.
2. Criar PR `feature/eaf-fluent-validation-html-sanitizer` para `main`.
