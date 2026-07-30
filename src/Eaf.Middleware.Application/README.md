# Eaf.Middleware.Application

## Descrição Técnica

O **Eaf.Middleware.Application** é a camada de aplicação do Enterprise Application Foundation (EAF). Este módulo fornece DTOs (Data Transfer Objects), serviços de aplicação, validações e lógica de negócio intermediária, servindo como ponte entre a camada de domínio (Core) e a camada de apresentação (Web).

Este módulo segue os padrões de Application Layer do Domain-Driven Design (DDD) e implementa o padrão CQRS (Command Query Responsibility Segregation) através de serviços de aplicação.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp.AutoMapper**: Configuração automática de mapeamento entre entidades e DTOs
- **Abp.MailKit**: Integração para envio de emails
- **Abp.Web.Common**: Funcionalidades web comuns
- **EPPlus**: Geração de arquivos Excel
- **MimeKit**: Processamento de emails
- **SixLabors.ImageSharp**: Processamento de imagens

### Dependências do EAF
- **Eaf.Middleware.Core**: Camada de domínio base

### Principais Componentes

#### Serviços de Aplicação
- Base classes para serviços de aplicação (`MiddlewareAppServiceBase`)
- Configuração de mapeamento de DTOs
- Validação automática de inputs

#### DTOs
- DTOs de entrada e saída
- Mapeamento automático com AutoMapper
- Validação de dados

#### Funcionalidades
- **Auditing**: Serviços de auditoria de aplicação
- **Authorization**: Serviços de autorização
- **Configuration**: Gerenciamento de configurações
- **Chat**: Serviços de chat
- **DataExporting**: Exportação de dados
- **Friendships**: Gerenciamento de amizades
- **Logging**: Serviços de logging
- **MultiTenancy**: Gerenciamento multi-tenant
- **Notifications**: Sistema de notificações
- **RealTime**: Comunicação em tempo real
- **Security**: Serviços de segurança
- **Sessions**: Gerenciamento de sessões
- **UiCustomization**: Customização de UI
- **WebHooks**: Integração com webhooks

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Eaf.Middleware.Core 9.4.1

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Application --version 9.4.1
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Application\Eaf.Middleware.Application.csproj" />
```

## Exemplo Básico de Uso

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareApplicationModule`:

```csharp
[DependsOn(
    typeof(MiddlewareCoreModule),
    typeof(MiddlewareApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class MyWebModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Criando um Serviço de Aplicação

```csharp
public class TaskAppService : MiddlewareAppServiceBase, ITaskAppService
{
    private readonly IRepository<Task> _taskRepository;

    public TaskAppService(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [AbpAuthorize(MyPermissions.UpdateTasks)]
    public async Task UpdateTask(UpdateTaskInput input)
    {
        Logger.Info($"Updating task {input.TaskId}");

        var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
        if (task == null)
        {
            throw new UserFriendlyException(L("TaskNotFound"));
        }

        ObjectMapper.Map(input, task);
    }

    public async Task<TaskDto> GetTask(GetTaskInput input)
    {
        var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
        return ObjectMapper.Map<TaskDto>(task);
    }
}
```

### 3. Definindo DTOs

```csharp
public class UpdateTaskInput
{
    [Required]
    public int TaskId { get; set; }

    [Required]
    [StringLength(Task.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(Task.MaxDescriptionLength)]
    public string Description { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
}
```

### 4. Configurando Mapeamento de DTOs

No arquivo `MiddlewareCustomDtoMapper.cs`:

```csharp
public class MiddlewareCustomDtoMapper : Profile
{
    public MiddlewareCustomDtoMapper()
    {
        CreateMap<Task, TaskDto>();
        CreateMap<CreateTaskInput, Task>();
        CreateMap<UpdateTaskInput, Task>();
    }
}
```

### 5. Usando Serviços de Notificação

```csharp
public class NotificationAppService : MiddlewareAppServiceBase, INotificationAppService
{
    private readonly INotificationPublisher _notificationPublisher;

    public NotificationAppService(INotificationPublisher notificationPublisher)
    {
        _notificationPublisher = notificationPublisher;
    }

    public async Task SendNotificationAsync(SendNotificationInput input)
    {
        await _notificationPublisher.PublishAsync(
            "MyNotification",
            new NotificationData(input.Message),
            userIds: new[] { input.TargetUserId }
        );
    }
}
```

### 6. Exportando Dados para Excel

```csharp
public class DataExportAppService : MiddlewareAppServiceBase, IDataExportAppService
{
    public async Task<FileDto> ExportTasksToExcel(GetTasksInput input)
    {
        var tasks = await _taskRepository.GetAllListAsync();

        var excelFile = new ExcelFileCreator();
        var file = excelFile.CreateExcelFile(tasks);

        return file;
    }
}
```

## Estrutura do Módulo

```
Eaf.Middleware.Application/
├── Auditing/              # Serviços de auditoria
├── Authorization/         # Serviços de autorização
├── Chat/                  # Serviços de chat
├── Common/                # Classes comuns
├── Configuration/         # Serviços de configuração
├── DataExporting/         # Exportação de dados
├── Dto/                   # DTOs base
├── Friendships/           # Serviços de amizades
├── Localization/          # Arquivos de localização
├── Logging/               # Serviços de logging
├── Maintenance/           # Serviços de manutenção
├── MultiTenancy/         # Serviços multi-tenant
├── Net/                   # Serviços de rede
├── Notifications/         # Sistema de notificações
├── RealTime/              # Comunicação em tempo real
├── Security/              # Serviços de segurança
├── Sessions/              # Gerenciamento de sessões
├── UiCustomization/       # Customização de UI
└── WebHooks/              # Integração com webhooks
```

## Configurações Opcionais

### Configuração de AutoMapper
```csharp
public override void Initialize()
{
    var thisAssembly = Assembly.GetExecutingAssembly();

    Configuration.Modules.AbpAutoMapper().Configurators.Add(
        cfg => cfg.AddMaps(thisAssembly)
    );
}
```

### Substituindo Serviços
```csharp
public override void PreInitialize()
{
    Configuration.ReplaceService<INotificationPublisher, MyNotificationPublisher>(
        DependencyLifeStyle.Transient
    );
}
```

## Testes

Atualmente, este módulo não possui testes unitários. Para criar testes, siga o padrão dos outros módulos do EAF:

```bash
# Criar projeto de teste
dotnet new xunit -n Eaf.Middleware.Application.Tests

# Adicionar referências
dotnet add Eaf.Middleware.Application.Tests reference ../src/Eaf.Middleware.Application
dotnet add Eaf.Middleware.Application.Tests package Abp.TestBase
dotnet add Eaf.Middleware.Application.Tests package Shouldly
dotnet add Eaf.Middleware.Application.Tests package NSubstitute
```

Exemplo de teste:

```csharp
public class TaskAppService_Tests : AppTestBase
{
    private readonly ITaskAppService _taskAppService;

    public TaskAppService_Tests()
    {
        _taskAppService = Resolve<ITaskAppService>();
    }

    [Fact]
    public async Task Should_Update_Task()
    {
        // Arrange
        var task = await CreateTaskAsync("Test Task");
        var input = new UpdateTaskInput
        {
            TaskId = task.Id,
            Title = "Updated Task"
        };

        // Act
        await _taskAppService.UpdateTask(input);

        // Assert
        var updatedTask = await GetTaskAsync(task.Id);
        updatedTask.Title.ShouldBe("Updated Task");
    }
}
```

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF