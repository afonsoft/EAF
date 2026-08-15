# Eaf.DynamicEntityProperties

Dynamic Entity Properties integration for the Enterprise Application Foundation (EAF).

## Description

This module exposes ABP's Dynamic Entity Properties subsystem as EAF application services and Angular-friendly REST endpoints, allowing administrators to define custom properties on entities at runtime without changing the domain model.

## Dependencies

- `Abp` (10.5.0)
- `Abp.AutoMapper` (10.5.0)

## Components

- `EafDynamicEntityPropertiesModule`
- `DynamicPropertyAppService`
- `DynamicEntityPropertyAppService`
- `DynamicEntityPropertyValueAppService`
- DTOs and AutoMapper profile under `Application/Dto`
- Authorization provider and permission constants under `Authorization`

## Usage

1. Add a project reference to `Eaf.DynamicEntityProperties`.
2. Add `typeof(EafDynamicEntityPropertiesModule)` to your module's `DependsOn` list.
3. Ensure your `DbContext` inherits from `AbpZeroDbContext` so the dynamic property tables already exist.
4. Register allowed input types and entities through `DynamicEntityPropertyDefinitionProvider`.

```csharp
[DependsOn(typeof(EafDynamicEntityPropertiesModule))]
public class MyProjectModule : AbpModule
{
    // ...
}
```

```csharp
public class MyDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
{
    public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
    {
        context.Manager.AddAllowedInputType<SingleLineStringInputType>();
        context.Manager.AddAllowedInputType<ComboboxInputType>();
        context.Manager.AddAllowedInputType<CheckboxInputType>();
        context.Manager.AddAllowedInputType<MultiSelectComboboxInputType>();
        context.Manager.AddAllowedEntity<User>();
    }
}
```

## License

This project is licensed under the GPL-3.0-or-later License.
