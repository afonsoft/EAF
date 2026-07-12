# EAF Coverage Audit Memory

Last session branch: `feature/devin-20260712-priority40-coverage-audit`
Baseline coverage (P39): Line 90.8%, Branch 72%, Method 96.9%.
Current coverage (after P40): Line 93.1%, Branch 76.9%, Method 98.1%.

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
- `Abp.Configuration.SettingManager.GetSettingValueForTenantAsync` and `GetSettingValueForApplicationAsync` are `virtual` but `IsFinal` (sealed) in ABP 10.4.0; `NSubstitute` cannot override them. Use `ISettingManager` in production code and `Substitute.For<ISettingManager>()` in tests.
- `WebContentDirectoryFinder.CalculateContentRootFolder()` throws when `src/Eaf.Middleware.Web.Host` does not exist; test the exception branch.
- `Microsoft.Data.Sqlite` connection pooling can keep stale file handles on deleted/recreated invalid database files; set `SqliteConnectionStringBuilder.Pooling = false` for SQLite cache and call `SqliteConnection.ClearAllPools()` when deleting an invalid cache file.
- `AuthorizationExtensions.GetExternalTokenInformation` relies on `IocManager.Instance`; isolate it by swapping the static instance via reflection and restoring it after the test.
- `IocManager.Instance` has a non-public setter; use reflection to get/set the static property in tests that require isolation.
- `OpenIdConnectAuthProviderApi.ValidateTokenInternal` is private; invoke it via reflection and await the resulting `Task` (not `Task<ExternalAuthUserInfo>`).

## P40 gotchas
- `LdapAuthenticationSource.SourceName` is `"LDAP"` (uppercase); `AzureActiveDirectoryAuthenticationSource.SourceName` is `"ActiveDirectory"`.
- `Novell.Directory.Ldap.LdapConnection` `SearchAsync` throws `LdapException("CONNECTION_CLOSED", ...)` when not connected; use it to exercise the `TryAuthenticateAsync` exception branch.
- `AzureActiveDirectoryAuthenticationSource` uses `CreateGraphServiceClient`, `CreateAzureApplication` and `CreateAzureConfidential` as protected virtual methods; create a `TestableAzureActiveDirectoryAuthenticationSource` subclass overriding them and return NSubstitute mocks for `IPublicClientApplication`/`IConfidentialClientApplication`.
- `AcquireTokenByUsernamePassword` and `AcquireTokenForClient` return builder objects; to simulate failure, use `.Returns(callInfo => throw new MsalException(...))` instead of `Task.FromException<AuthenticationResult>`.
- `WebContentDirectoryFinder` is `public static class` in `Eaf.Middleware.Web` namespace; `CalculateContentRootFolder` searches for `src/Eaf.Middleware.Web.Host` and throws `DirectoryNotFoundException` if not found.
- `WebContentDirectoryFinder.DirectoryContains` is private static; exercise it via reflection with a directory that contains none of `Eaf.sln`, `Eaf.csproj`, `web.config`.
- `EafHostBuilderExtensions` (Web) has two overloads for `IHostBuilder` and `IWebHostBuilder`, both with default and prefix variants; `UseAbpConfiguration()` with no arguments builds successfully using `HostBuilder`/`WebHostBuilder`.
- `RedisConfigurer.Configure` uses `bool.Parse` on `RedisCache:IsRedisEnabled` or `RedisCache:IsEnabled`; use `ConfigurationBuilder` with `Dictionary<string, string?>` to cover enabled/disabled branches.
- `FriendshipManager` methods use `IRepository<Friendship>` and `IUnitOfWorkManager`; return `Task.FromResult<Friendship>(null!)` to avoid nullability warnings.
- `CultureHelper` has static fields `IsRtl` and `UsingLunarCalendar` based on `CultureInfo.CurrentUICulture`; tests must set `CurrentUICulture` (e.g., `ar-SA` for RTL, `zh-CN` for lunar calendar) and restore it.
- `SimpleStringCipher` is in `Abp.Runtime.Security` namespace.

## P39 gotchas
- `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap` maps `unique_name` to `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name` by default; remove `unique_name` from the map before OIDC token validation and restore it in `finally`.
- `OpenIdConnectAuthProviderApi.GetUserInfo` cannot mock `HttpDocumentRetriever` directly; replace the static `_defaultHttpClient` field's `HttpMessageHandler` via reflection and serve a fake JWKS with the same RSA key used to sign the JWT.
- `MiddlewareWebCoreModule.PostInitialize` uses concrete `AbpStartupConfiguration`; create it via `Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp")` and `Activator.CreateInstance` so `BackgroundJobs`, `Auditing` and `EntityHistory` are writable.
- `TokenAuthController` private methods (`CreateJwtClaims`, `TwoFactorAuthenticateAsync`, `IsTwoFactorAuthRequiredAsync`, `UpdateExternalUserAsync`, `RegisterExternalUserAsync`) are `[UnitOfWork]`; invoke via reflection and set `controller.UnitOfWorkManager` to a real `IUnitOfWorkManager` substitute.
- `RegisterExternalUserAsync` calls `_iocManager.ResolveAsDisposable<DefaultExternalLoginInfoManager>()`; use a real `IocManager` with `Component.For<DefaultExternalLoginInfoManager>().Instance(...)` to resolve the external login manager in tests.
- `CreateJwtClaims` uses `ClaimsIdentity.ReplaceClaim` (extension that returns `IEnumerable<Claim>`), but the returned sequence is not assigned, so the `amr`/`ExternalAuthProviderformation` claims are not present in the result.
- `ProviderKeysAreEqual`, `AddSingleSignInParametersToReturnUrl` and `ByteArrayCompare` are private (static) helpers; invoke via reflection with `BindingFlags.NonPublic | BindingFlags.Static` or `BindingFlags.Instance`.
- `TwoFactorAuthenticateAsync` requires `TokenAuthConfiguration` configured with `SecurityKey`, `SigningCredentials`, `Issuer` and `Audience` so `CreateAccessToken` can write a valid JWT.

## Coverage command
- `bash run-tests-with-coverage.sh` requires `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet` because the script does not export `DOTNET_ROOT`.
- `reportgenerator` (global tool) is required to consolidate the `coverage.cobertura.xml` files. If missing, install with `dotnet tool install -g dotnet-reportgenerator-globaltool`.

## Notable classes with remaining low coverage (target for P41)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (80.6%)
- `Eaf.Middleware.Authorization.Impersonation.ImpersonationManager` (81.0%)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (81.4%)
- `Eaf.AspNetCore.SignalR.Chat.SignalRChatCommunicator` (81.5%)
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` (82.1%)
- `Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettings` (82.3%)
- `Eaf.KeyVault.OCIKeyVaultManager` (84.3%)
- `Eaf.Middleware.DataExporting.Excel.EpPlus.EpPlusExcelExporterBase` (85.1%)
- `Eaf.Middleware.Web.Controllers.ProfileControllerBase` (86.7%)
- `Eaf.Middleware.Authorization.Users.UserAppService` (86.9%)
- `Eaf.Middleware.Friendships.Cache.UserFriendsCache` (87.2%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
- `Eaf.Middleware.Web.Controllers.ChatControllerBase` (88.2%)
- `Eaf.Middleware.Web.Swagger.SwaggerEnumParameterFilter` (88.2%)
- `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)
- `Eaf.Middleware.Configuration.AppConfigurations` (88.4%)
- `Eaf.Middleware.Web.Controllers.FileController` (89.1%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (89.2%)
- `Eaf.Middleware.MiddlewareAppServiceBase` (89.4%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (89.5%)
- `Eaf.Middleware.Chat.ChatFeatureChecker` (90.2%)
- `Eaf.Middleware.Localization.CultureHelper` (78.5%)

## P38 gotchas
- `TokenAuthController` tests were split into `TokenAuthControllerBddTests` (partial base) and `TokenAuthControllerP38BddTests` (additional BDD methods); classes must be `partial` to share helpers.
- `HangFireConfigurer.Configure` deferred `AddHangfire` lambda is executed when `GetService<JobStorage>()` is called; set `Serilog.Log.Logger` before `BuildServiceProvider` to avoid `InvalidOperationException`.
- `MiddlewareWebCoreModule.PreInitialize` requires a full `IAbpStartupConfiguration` substitute with `IocManager`, `Modules`, `Notifications.Providers`, `Features.Providers`, `Webhooks.Providers`, `Caching`, `Auditing`, `EntityHistory` and `BackgroundJobs` configured.
- `TenantManager.CreateWithAdminUserAsync` is non-virtual; it still runs for real when called via `TenantAppService.CreateTenant`, so configure `CreateAsync` via `NSubstitute.When(...).Do(...)` to assign `tenant.Id` and set `RoleManager.FeatureDependencyContext` to avoid `NullReferenceException` in `GrantAllPermissionsAsync`.
- `AbpRoleManager.GrantAllPermissionsAsync` is `public virtual` but sealed (`IsFinal`); assert `SetGrantedPermissionsAsync` and `CreateStaticRoles` on the substituted `RoleManager` instead.
- `Clock.Provider` can be a `NSubstitute` substitute when other tests run in parallel; creating `new Tenant(...)` inside a `.Returns(...)` call records `IClockProvider.get_Now` as the last NSubstitute call, causing `CouldNotSetReturnDueToTypeMismatchException`. Extract the tenant instance before calling `.Returns(...)`.
- `OpenIdConnectAuthProviderApi.ValidateToken` and `ValidateTokenInternal` are private; invoke them via `MethodInfo.Invoke` and unwrap `TargetInvocationException` for exception assertions.
- `WebContentDirectoryFinder.DirectoryContains` is private static; exercise it via reflection with a temporary directory containing `Eaf.sln`.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` coverage can vary depending on `IHostedService` execution order; ensure `StartAsync` is awaited on all registered `IHostedService` instances.

## P37 gotchas
- `AzureKeyVaultManager` is `internal` in `Eaf.KeyVault` and accessible via `InternalsVisibleTo("Eaf.KeyVault.Tests")`; its `SecretClient` field is `private readonly` and can be replaced via reflection in tests.
- `ServiceBusQueueAppender.AppendBuffer` creates a real `QueueClient`; pre-set `_serviceBusConnection` with `OperationTimeout = 1ms` to force a `ServiceBusTimeoutException` and exercise the `catch (ServiceBusException)` branch.
- `EafWebHookReceiver.LocalizationSource` is `protected` and throws `AbpException` when `LocalizationSourceName` is null; use a test subclass to access it.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` registers `IHostedService` instances for `TracerProvider`/`MeterProvider`; starting them via `IHostedService.StartAsync` builds the providers and covers the `AddOtlpExporter`/`AddConsoleExporter` lambda bodies.
- `HangFireConfigurer.Configure` uses `services.AddHangfire(...)`; the `config` lambda is deferred and executed when `GetService<JobStorage>()` is resolved, but `UseConsole` throws "Console is already initialized" if invoked multiple times in the same process.
- `TenantManager.CreateWithAdminUserAsync` is non-virtual; cannot be directly mocked with `NSubstitute.Returns` on `Substitute.For<TenantManager>()`.

## P36 gotchas
- `TokenAuthController.SendTwoFactorAuthCode` uses `UserIdentifier.ToUserIdentifier()` and `CacheManager.GetTwoFactorCodeCache()`; use `Abp.Runtime.Caching.Memory.AbpMemoryCacheManager` with a real `ICachingConfiguration` substitute to avoid `CacheManagerExtensions.GetCache<TKey,TValue>` NSubstitute limitations.
- `AbpController.LocalizationManager` has a `protected` getter; configure a local `ILocalizationManager` substitute and assign it to `controller.LocalizationManager` instead of chaining `controller.LocalizationManager.GetString(...)`.
- `MiddlewareWebCoreModule.PostInitialize` requires `IEntityHistoryConfiguration` and `IAuditingConfiguration` set on `AbpStartupConfiguration` to avoid null-reference when `Hangfire.IsEnabled` is true.
- `MiddlewareWebCoreModule.PostInitialize` with `Hangfire.IsEnabled` true registers `IBackgroundWorkerManager` and sets `JobStorage.Current` to `InMemoryStorage`; stub `IBackgroundWorkerManager` to avoid `RecurringJob` side effects.
- `TokenAuthController.LogOut` uses `AbpSession`, `IPrincipalAccessor`, and `ControllerBase.User` set via `ControllerContext.HttpContext`.
- `TokenAuthController.ImpersonatedAuthenticate` uses `_impersonationManager.GetImpersonationToken(...)` and `Encoding.UTF8.GetString(Convert.FromBase64String(...))` to decode the supplied token.
- `TokenAuthController.ExternalAuthenticate` returns a `UserFriendlyException` when `_externalAuthManager.Authenticate` returns null; configure `_externalAuthManager` to return a valid `ExternalAuthUserInfo`.
- `OCIKeyVaultManager.GetKeyValues` returns an empty dictionary when the OCI service returns null; `GetValue` throws the original exception when `GetSecret` fails.

## P35 gotchas
- `Abp.Authorization.Users.AbpLoginResult<TTenant, TUser>` has a constructor `(TTenant tenant, TUser user, ClaimsIdentity identity)` and exposes `Identity`, `User`, and `Tenant` properties.
- `TokenAuthController.Authenticate` uses `ISettingManager.GetSettingValue` (sync) for `UseCaptchaOnLogin` and `AllowOneConcurrentLoginPerUser`, and `GetSettingValueAsync` (async) for `TokenExpiration`; mock the non-generic string return accordingly.
- `UserManager.AddTokenValidityKeyAsync` returns `Task` (non-generic), not `Task<IdentityResult>`.
- `AbpController.UnitOfWorkManager` has a public setter; `AbpController.AbpSession` can be assigned a substitute `IAbpSession`.
- `MiddlewareWebCoreModule.Initialize` covers Redis branches when `appsettings.json` contains `RedisCache.IsRedisEnabled` or `RedisCache.IsEnabled` set to `true`.
- `SwaggerOperationFilter.Apply` requires a real `OperationFilterContext` with `ApiDescription` and `MethodInfo`.
- `MiddlewareJwtSecurityTokenHandler.ValidateToken` needs `user.Tokens` populated to validate the token validity key; removing `Tokens` forces the invalid-key branch.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` registers `ILoggerFactory` after `BuildServiceProvider`; `GetService<TracerProvider>()` returns null in this test harness.
- `OCIKeyVaultManager` constructor with explicit OCI authentication creates a `SecretsClient` successfully; `Base64Decode` is a private static method and can be exercised via reflection.
- `ServiceBusQueueAppender` remains low-coverage; use a dummy Service Bus connection string to exercise `SendAsync` failure paths safely.

## P34 gotchas
- `Abp.Configuration.SettingManager.GetSettingValueForTenantAsync`/`GetSettingValueForApplicationAsync` are `virtual sealed` (IsFinal) and cannot be mocked; prefer `ISettingManager` in constructors and tests.
- `Microsoft.Data.Sqlite` connection pooling can keep stale handles to deleted cache files; disable pooling (`SqliteConnectionStringBuilder.Pooling = false`) and call `SqliteConnection.ClearAllPools()` when recreating a corrupt cache.
- `AuthorizationExtensions.GetExternalTokenInformation` uses `IocManager.Instance`; replace the static instance with a fresh `IocManager` via reflection and restore it to avoid cross-test state leaks.
- `OpenIdConnectAuthProviderApi.ValidateTokenInternal` is private and returns `Task`; invoke via reflection and await the returned `Task`.
- `EafSqliteCache` `Dispose` is idempotent; calling `Dispose()` twice should not throw.
- `UiCustomizationSettingsAppService` now accepts `ISettingManager` so it can be mocked with `NSubstitute`.
- `LdapSettings.GetContextType` returns `null` on non-Windows platforms regardless of `tenantId`.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` mutates `OTEL_*` environment variables; isolate tests that set `OtlpVariables`.
- `OCIKeyVaultManager.Base64Decode` is private static; exercise it via reflection in uninitialized instances.
- `DbCommandPool` is `internal` in `Eaf.SqliteCache`; create an initial `SQLiteConnection` with `EafSqliteCache.TableInitCommand` and `Cache=Shared` to exercise `Use`/`UseAsync`.
- `EafCastleWindsorHostBuilderExtensions.UseCastleWindsor` requires `HostBuilder.Build()` to actually run the `ConfigureServices` lambda and register the `IWindsorContainer` singleton.

## P33 gotchas
- `AbpModule.IocManager` and `Configuration` are protected properties; set them via reflection when unit-testing module `Initialize`/`PreInitialize` methods.
- `MiddlewareWebCoreModule` constructor sets environment variables (`ASPNETCORE_ENVIRONMENT`, `EAF_ENVIRONMENT`, `ASPNET_ENV`) from `IHostEnvironment.EnvironmentName`; tests should use a temporary directory and restore original values.
- `TokenAuthController.GetDefaultEnabledProvider` uses `SettingManagerExtensions.GetSettingValueForApplication<bool>` which is an extension method; mock the non-generic `ISettingManager.GetSettingValueForApplication(string)` with `"true"`/`"false"` strings.
- `TokenAuthController.GetExternalAuthenticationProviders` requires `ObjectMapper` to be set; otherwise `NullObjectMapper` throws `AbpException`.
- `OpenIdConnectAuthProviderApi.GetUserInfo` with invalid tokens reaches `ConfigurationManager.GetConfigurationAsync` and throws `Exception`; tests should assert `Exception` rather than specific error types.
- `EafSqliteCache` `Dispose` is idempotent; calling `Dispose()` twice should not throw.
- `UiCustomizationSettingsAppService.UseSystemDefaultSettings` uses `SettingManager.GetSettingValueForTenantAsync`/`GetSettingValueForApplicationAsync` for `AppSettings.UiManagement.Theme` and `IUiCustomizer.GetTenantUiCustomizationSettings`/`GetHostUiManagementSettings`.
- `LdapSettings.GetContextType` returns `null` on non-Windows platforms regardless of `tenantId`.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` ignores `OtlpVariables` entries with empty keys or null/empty values; tests can safely pass empty entries.
- `OCIKeyVaultManager.Base64Decode` is private static; exercise it via reflection in uninitialized instances.

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

## Sonar fix (2026-07-12)
Branch: `feature/devin-20260712-fix-sonar-jwt`
- `src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/MiddlewareJwtSecurityTokenHandler.cs`:
  - S2583 (line 117): `tokenValidityValueInClaims` is never null, so remove the unreachable `?? user.SecurityStamp` fallback from the cache `Set` call.
  - S6667/S2139 (line 131-138): `catch` clauses in `ValidateToken` were logging the exception and rethrowing it, which violates both S6667 (pass exception to log) and S2139 (log or rethrow, not both). Removed the `DebugFormat` calls so the `catch` blocks simply rethrow.
- Tests: `Eaf.Middleware.Web.Core.Tests` passed (662 tests).
