---
name: aspnet-boilerplate-modules
description: Expert guidance for working with ASP.NET Boilerplate (ABP) framework modules. Covers module definition, lifecycle methods, dependencies, configuration, and usage of common ABP modules including dependency injection, session, caching, logging, settings, entities, repositories, application services, authorization, multi-tenancy, and more. Use this skill when creating ABP modules, configuring module dependencies, understanding ABP module system, or troubleshooting module initialization issues. Do NOT use for general .NET development, non-ABP projects, or EAF-specific middleware modules.
---

# ASP.NET Boilerplate Modules Skill

You are an expert in ASP.NET Boilerplate (ABP) framework modules. You create, configure, and maintain ABP modules following ABP conventions and best practices.

## Project Context

ASP.NET Boilerplate (ABP) is an open source application framework that provides infrastructure to build modules and compose them to create applications. The module system is focused on server-side development.

### Technology Stack
- **Framework**: ASP.NET Boilerplate
- **.NET Version**: Compatible with .NET Framework and .NET Core
- **Module System**: AbpModule base class
- **Dependency Injection**: Castle Windsor
- **Architecture**: N-Layer Architecture

## Module Definition

### Basic Module

A module is defined with a class derived from `AbpModule`:

```csharp
using Abp.Modules;
using Abp.Reflection.Extensions;

public class MyBlogModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### Module with Dependencies

Use the `DependsOn` attribute to declare module dependencies:

```csharp
using Abp.Modules;
using Abp.Reflection.Extensions;

[DependsOn(typeof(MyBlogCoreModule))]
public class MyBlogApplicationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### Multiple Dependencies

```csharp
[DependsOn(
    typeof(AbpKernelModule),
    typeof(AbpAutoMapperModule),
    typeof(MyBlogCoreModule)
)]
public class MyBlogApplicationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

## Module Lifecycle Methods

ABP calls specific methods on application startup and shutdown. Override these methods to perform specific tasks.

### PreInitialize

Called first when the application starts. Use this method to:
- Configure the framework and other modules before they initialize
- Register conventional registration classes
- Write code before dependency injection registrations

```csharp
public override void PreInitialize()
{
    // Configure AutoMapper
    Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
    {
        configuration.CreateMap<User, UserDto>();
    });
    
    // Register conventional registerer
    IocManager.AddConventionalRegisterer(new MyConventionalRegistrar());
    
    // Configure module settings
    Configuration.Modules.MyBlogModule().EnableFeature = true;
}
```

### Initialize

The place where dependency injection registration should be done:

```csharp
public override void Initialize()
{
    IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
}
```

### PostInitialize

Called last in the startup process. Safe to resolve dependencies here:

```csharp
public override void PostInitialize()
{
    var myService = IocManager.Resolve<IMyService>();
    myService.Initialize();
}
```

### Shutdown

Called when the application shuts down:

```csharp
public override void Shutdown()
{
    // Cleanup resources
}
```

### Lifecycle Order

For modules A (depends on B) and B:
- **Startup**: PreInitialize-B → PreInitialize-A → Initialize-B → Initialize-A → PostInitialize-B → PostInitialize-A
- **Shutdown**: Shutdown-A → Shutdown-B

## Common ABP Modules

### AbpKernelModule

Core module providing basic infrastructure:
- Dependency injection
- Event bus
- Logging
- Configuration
- Localization

```csharp
[DependsOn(typeof(AbpKernelModule))]
public class MyModule : AbpModule
{
    // Automatically gets DI, logging, etc.
}
```

### AbpAutoMapperModule

Object-to-object mapping with AutoMapper:

```csharp
[DependsOn(typeof(AbpAutoMapperModule))]
public class MyModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
        {
            configuration.CreateMap<User, UserDto>();
            configuration.CreateMap<CreateUserDto, User>();
        });
    }
}
```

### AbpEntityFrameworkModule

Entity Framework integration:

```csharp
[DependsOn(typeof(AbpEntityFrameworkModule))]
public class MyModule : AbpModule
{
    public override void Initialize()
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<MyDbContext>());
    }
}
```

### AbpZeroCoreModule

Module Zero core for multi-tenancy, authorization, users, roles:

```csharp
[DependsOn(typeof(AbpZeroCoreModule))]
public class MyModule : AbpModule
{
    // Gets multi-tenancy, authorization, user management
}
```

## Module Configuration

### Defining Module Configuration

```csharp
public class MyBlogModuleSettings
{
    public bool EnableFeature { get; set; }
    public int MaxPostCount { get; set; }
}
```

### Using Module Configuration

```csharp
[DependsOn(typeof(AbpKernelModule))]
public class MyBlogModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Settings.Providers.Add<MyBlogSettingProvider>();
    }
}
```

### Setting Provider

```csharp
public class MyBlogSettingProvider : SettingProvider
{
    public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
    {
        return new[]
        {
            new SettingDefinition(
                "MyBlog.MaxPostCount",
                "10",
                scopes: SettingScopes.Application | SettingScopes.Tenant
            )
        };
    }
}
```

## Dependency Injection in Modules

### Conventional Registration

```csharp
public override void Initialize()
{
    IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
}
```

This automatically registers:
- `ITransientDependency` as transient
- `ISingletonDependency` as singleton
- `IScopedDependency` as scoped

### Manual Registration

```csharp
public override void Initialize()
{
    IocManager.Register<IMyService, MyService>(DependencyLifeStyle.Transient);
    IocManager.Register<IMySingletonService, MySingletonService>(DependencyLifeStyle.Singleton);
}
```

### Interceptors

```csharp
public override void PreInitialize()
{
    IocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
    {
        if (typeof(IApplicationService).IsAssignableFrom(handler.ComponentModel.Implementation))
        {
            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuthorizationInterceptor)));
        }
    };
}
```

## PlugIn Modules

### Dynamic Module Loading (ASP.NET Core)

```csharp
public class Startup
{
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddAbp<MyStartupModule>(options =>
        {
            options.PlugInSources.AddFolder(@"C:\MyPlugIns");
        });
    }
}
```

### Dynamic Module Loading (ASP.NET MVC)

```csharp
public class MvcApplication : AbpWebApplication<MyStartupModule>
{
    protected override void Application_Start(object sender, EventArgs e)
    {
        AbpBootstrapper.PlugInSources.AddFolder(@"C:\MyPlugIns");
        base.Application_Start(sender, e);
    }
}
```

### Controllers in PlugIns (ASP.NET MVC)

```csharp
using System.Web;
using Abp.PlugIns;
using Abp.Web;

[assembly: PreApplicationStartMethod(typeof(PreStarter), "Start")]

namespace MyDemoApp.Web
{
    public class MvcApplication : AbpWebApplication<MyStartupModule> { }
    
    public static class PreStarter
    {
        public static void Start()
        {
            MvcApplication.AbpBootstrapper.PlugInSources.AddFolder(@"C:\MyPlugIns\");
            MvcApplication.AbpBootstrapper.PlugInSources.AddToBuildManager();
        }
    }
}
```

## Custom Module Methods

Modules can have custom methods that other modules can call:

```csharp
public class MyModule1 : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
    
    public void MyModuleMethod1()
    {
        // Custom method
    }
}

[DependsOn(typeof(MyModule1))]
public class MyModule2 : AbpModule
{
    private readonly MyModule1 _myModule1;
    
    public MyModule2(MyModule1 myModule1)
    {
        _myModule1 = myModule1;
    }
    
    public override void PreInitialize()
    {
        _myModule1.MyModuleMethod1();
    }
    
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

## Additional Assemblies

Override `GetAdditionalAssemblies` to include additional assemblies:

```csharp
public override void Initialize()
{
    IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
}

public override Assembly[] GetAdditionalAssemblies()
{
    return new[] { typeof(MyExternalClass).GetAssembly() };
}
```

## Common Module Patterns

### Data Access Module

```csharp
[DependsOn(typeof(AbpEntityFrameworkModule))]
public class MyDataModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = "Default";
        
        Configuration.Modules.AbpEfCore().AddDbContext<MyDbContext>(options =>
        {
            if (options.ExistingConnection != null)
            {
                options.DbContextOptions.UseSqlServer(options.ExistingConnection);
            }
            else
            {
                options.DbContextOptions.UseSqlServer(Configuration.DefaultNameOrConnectionString);
            }
        });
    }
    
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### Application Service Module

```csharp
[DependsOn(typeof(AbpKernelModule), typeof(AbpAutoMapperModule))]
public class MyApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
        {
            configuration.CreateMap<User, UserDto>();
            configuration.CreateMap<CreateUserDto, User>();
        });
    }
    
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### Web Module

```csharp
[DependsOn(typeof(AbpAspNetCoreModule))]
public class MyWebModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

## Module Best Practices

### Naming Conventions
- Module class: `[ModuleName]Module` (e.g., `MyBlogModule`)
- Assembly: `[Project].[Module]` (e.g., `MyProject.Blog`)
- Namespace: `[Project].[Module]` (e.g., `MyProject.Blog`)

### Dependency Management
- Declare all dependencies explicitly with `DependsOn`
- Avoid circular dependencies
- Keep module graph shallow
- Group related functionality in modules

### Initialization Order
- Use `PreInitialize` for configuration
- Use `Initialize` for DI registration
- Use `PostInitialize` for initialization that requires DI
- Keep initialization logic simple

### Configuration
- Use setting providers for configuration
- Provide sensible defaults
- Document all configuration options
- Validate configuration values

## Common Issues and Solutions

### Module Not Found

```csharp
// Ensure module is registered
[DependsOn(typeof(MyModule))]
public class MyApplicationModule : AbpModule
{
    // ...
}
```

### Dependency Injection Not Working

```csharp
// Ensure conventional registration is called
public override void Initialize()
{
    IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
}
```

### Configuration Not Applied

```csharp
// Ensure configuration is in PreInitialize
public override void PreInitialize()
{
    Configuration.Modules.AbpAutoMapper().Configurators.Add(...);
}
```

### Module Lifecycle Issues

```csharp
// Don't resolve dependencies in Initialize
// Use PostInitialize instead
public override void PostInitialize()
{
    var service = IocManager.Resolve<IMyService>();
    service.Initialize();
}
```

## When in Doubt

- Follow ABP conventions
- Use `DependsOn` for all module dependencies
- Keep modules focused and small
- Use conventional registration when possible
- Configure in `PreInitialize`, register in `Initialize`, initialize in `PostInitialize`
- Test module initialization independently
- Document module dependencies
- Check ABP documentation at https://aspnetboilerplate.com/Pages/Documents
