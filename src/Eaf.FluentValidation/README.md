# Eaf.FluentValidation

Módulo EAF para integração do FluentValidation ao pipeline de validação do ABP, sem remover o suporte a DataAnnotations.

## Dependências

- `Abp` 10.5.0
- `FluentValidation` 11.11.0

## Uso

```csharp
[DependsOn(typeof(EafFluentValidationModule))]
public class MyProjectModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.Configure<EafFluentValidationOptions>(options =>
        {
            options.ValidatorAssemblies.Add(typeof(MyProjectModule).GetAssembly());
        });
    }
}
```

Crie validadores normais do FluentValidation:

```csharp
public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8);
    }
}
```

O ABP executará automaticamente o FluentValidation junto com DataAnnotations.
