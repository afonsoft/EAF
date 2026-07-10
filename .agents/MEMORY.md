# EAF Coverage Audit Memory

Last session branch: `devin/1783717794-priority27-coverage-audit`
Baseline coverage (P26): Line 68.0%, Branch 52.1%, Method 86.4%.
Current coverage (after P27): Line 75.6%, Branch 54.0%, Method 90.8%.

## Mocking gotchas
- `UserManager.GetUserByLoginAsync(string userName, int? tanantId)` is non-virtual; cannot be mocked with `NSubstitute.Returns`. Tests must rely on the underlying `_userRepository` substitute defaulting to null.
- `AbpUserManager.GetOldUserNameAsync` is protected virtual; the admin-rename branch in `UserManager.UpdateWithValidateAsync` is not reachable with NSubstitute without reflection.
- `IEmailSender.SendAsync` returns `Task` non-generic. To simulate failure, use `emailSender.SendAsync(...).Returns(Task.FromException(new Exception(...)))` — `Throws`/`ThrowsAsync` from `NSubstitute.ExceptionExtensions` is not applicable.
- `SimpleStringCipher.Instance.Encrypt` defaults to `SimpleStringCipher.DefaultPassPhrase` (`gsKnGZ041HLL4IM8`). Web.Core/Worker classes that decrypt token/userId use `MiddlewareCoreConsts.DefaultPassPhrase` (`gsKxGZ012HLL3MI5`). Tests must pass the correct passphrase to `Encrypt`.
- `PerformContext` has no parameterless constructor; create a real instance with `new PerformContext(null, Substitute.For<IStorageConnection>(), new BackgroundJob("id", null, DateTime.UtcNow), Substitute.For<IJobCancellationToken>())`.
- `SmtpClient` is not easily mocked with `NSubstitute` because `Authenticate`/`Connect` are non-virtual/intercept complex. Prefer a `TestableSmtpClient : SmtpClient` that overrides `Authenticate(Encoding, ICredentials, ct)` and `Connect(...)`.
- `BinaryObject` constructor signature is `(int? tenantId, byte[] bytes, string fileType, string fileName)`; the `Id` is generated, so tests that need a specific `Id` must set `binaryObject.Id = fileId` after construction.
- `IApplicationBuilder.UseHealthChecks(...)` is an extension method that resolves `IEnumerable<IHealthCheckService>` from `ApplicationServices`; do not mock `UseHealthChecks` directly on a substitute `IApplicationBuilder` — it will leak `Arg.Any` specs and fail.
- `ActionDescriptor.Id` is read-only and derived from `RouteValues`; use `ActionDescriptor.RouteValues` with `Dictionary<string, string?>` and assert `IDiagnosticContext.Set` with `Arg.Any<string>()` for the id.
- `EafServiceCollectionExtensions.AddEaf` bootstraps a full ABP/Castle pipeline; always use `options.IocManager = new IocManager()` in tests to avoid duplicate component registration in the static `IocManager.Instance`.
- `EafHangfireApplicationBuilderExtensions.UseEafHangfire` requires `IHostApplicationLifetime`, `Hangfire.Dashboard.RouteCollection` and `JobStorage` to be registered in the service provider.
- `IApplicationBuilder.Map` is an extension method, not a mockable interface member; assert `IApplicationBuilder.New()` and `IApplicationBuilder.Use(...)` when verifying Hangfire/Map middleware setup.

## Coverage command
- `bash run-tests-with-coverage.sh` requires `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet` because the script does not export `DOTNET_ROOT`.
- `reportgenerator` (global tool) is required to consolidate the `coverage.cobertura.xml` files. If missing, install with `dotnet tool install -g dotnet-reportgenerator-globaltool`.

## Notable classes with remaining low coverage (target for P28)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (0%)
- `Eaf.Middleware.Web.Configuration.CacheConfigurer` (41.1%)
- `Eaf.Middleware.Web.Swagger.SwaggerExtensions` (33.3%)
- `Eaf.Middleware.Configuration.EafHostBuilderExtensions` (40.7%)
- `Eaf.Middleware.Web.Configuration.EafHostBuilderExtensions` (0%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (18.6%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (19%)
- `Eaf.Hangfire.EafDisplayNameExtensions` (0%)
- `Eaf.Hangfire.EafHangfireConfigurationExtensions` (0%)
- `Eaf.Middleware.Core.Authentication.External.*` providers (0%)
- `Eaf.Middleware.Identity.LogInManager`, `SecurityStampValidator`, `SignInManager` (0%)
- `Eaf.AspNetCore.SignalR.Chat.ChatHub` (0%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
- `Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions` (0%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
- `Eaf.KeyVault.*` modules (0-19%)
