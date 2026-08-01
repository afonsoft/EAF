---
name: eaf-api
description: Expert guidance for developing, maintaining, and extending EAF (Enterprise Application Foundation) RESTful APIs using ASP.NET Core, ABP framework, and EAF middleware modules. Covers dynamic API generation, DTO patterns, authentication, authorization, caching, Swagger/OpenAPI documentation, and EAF-specific API patterns like KeyVault, OpenTelemetry, and cache integration. Use this skill when creating REST APIs, working with Application Services, implementing authentication/authorization, configuring Swagger, or troubleshooting API endpoints. Do NOT use for frontend development, database migrations, or non-API backend work.
metadata:
  version: '1.0.0'
---

# EAF API Development Skill

You are an expert in EAF (Enterprise Application Foundation) API development. You develop, maintain, and extend RESTful APIs using ASP.NET Core, ABP framework, and EAF middleware modules. You write functional, maintainable, performant, and scalable APIs following REST best practices.

## Project Context

EAF is an open source middleware platform built on ASP.NET Boilerplate (ABP). The API layer provides RESTful endpoints for consuming EAF functionality.

### Technology Stack
- **.NET Version**: 10.0
- **ASP.NET Core**: 10.0
- **ABP Version**: 10.5.0
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Authentication**: JWT Bearer, Cookie-based, Azure AD, LDAP
- **Authorization**: ABP Permission System
- **Serialization**: JSON (System.Text.Json)

### API Architecture

EAF follows the ABP layered architecture for APIs:
- **Controllers**: HTTP endpoints (optional, ABP uses dynamic API generation)
- **Application Services**: Business logic and DTOs (exposed as API automatically)
- **Domain Services**: Business logic
- **Repositories**: Data access

## Dynamic API Generation

ABP automatically generates REST APIs from Application Services. You typically don't need to write Controllers.

### Application Service as API

```csharp
public class UserAppService : ApplicationService, IUserAppService
{
    private readonly IRepository<User, Guid> _userRepository;
    
    public UserAppService(IRepository<User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
    
    [AbpAuthorize("Pages.Users.Create")]
    public async Task CreateUserDto CreateUser(CreateUserDto input)
    {
        var user = ObjectMapper.Map<User>(input);
        await _userRepository.InsertAsync(user);
        return ObjectMapper.Map<UserDto>(user);
    }
    
    public async Task<UserDto> GetUser(EntityDto<Guid> input)
    {
        var user = await _userRepository.GetAsync(input.Id);
        return ObjectMapper.Map<UserDto>(user);
    }
    
    public async Task<PagedResultDto<UserDto>> GetAll(GetAllUsersInput input)
    {
        var query = _userRepository.GetAll();
        
        // Filtering
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x => x.Name.Contains(input.Filter));
        }
        
        // Sorting
        query = query.OrderBy(x => x.Name);
        
        // Paging
        var totalCount = await AsyncQueryableExecuter.CountAsync(query);
        var items = await AsyncQueryableExecuter.ToListAsync(
            query.PageBy(input)
        );
        
        return new PagedResultDto<UserDto>(
            totalCount,
            ObjectMapper.Map<List<UserDto>>(items)
        );
    }
}
```

This service is automatically exposed as:
- `POST /api/services/app/user/create`
- `GET /api/services/app/user/{id}`
- `GET /api/services/app/user/all`

## DTO Patterns

### Input DTOs

```csharp
public class CreateUserDto
{
    [Required]
    [StringLength(UserConsts.MaxNameLength)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(UserConsts.MaxPasswordLength)]
    public string Password { get; set; }
}
```

### Output DTOs

```csharp
public class UserDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreationTime { get; set; }
}
```

### Paged Input DTOs

```csharp
public class GetAllUsersInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
```

### Paged Result DTOs

```csharp
public class PagedResultDto<T>
{
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; }
}
```

## API Configuration

### Swagger/OpenAPI Configuration

```csharp
[DependsOn(typeof(AbpAspNetCoreModule))]
public class EafWebCoreModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        
        Configuration.Modules.AbpAspNetCore()
            .CreateControllersForAppServices(
                typeof(EafApplicationModule).GetAssembly()
            );
    }
    
    public override void PreInitialize()
    {
        Configuration.Modules.AbpAspNetCore()
            .CreateControllersForAppServices(
                typeof(EafApplicationModule).GetAssembly()
            );
    }
}
```

### Startup Configuration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAbpAspNetCore<EafWebHostModule>(options =>
        {
            options.UseConventionalUrlCreation = true;
        });
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "EAF API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAbp();
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EAF API V1");
        });
    }
}
```

## Authentication & Authorization

### JWT Authentication

```csharp
public class TokenAuthController : EafControllerBase
{
    private readonly TokenAuthController _tokenAuthController;
    
    [HttpPost]
    public async Task<AuthenticateResultModel> Authenticate(AuthenticateModel model)
    {
        var loginResult = await _tokenAuthController.GetResult(
            new AuthenticateModel
            {
                UserNameOrEmailAddress = model.UserNameOrEmailAddress,
                Password = model.Password
            }
        );
        
        return loginResult;
    }
}
```

### Authorization Attributes

```csharp
[AbpAuthorize("Pages.Users.Create")]
public async Task CreateUserDto CreateUser(CreateUserDto input)
{
    // Only users with "Pages.Users.Create" permission can access
}

[AbpAuthorize]
public async Task GetAllUsers()
{
    // Only authenticated users can access
}

public async Task PublicMethod()
{
    // No authorization required
}
```

### Permission Checking Programmatically

```csharp
public class MyService : ITransientDependency
{
    private readonly IPermissionChecker _permissionChecker;
    
    public MyService(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }
    
    public async Task<bool> CanDeleteUser()
    {
        return await _permissionChecker.IsGrantedAsync("Pages.Users.Delete");
    }
}
```

## API Response Patterns

### Success Response

```csharp
public class UserAppService : ApplicationService
{
    public async Task<UserDto> GetUser(EntityDto<Guid> input)
    {
        var user = await _userRepository.GetAsync(input.Id);
        return ObjectMapper.Map<UserDto>(user);
    }
}
```

Response:
```json
{
  "result": {
    "id": "guid",
    "name": "John Doe",
    "email": "john@example.com"
  },
  "success": true,
  "error": null
}
```

### Error Response

```csharp
public class UserAppService : ApplicationService
{
    public async Task<UserDto> GetUser(EntityDto<Guid> input)
    {
        var user = await _userRepository.FirstOrDefaultAsync(input.Id);
        if (user == null)
        {
            throw new UserFriendlyException("User not found");
        }
        return ObjectMapper.Map<UserDto>(user);
    }
}
```

Response:
```json
{
  "result": null,
  "success": false,
  "error": {
    "code": 0,
    "message": "User not found",
    "details": null,
    "validationErrors": null
  }
}
```

## EAF-Specific API Patterns

### Using EAF Middleware in APIs

#### KeyVault Integration

```csharp
public class SecretAppService : ApplicationService
{
    private readonly IKeyVaultSecretManager _keyVaultManager;
    
    public SecretAppService(IKeyVaultSecretManager keyVaultManager)
    {
        _keyVaultManager = keyVaultManager;
    }
    
    public async Task<string> GetSecret(string secretName)
    {
        var secret = await _keyVaultManager.GetSecretAsync(secretName);
        return secret.Value;
    }
}
```

#### OpenTelemetry Integration

```csharp
public class MyApiService : ApplicationService
{
    private readonly ActivitySource _activitySource;
    
    public MyApiService()
    {
        _activitySource = new ActivitySource("MyApiService");
    }
    
    public async Task DoWorkAsync()
    {
        using var activity = _activitySource.StartActivity("DoWork");
        activity?.SetTag("operation", "important");
        
        // Your code here
    }
}
```

#### Cache Integration

```csharp
public class CachedUserService : ApplicationService
{
    private readonly ICacheManager _cacheManager;
    private readonly IRepository<User, Guid> _userRepository;
    
    public CachedUserService(
        ICacheManager cacheManager,
        IRepository<User, Guid> userRepository)
    {
        _cacheManager = cacheManager;
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> GetUser(EntityDto<Guid> input)
    {
        var cache = _cacheManager.GetCache<UserDto>("Users");
        return await cache.GetAsync(input.Id.ToString(), 
            () => GetUserFromDb(input.Id));
    }
    
    private async Task<UserDto> GetUserFromDb(Guid id)
    {
        var user = await _userRepository.GetAsync(id);
        return ObjectMapper.Map<UserDto>(user);
    }
}
```

## API Best Practices

### Versioning

Use URL path versioning:
```
/api/v1/users
/api/v2/users
```

### Filtering, Sorting, Paging

```csharp
public class GetAllUsersInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public async Task<PagedResultDto<UserDto>> GetAll(GetAllUsersInput input)
{
    var query = _userRepository.GetAll();
    
    // Filter
    if (!string.IsNullOrWhiteSpace(input.Filter))
    {
        query = query.Where(x => x.Name.Contains(input.Filter));
    }
    
    // Sort
    query = input.SortDescending
        ? query.OrderByDescending(x => x.Name)
        : query.OrderBy(x => x.Name);
    
    // Page
    var totalCount = await AsyncQueryableExecuter.CountAsync(query);
    var items = await AsyncQueryableExecuter.ToListAsync(
        query.PageBy(input)
    );
    
    return new PagedResultDto<UserDto>(
        totalCount,
        ObjectMapper.Map<List<UserDto>>(items)
    );
}
```

### Validation

```csharp
public class CreateUserDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }
}
```

### Error Handling

```csharp
public class UserAppService : ApplicationService
{
    public async Task<UserDto> GetUser(EntityDto<Guid> input)
    {
        try
        {
            var user = await _userRepository.GetAsync(input.Id);
            return ObjectMapper.Map<UserDto>(user);
        }
        catch (EntityNotFoundException ex)
        {
            throw new UserFriendlyException(L("UserNotFoundMessage"));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error getting user");
            throw;
        }
    }
}
```

## Testing APIs

### Integration Tests

```csharp
public class UserAppService_Tests : AbpIntegratedTestBase
{
    private readonly IUserAppService _userAppService;
    
    public UserAppService_Tests()
    {
        _userAppService = Resolve<IUserAppService>();
    }
    
    [Fact]
    public async Task Should_Create_User()
    {
        // Arrange
        var input = new CreateUserDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "123456"
        };
        
        // Act
        var result = await _userAppService.CreateUser(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("John Doe");
        result.Email.ShouldBe("john@example.com");
    }
}
```

### API Endpoint Testing

```csharp
public class UsersController_Tests : AbpWebTestBase
{
    [Fact]
    public async Task Should_Get_User()
    {
        // Act
        var response = await GetResponseAsObjectAsync<UserDto>(
            $"/api/services/app/user/{userId}"
        );
        
        // Assert
        response.ShouldNotBeNull();
        response.Name.ShouldBe("John Doe");
    }
}
```

## Common API Patterns

### CRUD Operations

```csharp
public class UserAppService : ApplicationService, IUserAppService
{
    // CREATE
    public async Task<UserDto> Create(CreateUserDto input)
    {
        var user = ObjectMapper.Map<User>(input);
        await _userRepository.InsertAsync(user);
        return ObjectMapper.Map<UserDto>(user);
    }
    
    // READ
    public async Task<UserDto> Get(EntityDto<Guid> input)
    {
        var user = await _userRepository.GetAsync(input.Id);
        return ObjectMapper.Map<UserDto>(user);
    }
    
    // UPDATE
    public async Task<UserDto> Update(UpdateUserDto input)
    {
        var user = await _userRepository.GetAsync(input.Id);
        ObjectMapper.Map(input, user);
        await _userRepository.UpdateAsync(user);
        return ObjectMapper.Map<UserDto>(user);
    }
    
    // DELETE
    public async Task Delete(EntityDto<Guid> input)
    {
        await _userRepository.DeleteAsync(input.Id);
    }
    
    // LIST
    public async Task<PagedResultDto<UserDto>> GetAll(GetAllUsersInput input)
    {
        var query = _userRepository.GetAll();
        var totalCount = await AsyncQueryableExecuter.CountAsync(query);
        var items = await AsyncQueryableExecuter.ToListAsync(
            query.PageBy(input)
        );
        return new PagedResultDto<UserDto>(
            totalCount,
            ObjectMapper.Map<List<UserDto>>(items)
        );
    }
}
```

### Batch Operations

```csharp
public class UserAppService : ApplicationService
{
    public async Task BulkDelete(BulkDeleteUsersInput input)
    {
        foreach (var userId in input.UserIds)
        {
            await _userRepository.DeleteAsync(userId);
        }
    }
}
```

### Export/Import

```csharp
public class UserAppService : ApplicationService
{
    public async Task<FileDto> ExportToExcel()
    {
        var users = await _userRepository.GetAllListAsync();
        
        var excelFile = _excelExporter.ExportToExcel(
            new List<ExcelColumnDefinition>
            {
                new ExcelColumnDefinition("Name", typeof(string)),
                new ExcelColumnDefinition("Email", typeof(string))
            },
            users.Select(u => new object[] { u.Name, u.Email }).ToList()
        );
        
        return new FileDto(excelFile.FileName, MimeTypeNames.ApplicationVndOpenXml);
    }
}
```

## Performance Optimization

### Caching API Responses

```csharp
public class UserAppService : ApplicationService
{
    private readonly ICacheManager _cacheManager;
    
    [AbpAuthorize("Pages.Users")]
    [CacheAspect(Duration = 300)] // 5 minutes cache
    public async Task<List<UserDto>> GetAll()
    {
        var users = await _userRepository.GetAllListAsync();
        return ObjectMapper.Map<List<UserDto>>(users);
    }
}
```

### Async/Await

Always use async/await for I/O operations:
```csharp
// GOOD
public async Task<UserDto> GetUser(Guid id)
{
    var user = await _userRepository.GetAsync(id);
    return ObjectMapper.Map<UserDto>(user);
}

// BAD - synchronous
public UserDto GetUser(Guid id)
{
    var user = _userRepository.Get(id);
    return ObjectMapper.Map<UserDto>(user);
}
```

### Database Query Optimization

```csharp
// GOOD - Project only needed fields
public async Task<List<UserDto>> GetAll()
{
    var query = from user in _userRepository.GetAll()
                select new { user.Id, user.Name, user.Email };
    
    var result = await AsyncQueryableExecuter.ToListAsync(query);
    return result.Select(x => new UserDto
    {
        Id = x.Id,
        Name = x.Name,
        Email = x.Email
    }).ToList();
}

// BAD - Fetches all fields
public async Task<List<UserDto>> GetAll()
{
    var users = await _userRepository.GetAllListAsync();
    return ObjectMapper.Map<List<UserDto>>(users);
}
```

## Security Best Practices

### Input Validation

```csharp
public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$")]
    public string Name { get; set; }
}
```

### SQL Injection Prevention

Use parameterized queries (ABP repositories handle this automatically):
```csharp
// SAFE - ABP repository handles parameterization
var users = await _userRepository.GetAllListAsync(
    x => x.Name == input.Name
);

// NEVER do this (SQL injection risk)
var sql = $"SELECT * FROM Users WHERE Name = '{input.Name}'";
```

### Authorization

Always check permissions:
```csharp
[AbpAuthorize("Pages.Users.Delete")]
public async Task DeleteUser(EntityDto<Guid> input)
{
    await _userRepository.DeleteAsync(input.Id);
}
```

### HTTPS

Always use HTTPS in production:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001"
      }
    }
  }
}
```

## Documentation

### XML Documentation

```csharp
/// <summary>
/// Creates a new user.
/// </summary>
/// <param name="input">User creation input DTO</param>
/// <returns>Created user DTO</returns>
[AbpAuthorize("Pages.Users.Create")]
public async Task<UserDto> CreateUser(CreateUserDto input)
{
    // Implementation
}
```

### Swagger Documentation

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EAF API",
                Version = "v1",
                Description = "EAF (Enterprise Application Foundation) API Documentation",
                Contact = new OpenApiContact
                {
                    Name = "EAF Team",
                    Url = "https://github.com/afonsoft/EAF"
                }
            });
            
            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }
}
```

## Common Issues and Solutions

### CORS Issues

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseCors("AllowAll");
}
```

### Large Payloads

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = 104857600; // 100MB
    });
}
```

### Timeout Issues

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers(options =>
    {
        options.Filters.Add(new RequestTimeoutAttribute(300)); // 5 minutes
    });
}
```

## When in Doubt

- Follow ABP conventions for API development
- Use Application Services instead of Controllers when possible
- Always validate input
- Use async/await for I/O operations
- Add XML documentation to public APIs
- Test APIs thoroughly
- Keep APIs RESTful and resource-oriented
- Use appropriate HTTP verbs (GET, POST, PUT, DELETE)
- Return appropriate HTTP status codes
