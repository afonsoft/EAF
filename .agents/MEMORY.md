# EAF Coverage Audit Memory

Last session branch: `feature/devin-20260711-priority32-coverage-audit`
Baseline coverage (P31): Line 83.4%, Branch 62.6%, Method 94.3%.
Current coverage (after P32): Line 83.6%, Branch 62.8%, Method 94.4%.

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

## Notable classes with remaining low coverage (target for P33)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
- `Eaf.Middleware.Identity.LogInManager`, `SecurityStampValidator`, `SignInManager` (0%)
- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (17.4%)
- `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%)
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (53.4%)
- `Eaf.Middleware.Web.Startup.AuthConfigurer` (69.3%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (85%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
- `Eaf.OpenTelemetry` (78.5%) — `EafOpenTelemetryServiceCollectionExtensions` (68.1%)
- `Eaf.Middleware.Application.Authorization.Users.Profile.ProfileAppService` (81.3%)
- `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)

## P32 gotchas
- `ICache.GetOrDefault("userId@tenantId")` is the key shape used by `AuthorizationExtensions.GetExternalTokenInformation`; `NSubstitute.Arg.Any<string>()` does not always match the `IAbpCache<string, object>` method, so use the explicit key.
- `SettingManager.GetSettingValueForTenantAsync`/`GetSettingValueForApplicationAsync` are virtual and return `Task<string>`; configure with `Task.FromResult(value)` and do not attempt to `Returns` on `IAbpSession.ToUserIdentifier()` (extension method, not a virtual member).
- `TenantManager.CreateWithAdminUserAsync` is non-virtual; cannot be directly mocked with `NSubstitute.Returns` on `Substitute.For<TenantManager>`.
- `EafSqliteCache` will reuse an existing database file, and `DbCommandPool.CheckExistingDb` falls back to `DELETE`/`CREATE` when the schema is invalid; tests should clean up temp files.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` mutates `OTEL_*` environment variables; tests that set `OtlpVariables` should use a dedicated key and reset it.

## P30 gotchas
- `Microsoft.Extensions.Caching.StackExchangeRedis` (10.0.8) registers `IDistributedCache` with implementation type `RedisCacheImpl`; assertions must check `ImplementationType.Name.Contains("RedisCache")`.
- `EafHangfireAuthorizationFilter.Authorize` accepts tokens from query string (`auth`, `access_token`), cookie `Eaf.AuthToken`, header `Eaf.AuthToken`, and from the `EafCache` by remote IP.
- `ExpiredAuditLogDeleterWorker` uses a private `MaxDeletionCount` of 30,000; reflection can lower it to avoid large test data sets.
- `AuthConfigurer.Configure` uses `IocManager.Instance` to resolve `TokenAuthConfiguration`; tests reuse the static singleton already initialized by other tests.
- `ServiceBusQueueAppender` creates a real `QueueClient` when `ConnectionString` and `QueueName` are valid; a dummy `Endpoint=sb://localhost:1;SharedAccessKeyName=x;SharedAccessKey=y` string safely fails during `SendAsync` and exercises the `catch` branch.
- `OpenIdConnectAuthProviderApi` validates `Token` and `Authority` before attempting `ConfigurationManager.GetConfigurationAsync`; pass null/empty values to trigger early exceptions.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` mutates `OTEL_*` environment variables; tests that set `OtlpEndpoint` should not reassign `OtlpProtocol` to unsupported values without restoring state.
- `ChatHub.DeleteMessage` and `SendMessage` group paths throw `AbpException`/`UserFriendlyException` and a generic `Exception` branch; they require a `DefaultHttpContext` with `RequestServices` configured.
- `MicrosoftAuthProviderApi.GetUserInfo` falls back to `Provider` "Microsoft" and `Picture` null when the photo endpoint throws.
- `EafHangfireAuthorizationFilter.Authorize` requires a real `AspNetCoreDashboardContext` built with `JobStorage`, `DashboardOptions` and `HttpContext`; `GetHttpContext()` returns `HttpContext` only for that concrete type.
- `AspNetCoreDashboardContext` needs `RequestServices` with `IAbpSession`, `IPermissionChecker` (the interface method `IsGranted(UserIdentifier, string)` is used by `PermissionCheckerExtensions`, not the `params` extension) and `ICacheManager`.
- `ExpiredEntityLogDeleterWorker` is structurally similar to `ExpiredAuditLogDeleterWorker`, but uses `IEntityHistoryConfiguration`, `ISettingManager` and `IRepository<EntityChange, long>`.
- `OpenIdConnectAuthProviderApi` and `MicrosoftAuthProviderApi` tests need a `TestHttpMessageHandler` that supports multiple staged responses (`(uri, status, content)`).
- `AddEafOpenTelemetry` mutates `OTEL_*` environment variables; avoid changing `OtlpProtocol` to non-default values without isolating/restoring environment variables.
- `MapEafOpenTelemetryMetrics` depends on `MapPrometheusScrapingEndpoint` which requires a real `MeterProvider` in `IEndpointRouteBuilder.ServiceProvider`; leave it for an integration test harness.
- `FileController` `FormFile.ContentType` setter requires `formFile.Headers` to be initialized first (`new HeaderDictionary()`).
- `BinaryObject` constructor formats `FileName` as `{Id}_{fileName}`, so `FileDownloadName` assertions must use the constructed name.
- `ChatHub.Dispose` uses `WindsorContainer` injection; capture the container substitute in the test setup to assert `Release`.
