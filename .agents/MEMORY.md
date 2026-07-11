# EAF Coverage Audit Memory

Last session branch: `devin/1783727214-priority28-coverage-audit`
Baseline coverage (P27): Line 75.6%, Branch 54.0%, Method 90.8%.
Current coverage (after P28): Line 78.3%, Branch 56.1%, Method 92.8%.

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
- `IConfigurationRoot` substitutes return empty strings for `GetValue<T>`; use `ConfigurationBuilder().AddInMemoryCollection()` with `Dictionary<string, string?>` to supply real values.
- `CacheConfigurer` uses `bool.Parse` and `IConfigurationRoot.GetValue<int>`; `NSubstitute` defaults break `FormatException` without real config.
- `NSubstitute.Received(n).Property` returns the default value of the property, not the configured `Returns`; split the `Received` count assertion from the value assertion (e.g., `_ = sub.Received(1).Property; sub.Property.ShouldBeTrue();`).
- `Microsoft.Extensions.Diagnostics.HealthChecks` requires `AddOptions()` and `AddLogging()` in the `ServiceCollection` so `UseHealthChecks` can resolve `IOptions<HealthCheckServiceOptions>` and `ILogger<DefaultHealthCheckService>`.
- `Serilog.Sinks.File` with `rollingInterval: RollingInterval.Day` writes `log<yyyyMMdd>.txt` and the file is only created after `Emit`; write a test event and match `log*.txt`.
- `GoogleAuthProviderApi.GetUserInfo` throws `AbpException` when `UserInfoEndpoint` is empty, but `KeyNotFoundException` when the key is absent; supply an empty string for the expected `AbpException`.
- `SettingManagerExtensions.GetSettingValue<T>` is an ABP extension that calls `ISettingManager.GetSettingValue(string)`; mock the non-generic string return (e.g., `"587"`, `"true"`) instead of `GetSettingValue<T>`.
- `WebContentDirectoryFinder.CalculateContentRootFolder()` throws when `src/Eaf.Middleware.Web.Host` does not exist; test the exception branch.

## Coverage command
- `bash run-tests-with-coverage.sh` requires `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet` because the script does not export `DOTNET_ROOT`.
- `reportgenerator` (global tool) is required to consolidate the `coverage.cobertura.xml` files. If missing, install with `dotnet tool install -g dotnet-reportgenerator-globaltool`.

## Notable classes with remaining low coverage (target for P29)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.Controllers.FileController` (8.6%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.Configuration.SqlServerCacheConfigurer` (66.6%)
- `Eaf.Middleware.Web.Startup.AuthConfigurer` (67.3%)
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (53.4%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (45.3%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (45.2%)
- `Eaf.Middleware.Worker.EafHostBuilderExtensions` (40.7%)
- `Eaf.Middleware.Worker.EafWorkerBase` (71.6%)
- `Eaf.Middleware.Configuration.HostingEnvironmentExtensions` (50%) — MiddlewareCore
- `Eaf.Middleware.Identity.LogInManager`, `SecurityStampValidator`, `SignInManager` (0%)
- `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` (6.2%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (65.6%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (9.8%)
- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (11.1%)
- `Eaf.Middleware.Core.Authentication.External.Microsoft.MicrosoftAuthProviderApi` (79.1%)
- `Eaf.AspNetCore.SignalR.Chat.ChatHub` (45.8%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
- `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
- `Eaf.OpenTelemetry` (70.1%) — `EafOpenTelemetryServiceCollectionExtensions` (55.6%)
