# EAF
## Enterprise Application Foundation (EAF)

Using Abp, but the optimization works with the latest ASP.NET Core and EF Core and focused only on ASP NET 9, with the aspzero-based middleware.
Replacing log4net with serilog, implemented OpenTelemetry and KeyVault from Azure or Oracle.
Hangfire tweaks for better log management with hangfire.cosole, among other tweaks.

Angular 18


![GitHub](https://img.shields.io/github/license/afonsoft/eaf) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/)

### What is the EAF?
Enterprise Application Foundation (**EAF**) is a middleware responsible for managing audit login mfa among others, based on aspzero.


Based in **[ASP.NET Boilerplate (ABP)](https://aspnetboilerplate.com/)** but made  with Middleware component. 

| Code Smell | Bugs | Tests | Lang | Quality |
| ------ | ------ | ------ | ------ | ------ |
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=code_smells)](https://sonarcloud.io/dashboard?id=EAF) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=bugs)](https://sonarcloud.io/dashboard?id=EAF) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=alert_status)](https://sonarcloud.io/dashboard?id=EAF) |

### STATISTICS
| | | | |
| ------ | ------ | ------ | ------ |
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=ncloc)](https://sonarcloud.io/dashboard?id=EAF) | [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=EAF) | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=coverage)](https://sonarcloud.io/dashboard?id=EAF) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=EAF) | 
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=EAF) | [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=security_rating)](https://sonarcloud.io/dashboard?id=EAF) | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=sqale_index)](https://sonarcloud.io/dashboard?id=EAF) | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=EAF)

### DOWNLOAD

![GitHub all releases](https://img.shields.io/github/downloads/afonsoft/eaf/total)

### ISSUES

![GitHub issues](https://img.shields.io/github/issues-raw/afonsoft/eaf)

### A Quick Sample

Let's investigate a simple class to see EAF's benefits:

    public class TaskAppService : ApplicationService, ITaskAppService
    {
        private readonly IRepository<Task> _taskRepository;

        public TaskAppService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [AbpAuthorize(MyPermissions.UpdateTasks)]
        public async Task UpdateTask(UpdateTaskInput input)
        {
            Logger.Info("Updating a task for input: " + input);

            var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
            if (task == null)
            {
                throw new UserFriendlyException(L("CouldNotFindTheTaskMessage"));
            }

            ObjectMapper.MapTo(input, task);
        }
    }

Here we see a sample [Application Service](https://aspnetboilerplate.com/Pages/Documents/Application-Services) method. An application service, in DDD,
is directly used by the presentation layer to perform the **use cases** of the application.
Think **UpdateTask** as a method that is called by JavaScript via AJAX.

Let's see some of ABP's benefits here:

-   **[Dependency Injection](https://aspnetboilerplate.com/Pages/Documents/Dependency-Injection)**: Abp uses and provides a conventional DI infrastructure.
    Since this class is an application service, it's conventionally
    registered to the DI container as transient (created per request). It
    can simply inject any dependencies (such as the IRepository&lt;Task&gt; in
    this sample).
-   **[Repository](https://aspnetboilerplate.com/Pages/Documents/Repositories)**: Abp can create a default repository for each entity (such as IRepository&lt;Task&gt; in
    this example). The default repository has many useful methods such as the
    FirstOrDefault method used in this example. We can extend the default
    repository to suit our needs. Repositories abstract the DBMS and ORMs and
    simplify the data access logic.
-   **[Authorization](https://aspnetboilerplate.com/Pages/Documents/Authorization)**: Abp can check permissions declaratively.
    It prevents access to the UpdateTask method if the current user
    has no "update tasks" permission or is not logged in. Abp not only uses declarative
    attributes, but it also has additional ways in which you can authorize.
-   **[Validation](https://aspnetboilerplate.com/Pages/Documents/Validating-Data-Transfer-Objects)**: Abp automatically checks if the input is null. It also validates all
    the properties of an input based on standard data annotation attributes
    and custom validation rules. If a request is not valid, it throws a
    proper validation exception and handles it in the client side.
-   **[Audit Logging](https://aspnetboilerplate.com/Pages/Documents/Audit-Logging)** : User, browser, IP address, calling service, method, parameters, calling time,
    execution duration and some other information is automatically
    saved for each request based on conventions and configurations.
-   **[Unit Of Work](https://aspnetboilerplate.com/Pages/Documents/Unit-Of-Work)**: In Abp, each application service method is assumed to be a unit of work by default.
    It automatically creates a connection and begins a transaction at
    the beginning of the method. If the method successfully completes
    without an exception, then the transaction is committed and the connection
    is disposed. Even if this method uses different repositories or
    methods, all of them will be atomic (transactional). All changes
    on entities are automatically saved when a transaction is committed.
    We don't even need to call the \_repository.Update(task) method as
    shown above.
-   **[Exception Handling](https://aspnetboilerplate.com/Pages/Documents/Handling-Exceptions)**: We almost never have to manually handle exceptions in Abp on a web application. All exceptions are automatically handled by default! If an exception occurs, Abp automatically logs it and returns a proper result to the client. For example, if this is an AJAX request, it returns a JSON object to the client indicating that an error occurred. It hides the actual exception from the client unless the exception is a  UserFriendlyException, as used in this sample. It also understands  and handles errors on the client side and show appropriate messages to the users.
-   **[Logging](https://aspnetboilerplate.com/Pages/Documents/Logging)**: As you see, we can write logs using the Logger object defined in the base class.
    Log4Net or SerilLog is used by default, but it's changeable and configurable.
-   **[Localization](https://aspnetboilerplate.com/Pages/Documents/Localization)**: Note that we used the 'L' method while throwing the exception?
    This way, it's automatically localized based on the current user's culture. See the [localization](https://aspnetboilerplate.com/Pages/Documents/Localization) document for more.
-   **[Auto Mapping](/Pages/Documents/Data-Transfer-Objects)**: In the last line, we map input using the MapTo method of EAF's IObjectMapper.
    properties to entity properties. It uses the AutoMapper library to
    perform the mapping. We can easily map properties from one object
    to another based on naming conventions.
-   **[Dynamic API Layer](https://aspnetboilerplate.com/Pages/Documents/Data-Transfer-Objects)**: TaskAppService is a simple class, actually. We generally have to write a wrapper API Controller to expose methods to JavaScript clients, but EAF automatically does that on runtime. This way, we can use applicationservice methods directly from clients.
-   **[Dynamic JavaScript AJAX Proxy](https://aspnetboilerplate.com/Pages/Documents/Dynamic-Web-API#dynamic-javascript-proxies)** : EAF creates proxy methods that make calling application
    service methods as simple as calling JavaScript methods on the client.



Besides this simple example, ABP provides a strong infrastructure and development model for
[modularity](https://aspnetboilerplate.com/Pages/Documents/Module-System), [multi-tenancy](https://aspnetboilerplate.com/Pages/Documents/Multi-Tenancy), [caching](https://aspnetboilerplate.com/Pages/Documents/Caching), [background jobs](https://aspnetboilerplate.com/Pages/Documents/Background-Jobs-And-Workers), [data filters](https://aspnetboilerplate.com/Pages/Documents/Data-Filters), [setting management](https://aspnetboilerplate.com/Pages/Documents/Setting-Management), [domain events](https://aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events), unit & integration testing and so on... You focus on your business code and don't repeat yourself!

## Packages Nuget

| Package | Nuget |
| ------ | ------ |
| [Eaf.Middleware.Application](https://www.nuget.org/packages/Eaf.Middleware.Application/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Application.svg)](https://badge.fury.io/nu/Eaf.Middleware.Application) |
| [Eaf.Middleware.AzureActiveDirectory](https://www.nuget.org/packages/Eaf.Middleware.AzureActiveDirectory/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory.svg)](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory) |
| [Eaf.Middleware.Core](https://www.nuget.org/packages/Eaf.Middleware.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Core) |
| [Eaf.Middleware.Ldap](https://www.nuget.org/packages/Eaf.Middleware.Ldap/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Ldap.svg)](https://badge.fury.io/nu/Eaf.Middleware.Ldap) |
| [Eaf.Middleware.Web.Core](https://www.nuget.org/packages/Eaf.Middleware.Web.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Web.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Web.Core) |
| [Eaf.Castle.Serilog](https://www.nuget.org/packages/Eaf.Castle.Serilog/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Castle.Serilog.svg)](https://badge.fury.io/nu/Eaf.Castle.Serilog) |
| [Eaf.KeyVault](https://www.nuget.org/packages/Eaf.KeyVault/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.svg)](https://badge.fury.io/nu/Eaf.KeyVault) |
| [Eaf.KeyVault.AspNetCore](https://www.nuget.org/packages/Eaf.KeyVault.AspNetCore/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore.svg)](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore) |
| [Eaf.OpenTelemetry](https://www.nuget.org/packages/Eaf.OpenTelemetry/) | [![NuGet version](https://badge.fury.io/nu/Eaf.OpenTelemetry.svg)](https://badge.fury.io/nu/Eaf.OpenTelemetry) |
| [Eaf.Log4NetServiceBus](https://www.nuget.org/packages/Eaf.Log4NetServiceBus/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Log4NetServiceBus.svg)](https://badge.fury.io/nu/Eaf.Log4NetServiceBus) |
| [Eaf.SqlServerCache](https://www.nuget.org/packages/Eaf.SqlServerCache/) | [![NuGet version](https://badge.fury.io/nu/Eaf.SqlServerCache.svg)](https://badge.fury.io/nu/Eaf.SqlServerCache) |


[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/project/overview?id=afonsoft_EAF)
