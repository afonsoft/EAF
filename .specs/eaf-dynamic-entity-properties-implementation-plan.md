# Plano de Implementação — Eaf.DynamicEntityProperties

> **Para agentes:** use `superpowers:executing-plans` ou implemente passo a passo. Cada tarefa é um passo curto.

**Objetivo:** Entregar módulo `Eaf.DynamicEntityProperties` reutilizando os gerenciadores (`IDynamicPropertyManager`, `IDynamicEntityPropertyManager`, `IDynamicEntityPropertyValueManager`) do ABP, sem duplicar as entidades/tabelas existentes. Adicionar serviços de aplicação, permissões e telas Angular.

**Abordagem:** Criar projeto `src/Eaf.DynamicEntityProperties` com DTOs, app services, permissões, módulo ABP e testes. Reaproveitar `Abp.DynamicEntityProperties` para domínio/persistência. Adicionar página `admin/dynamic-properties` e componente `dynamic-entity-property-manager` no template Angular.

**Stack:** .NET 10, ABP 10.5, Castle Windsor, C# 14, Angular 20, PrimeNG 17, xUnit/Shouldly/NSubstitute.

---

## Estrutura de arquivos

- `src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj`
- `src/Eaf.DynamicEntityProperties/EafDynamicEntityPropertiesModule.cs`
- `src/Eaf.DynamicEntityProperties/Application/Dto/*.cs`
- `src/Eaf.DynamicEntityProperties/Application/DynamicPropertyAppService.cs`
- `src/Eaf.DynamicEntityProperties/Application/IDynamicPropertyAppService.cs`
- `src/Eaf.DynamicEntityProperties/Application/DynamicEntityPropertyAppService.cs`
- `src/Eaf.DynamicEntityProperties/Application/IDynamicEntityPropertyAppService.cs`
- `src/Eaf.DynamicEntityProperties/Application/DynamicEntityPropertyValueAppService.cs`
- `src/Eaf.DynamicEntityProperties/Application/IDynamicEntityPropertyValueAppService.cs`
- `src/Eaf.DynamicEntityProperties/Authorization/EafDynamicEntityPropertiesPermissionNames.cs`
- `src/Eaf.DynamicEntityProperties/Authorization/EafDynamicEntityPropertiesAuthorizationProvider.cs`
- `src/Eaf.DynamicEntityProperties/README.md`
- `test/Eaf.DynamicEntityProperties.Tests/Eaf.DynamicEntityProperties.Tests.csproj`
- `test/Eaf.DynamicEntityProperties.Tests/EafDynamicEntityPropertiesTestModule.cs`
- `test/Eaf.DynamicEntityProperties.Tests/EafDynamicEntityPropertiesTestBase.cs`
- `test/Eaf.DynamicEntityProperties.Tests/DynamicPropertyAppService_Tests.cs`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/dynamic-properties/*.ts|html|less`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/dynamic-entity-property-manager/*.ts|html|less`
- `Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/dynamic-entity-property.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/admin-routing.module.ts` (adicionar rota)
- `Eaf.sln` (adicionar projetos)

---

## Tarefas

### Tarefa 1: Criar projeto .NET e módulo ABP

**Arquivos:**
- Criar: `src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj`
- Criar: `src/Eaf.DynamicEntityProperties/EafDynamicEntityPropertiesModule.cs`

**Passos:**

- [ ] **1.1 Criar o csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Import Project="..\..\common.props" />
  <PropertyGroup>
    <RootNamespace>Eaf</RootNamespace>
    <TargetFrameworks>net10.0</TargetFrameworks>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <AssemblyName>Eaf.DynamicEntityProperties</AssemblyName>
    <PackageId>Eaf.DynamicEntityProperties</PackageId>
    <Description>Enterprise Application Foundation - Dynamic Entity Properties</Description>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Abp" Version="10.5.0" />
    <PackageReference Include="Abp.AutoMapper" Version="10.5.0" />
  </ItemGroup>
  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
  </ItemGroup>
</Project>
```

- [ ] **1.2 Criar o módulo ABP**

```csharp
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using Abp.Modules;
using System.Reflection;

namespace Eaf.DynamicEntityProperties
{
    /// <summary>
    /// Módulo ABP para Dynamic Entity Properties do EAF.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule), typeof(AbpAutoMapperModule))]
    public class EafDynamicEntityPropertiesModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<EafDynamicEntityPropertiesAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
```

- [ ] **1.3 Buildar**

```bash
dotnet build src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj --configuration Release
```

Esperado: build com 0 warnings.

### Tarefa 2: Criar DTOs

**Arquivos:**
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/DynamicPropertyDto.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/CreateOrUpdateDynamicPropertyInput.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/DynamicPropertyValueDto.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/DynamicEntityPropertyDto.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/CreateDynamicEntityPropertyInput.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/DynamicEntityPropertyValueDto.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/Dto/CreateOrUpdateDynamicEntityPropertyValueInput.cs`

**Passos:**

- [ ] **2.1 DynamicPropertyDto**

```csharp
using Abp.Application.Services.Dto;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class DynamicPropertyDto : EntityDto<int>
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string InputType { get; set; }
        public string Permission { get; set; }
        public int? TenantId { get; set; }
    }
}
```

- [ ] **2.2 CreateOrUpdateDynamicPropertyInput**

```csharp
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class CreateOrUpdateDynamicPropertyInput : EntityDto<int>
    {
        [Required]
        [StringLength(256)]
        public string PropertyName { get; set; }

        [StringLength(256)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(256)]
        public string InputType { get; set; }

        [StringLength(256)]
        public string Permission { get; set; }
    }
}
```

- [ ] **2.3 DynamicPropertyValueDto**

```csharp
using Abp.Application.Services.Dto;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class DynamicPropertyValueDto : EntityDto<long>
    {
        string Value { get; set; }
        public int DynamicPropertyId { get; set; }
        public int? TenantId { get; set; }
    }
}
```

- [ ] **2.4 DynamicEntityPropertyDto**

```csharp
using Abp.Application.Services.Dto;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class DynamicEntityPropertyDto : EntityDto<int>
    {
        public string EntityFullName { get; set; }
        public int DynamicPropertyId { get; set; }
        public DynamicPropertyDto DynamicProperty { get; set; }
        public int? TenantId { get; set; }
    }
}
```

- [ ] **2.5 CreateDynamicEntityPropertyInput**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class CreateDynamicEntityPropertyInput
    {
        [Required]
        [StringLength(256)]
        public string EntityFullName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DynamicPropertyId { get; set; }
    }
}
```

- [ ] **2.6 DynamicEntityPropertyValueDto**

```csharp
using Abp.Application.Services.Dto;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class DynamicEntityPropertyValueDto : EntityDto<long>
    {
        public string Value { get; set; }
        public string EntityId { get; set; }
        public int DynamicEntityPropertyId { get; set; }
        public DynamicEntityPropertyDto DynamicEntityProperty { get; set; }
        public int? TenantId { get; set; }
    }
}
```

- [ ] **2.7 CreateOrUpdateDynamicEntityPropertyValueInput**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    public class CreateOrUpdateDynamicEntityPropertyValueInput
    {
        public long? Id { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public string EntityId { get; set; }

        [Range(1, int.MaxValue)]
        public int DynamicEntityPropertyId { get; set; }
    }
}
```

- [ ] **2.8 Buildar**

```bash
dotnet build src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj --configuration Release
```

### Tarefa 3: Criar permissões e provider

**Arquivos:**
- Criar: `src/Eaf.DynamicEntityProperties/Authorization/EafDynamicEntityPropertiesPermissionNames.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Authorization/EafDynamicEntityPropertiesAuthorizationProvider.cs`

**Passos:**

- [ ] **3.1 Constantes de permissão**

```csharp
namespace Eaf.DynamicEntityProperties.Authorization
{
    public static class EafDynamicEntityPropertiesPermissionNames
    {
        public const string Pages_Administration_DynamicProperties = "Pages.Administration.DynamicProperties";
        public const string Pages_Administration_DynamicProperties_Create = "Pages.Administration.DynamicProperties.Create";
        public const string Pages_Administration_DynamicProperties_Edit = "Pages.Administration.DynamicProperties.Edit";
        public const string Pages_Administration_DynamicProperties_Delete = "Pages.Administration.DynamicProperties.Delete";
        public const string Pages_Administration_DynamicEntityPropertyValues = "Pages.Administration.DynamicEntityPropertyValues";
    }
}
```

- [ ] **3.2 Authorization provider**

```csharp
using Abp.Authorization;
using Abp.Localization;

namespace Eaf.DynamicEntityProperties.Authorization
{
    public class EafDynamicEntityPropertiesAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages.Administration");
            var dynamicProperties = pages?.CreateChildPermission(
                EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties,
                new FixedLocalizableString("DynamicProperties"));

            dynamicProperties?.CreateChildPermission(
                EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Create,
                new FixedLocalizableString("Create"));

            dynamicProperties?.CreateChildPermission(
                EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Edit,
                new FixedLocalizableString("Edit"));

            dynamicProperties?.CreateChildPermission(
                EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Delete,
                new FixedLocalizableString("Delete"));

            dynamicProperties?.CreateChildPermission(
                EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicEntityPropertyValues,
                new FixedLocalizableString("ManageValues"));
        }
    }
}
```

### Tarefa 4: Criar app services

**Arquivos:**
- Criar: `src/Eaf.DynamicEntityProperties/Application/IDynamicPropertyAppService.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/DynamicPropertyAppService.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/IDynamicEntityPropertyAppService.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/DynamicEntityPropertyAppService.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/IDynamicEntityPropertyValueAppService.cs`
- Criar: `src/Eaf.DynamicEntityProperties/Application/DynamicEntityPropertyValueAppService.cs`

**Passos:**

- [ ] **4.1 IDynamicPropertyAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.DynamicEntityProperties.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public interface IDynamicPropertyAppService : IApplicationService
    {
        Task<DynamicPropertyDto> CreateAsync(CreateOrUpdateDynamicPropertyInput input);
        Task<DynamicPropertyDto> UpdateAsync(CreateOrUpdateDynamicPropertyInput input);
        Task DeleteAsync(EntityDto<int> input);
        Task<List<DynamicPropertyDto>> GetAllAsync();
        Task<DynamicPropertyDto> GetAsync(EntityDto<int> input);
    }
}
```

- [ ] **4.2 DynamicPropertyAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using Abp.UI;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public class DynamicPropertyAppService : ApplicationService, IDynamicPropertyAppService
    {
        private readonly IDynamicPropertyManager _dynamicPropertyManager;

        public DynamicPropertyAppService(IDynamicPropertyManager dynamicPropertyManager)
        {
            _dynamicPropertyManager = dynamicPropertyManager;
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Create)]
        public virtual async Task<DynamicPropertyDto> CreateAsync(CreateOrUpdateDynamicPropertyInput input)
        {
            var entity = ObjectMapper.Map<DynamicProperty>(input);
            await _dynamicPropertyManager.AddAsync(entity);
            return ObjectMapper.Map<DynamicPropertyDto>(entity);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Edit)]
        public virtual async Task<DynamicPropertyDto> UpdateAsync(CreateOrUpdateDynamicPropertyInput input)
        {
            var existing = await _dynamicPropertyManager.GetAsync(input.Id);
            ObjectMapper.Map(input, existing);
            await _dynamicPropertyManager.UpdateAsync(existing);
            return ObjectMapper.Map<DynamicPropertyDto>(existing);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Delete)]
        public virtual async Task DeleteAsync(EntityDto<int> input)
        {
            await _dynamicPropertyManager.DeleteAsync(input.Id);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties)]
        public virtual async Task<List<DynamicPropertyDto>> GetAllAsync()
        {
            var items = await _dynamicPropertyManager.GetAllAsync();
            return ObjectMapper.Map<List<DynamicPropertyDto>>(items);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties)]
        public virtual async Task<DynamicPropertyDto> GetAsync(EntityDto<int> input)
        {
            var item = await _dynamicPropertyManager.GetAsync(input.Id);
            return ObjectMapper.Map<DynamicPropertyDto>(item);
        }
    }
}
```

- [ ] **4.3 IDynamicEntityPropertyAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.DynamicEntityProperties.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public interface IDynamicEntityPropertyAppService : IApplicationService
    {
        Task<DynamicEntityPropertyDto> CreateAsync(CreateDynamicEntityPropertyInput input);
        Task DeleteAsync(EntityDto<int> input);
        Task<List<DynamicEntityPropertyDto>> GetAllAsync(string entityFullName);
    }
}
```

- [ ] **4.4 DynamicEntityPropertyAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public class DynamicEntityPropertyAppService : ApplicationService, IDynamicEntityPropertyAppService
    {
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;

        public DynamicEntityPropertyAppService(IDynamicEntityPropertyManager dynamicEntityPropertyManager)
        {
            _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Create)]
        public virtual async Task<DynamicEntityPropertyDto> CreateAsync(CreateDynamicEntityPropertyInput input)
        {
            var entity = ObjectMapper.Map<DynamicEntityProperty>(input);
            await _dynamicEntityPropertyManager.AddAsync(entity);
            return ObjectMapper.Map<DynamicEntityPropertyDto>(entity);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties_Delete)]
        public virtual async Task DeleteAsync(EntityDto<int> input)
        {
            await _dynamicEntityPropertyManager.DeleteAsync(input.Id);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicProperties)]
        public virtual async Task<List<DynamicEntityPropertyDto>> GetAllAsync(string entityFullName)
        {
            var items = await _dynamicEntityPropertyManager.GetAllAsync(entityFullName);
            return ObjectMapper.Map<List<DynamicEntityPropertyDto>>(items);
        }
    }
}
```

- [ ] **4.5 IDynamicEntityPropertyValueAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.DynamicEntityProperties.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public interface IDynamicEntityPropertyValueAppService : IApplicationService
    {
        Task<DynamicEntityPropertyValueDto> CreateOrUpdateAsync(CreateOrUpdateDynamicEntityPropertyValueInput input);
        Task DeleteAsync(EntityDto<long> input);
        Task<List<DynamicEntityPropertyValueDto>> GetAllValuesAsync(string entityFullName, string entityId);
    }
}
```

- [ ] **4.6 DynamicEntityPropertyValueAppService**

```csharp
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application.Dto;
using Eaf.DynamicEntityProperties.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.DynamicEntityProperties.Application
{
    public class DynamicEntityPropertyValueAppService : ApplicationService, IDynamicEntityPropertyValueAppService
    {
        private readonly IDynamicEntityPropertyValueManager _valueManager;

        public DynamicEntityPropertyValueAppService(IDynamicEntityPropertyValueManager valueManager)
        {
            _valueManager = valueManager;
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicEntityPropertyValues)]
        public virtual async Task<DynamicEntityPropertyValueDto> CreateOrUpdateAsync(CreateOrUpdateDynamicEntityPropertyValueInput input)
        {
            DynamicEntityPropertyValue entity;
            if (input.Id.HasValue)
            {
                entity = await _valueManager.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, entity);
                await _valueManager.UpdateAsync(entity);
            }
            else
            {
                entity = ObjectMapper.Map<DynamicEntityPropertyValue>(input);
                await _valueManager.AddAsync(entity);
            }

            return ObjectMapper.Map<DynamicEntityPropertyValueDto>(entity);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicEntityPropertyValues)]
        public virtual async Task DeleteAsync(EntityDto<long> input)
        {
            await _valueManager.DeleteAsync(input.Id);
        }

        [Abp.Authorization.AbpAuthorize(EafDynamicEntityPropertiesPermissionNames.Pages_Administration_DynamicEntityPropertyValues)]
        public virtual async Task<List<DynamicEntityPropertyValueDto>> GetAllValuesAsync(string entityFullName, string entityId)
        {
            var items = await _valueManager.GetValuesAsync(entityFullName, entityId);
            return ObjectMapper.Map<List<DynamicEntityPropertyValueDto>>(items);
        }
    }
}
```

- [ ] **4.7 Configurar AutoMapper**

Criar `src/Eaf.DynamicEntityProperties/DynamicEntityPropertiesDtoMapper.cs`:

```csharp
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using Eaf.DynamicEntityProperties.Application.Dto;

namespace Eaf.DynamicEntityProperties
{
    public class DynamicEntityPropertiesDtoMapper : IDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>().ReverseMap();

            configuration.CreateMap<CreateOrUpdateDynamicPropertyInput, DynamicProperty>();
            configuration.CreateMap<CreateDynamicEntityPropertyInput, DynamicEntityProperty>();
            configuration.CreateMap<CreateOrUpdateDynamicEntityPropertyValueInput, DynamicEntityPropertyValue>();
        }
    }
}
```

No módulo, adicionar ao `PreInitialize`:

```csharp
Configuration.Modules.AbpAutoMapper().Configurators.Add(DynamicEntityPropertiesDtoMapper.CreateMappings);
```

- [ ] **4.8 Buildar**

```bash
dotnet build src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj --configuration Release
```

### Tarefa 5: Testes

**Arquivos:**
- Criar: `test/Eaf.DynamicEntityProperties.Tests/Eaf.DynamicEntityProperties.Tests.csproj`
- Criar: `test/Eaf.DynamicEntityProperties.Tests/EafDynamicEntityPropertiesTestModule.cs`
- Criar: `test/Eaf.DynamicEntityProperties.Tests/EafDynamicEntityPropertiesTestBase.cs`
- Criar: `test/Eaf.DynamicEntityProperties.Tests/DynamicPropertyAppService_Tests.cs`

**Passos:**

- [ ] **5.1 Criar o csproj do teste**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>14.0</LangVersion>
    <Nullable>disable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Abp.TestBase" Version="10.5.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.*" />
    <PackageReference Include="Shouldly" Version="4.3.0" />
    <PackageReference Include="xunit" Version="2.9.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Eaf.DynamicEntityProperties\Eaf.DynamicEntityProperties.csproj" />
    <ProjectReference Include="..\..\test\Eaf.MiddlewareCore.SampleApp\Eaf.MiddlewareCore.SampleApp.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **5.2 Módulo de teste**

```csharp
using Abp.Modules;
using Abp.TestBase;
using Eaf.MiddlewareCore.SampleApp;

namespace Eaf.DynamicEntityProperties.Tests
{
    [DependsOn(typeof(EafDynamicEntityPropertiesModule), typeof(EafMiddlewareCoreSampleAppModule), typeof(AbpTestBaseModule))]
    public class EafDynamicEntityPropertiesTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<EafDynamicEntityPropertiesAuthorizationProvider>();
        }
    }
}
```

- [ ] **5.3 Test base**

```csharp
using Abp.TestBase;

namespace Eaf.DynamicEntityProperties.Tests
{
    public abstract class EafDynamicEntityPropertiesTestBase : AbpIntegratedTestBase<EafDynamicEntityPropertiesTestModule>
    {
    }
}
```

- [ ] **5.4 Teste de create/get**

```csharp
using Eaf.DynamicEntityProperties.Application;
using Eaf.DynamicEntityProperties.Application.Dto;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.DynamicEntityProperties.Tests
{
    public class DynamicPropertyAppService_Tests : EafDynamicEntityPropertiesTestBase
    {
        private readonly IDynamicPropertyAppService _service;

        public DynamicPropertyAppService_Tests()
        {
            _service = Resolve<IDynamicPropertyAppService>();
            Abp.Authorization.AbpSession.TenantId = null;
        }

        [Fact]
        public async Task Dado_PropriedadeValida_Quando_Criar_Entao_DeveRetornarComId()
        {
            var input = new CreateOrUpdateDynamicPropertyInput
            {
                PropertyName = "City",
                DisplayName = "Cidade",
                InputType = "Abp.UI.Inputs.SingleLineStringInputType"
            };

            var result = await _service.CreateAsync(input);

            result.ShouldNotBeNull();
            result.Id.ShouldBeGreaterThan(0);
            result.PropertyName.ShouldBe("City");
        }

        [Fact]
        public async Task Dado_PropriedadeExistente_Quando_Consultar_Entao_DeveRetornar()
        {
            var created = await _service.CreateAsync(new CreateOrUpdateDynamicPropertyInput
            {
                PropertyName = "Country",
                InputType = "Abp.UI.Inputs.SingleLineStringInputType"
            });

            var result = await _service.GetAsync(new Abp.Application.Services.Dto.EntityDto<int> { Id = created.Id });

            result.ShouldNotBeNull();
            result.PropertyName.ShouldBe("Country");
        }
    }
}
```

- [ ] **5.5 Rodar testes**

```bash
dotnet test test/Eaf.DynamicEntityProperties.Tests/Eaf.DynamicEntityProperties.Tests.csproj --configuration Release
```

### Tarefa 6: Angular

**Arquivos:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/dynamic-entity-property.service.ts`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/dynamic-properties/dynamic-properties.component.ts`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/dynamic-properties/dynamic-properties.component.html`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/dynamic-properties/dynamic-properties.component.less`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/dynamic-entity-property-manager/dynamic-entity-property-manager.component.ts`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/dynamic-entity-property-manager/dynamic-entity-property-manager.component.html`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/dynamic-entity-property-manager/dynamic-entity-property-manager.component.less`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/admin-routing.module.ts`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/admin.module.ts` (se necessário)

**Passos:**

- [ ] **6.1 Serviço manual**

```typescript
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

export interface DynamicPropertyDto {
  id: number;
  propertyName: string;
  displayName?: string;
  inputType: string;
  permission?: string;
  tenantId?: number;
}

export interface CreateOrUpdateDynamicPropertyInput {
  id?: number;
  propertyName: string;
  displayName?: string;
  inputType: string;
  permission?: string;
}

export interface DynamicEntityPropertyDto {
  id: number;
  entityFullName: string;
  dynamicPropertyId: number;
  dynamicProperty?: DynamicPropertyDto;
  tenantId?: number;
}

export interface CreateDynamicEntityPropertyInput {
  entityFullName: string;
  dynamicPropertyId: number;
}

@Injectable({ providedIn: 'root' })
export class DynamicEntityPropertyService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.baseUrl = baseUrl ?? '';
  }

  getAll(): Observable<DynamicPropertyDto[]> {
    return this.http.get<DynamicPropertyDto[]>(`${this.baseUrl}/api/services/app/DynamicProperty/GetAll`);
  }

  create(input: CreateOrUpdateDynamicPropertyInput): Observable<DynamicPropertyDto> {
    return this.http.post<DynamicPropertyDto>(`${this.baseUrl}/api/services/app/DynamicProperty/Create`, input);
  }

  update(input: CreateOrUpdateDynamicPropertyInput): Observable<DynamicPropertyDto> {
    return this.http.put<DynamicPropertyDto>(`${this.baseUrl}/api/services/app/DynamicProperty/Update`, input);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/api/services/app/DynamicProperty/Delete`, { body: { id } });
  }

  getAllEntityProperties(entityFullName: string): Observable<DynamicEntityPropertyDto[]> {
    return this.http.get<DynamicEntityPropertyDto[]>(`${this.baseUrl}/api/services/app/DynamicEntityProperty/GetAll?entityFullName=${encodeURIComponent(entityFullName)}`);
  }

  createEntityProperty(input: CreateDynamicEntityPropertyInput): Observable<DynamicEntityPropertyDto> {
    return this.http.post<DynamicEntityPropertyDto>(`${this.baseUrl}/api/services/app/DynamicEntityProperty/Create`, input);
  }

  deleteEntityProperty(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/api/services/app/DynamicEntityProperty/Delete`, { body: { id } });
  }
}
```

- [ ] **6.2 Componente admin/dynamic-properties**

Criar componente standalone usando PrimeNG (`Table`, `Dialog`, `Dropdown`, `InputText`, `Button`) para listar/criar/editar `DynamicProperty` e associar a tipos de entidade (`User`).

Pseudo-código:

```typescript
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DynamicEntityPropertyService } from '@shared/service-proxies/dynamic-entity-property.service';

@Component({
  standalone: false,
  templateUrl: './dynamic-properties.component.html'
})
export class DynamicPropertiesComponent extends AppComponentBase implements OnInit {
  properties: any[] = [];
  dialogVisible = false;
  selected = {} as any;
  allowedInputTypes = [
    { label: 'SingleLineString', value: 'Abp.UI.Inputs.SingleLineStringInputType' },
    { label: 'Combobox', value: 'Abp.UI.Inputs.ComboboxInputType' },
    { label: 'Checkbox', value: 'Abp.UI.Inputs.CheckboxInputType' },
    { label: 'MultiSelectCombobox', value: 'Abp.UI.Inputs.MultiSelectComboboxInputType' }
  ];

  constructor(
    injector: Injector,
    private service: DynamicEntityPropertyService
  ) { super(injector); }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.getAll().subscribe(r => this.properties = r);
  }

  save(): void {
    const op = this.selected.id
      ? this.service.update(this.selected)
      : this.service.create(this.selected);
    op.subscribe(() => { this.hideDialog(); this.load(); });
  }

  showDialog(item?: any): void {
    this.selected = item ? { ...item } : { inputType: this.allowedInputTypes[0].value };
    this.dialogVisible = true;
  }

  hideDialog(): void { this.dialogVisible = false; }

  deleteItem(id: number): void {
    this.message.confirm('', this.l('AreYouSureToDelete'), (isConfirmed) => {
      if (isConfirmed) {
        this.service.delete(id).subscribe(() => this.load());
      }
    });
  }
}
```

- [ ] **6.3 Componente dynamic-entity-property-manager**

Receber `@Input() entityFullName` e `@Input() entityId`. Exibir campos input dinâmicos obtidos via `DynamicEntityPropertyService.getAllEntityProperties` e salvar valores via `DynamicEntityPropertyValue` endpoint.

- [ ] **6.4 Registrar rota**

Em `admin-routing.module.ts`:

```typescript
{
  path: 'dynamic-properties',
  component: DynamicPropertiesComponent,
  data: { permission: 'Pages.Administration.DynamicProperties' }
}
```

Em `admin.module.ts` declarar o componente.

- [ ] **6.5 Buildar Angular**

```bash
cd Templates/Angular/Eaf.ProjectName.UI
nvm use 20
npm install --legacy-peer-deps
npx ng build --configuration=production
```

### Tarefa 7: Integrar no EAF

**Arquivos:**
- Modificar: `src/Eaf.Middleware.Web.Core/EafMiddlewareWebCoreModule.cs`
- Modificar: `Eaf.sln`
- Criar: `src/Eaf.DynamicEntityProperties/README.md`

**Passos:**

- [ ] **7.1 Adicionar `Eaf.DynamicEntityPropertiesModule` no Web.Core opcional**

```csharp
[DependsOn(
    ...,
    typeof(Eaf.DynamicEntityProperties.EafDynamicEntityPropertiesModule)
)]
```

- [ ] **7.2 Adicionar projetos na solution**

```bash
dotnet sln Eaf.sln add src/Eaf.DynamicEntityProperties/Eaf.DynamicEntityProperties.csproj
dotnet sln Eaf.sln add test/Eaf.DynamicEntityProperties.Tests/Eaf.DynamicEntityProperties.Tests.csproj
```

- [ ] **7.3 README**

Criar `src/Eaf.DynamicEntityProperties/README.md` em português, descrevendo o módulo, dependências e uso básico.

### Tarefa 8: Verificação final

- [ ] **8.1 Build da solution**

```bash
dotnet build Eaf.sln --configuration Release
```

- [ ] **8.2 Testes**

```bash
dotnet test Eaf.sln --configuration Release --no-build
```

- [ ] **8.3 Angular**

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
```

- [ ] **8.4 Commit e PR**

Branch: `feature/eaf-dynamic-entity-properties`.

```bash
git checkout -b feature/eaf-dynamic-entity-properties
git add .
git commit -m "feat: add Eaf.DynamicEntityProperties module with ABP manager reuse and Angular UI"
git push origin feature/eaf-dynamic-entity-properties
```

---

## Cobertura da spec

| Spec FR | Tarefa |
|---|---|
| FR-001 CRUD de dynamic properties | Tarefas 2, 4 |
| FR-002 Associação de propriedades a entidades e valores | Tarefas 4, 6 |
| FR-003 Componente Angular | Tarefa 6 |
| FR-004 Permissões | Tarefa 3 |

Não duplica tabelas do ABP — reaproveita `AbpDynamicProperties` etc.
