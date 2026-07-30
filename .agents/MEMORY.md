# EAF Coverage Audit Memory

## P71: full Docker stack multi-tenant / chat test (2026-07-30)
- Stack: `docker-compose.all.yml` with `eaf-sqlserver`, `eaf-migrator`, `eaf-api`, `eaf-worker`, `eaf-angular` all healthy on `localhost:5000` and `localhost:4200`.
- Default ABP seed admin password forces a reset on first login; use `/api/services/app/Account/ResetPassword` to set a real password before any other calls.
- Created tenants `tenantA` (id 2) and `tenantB` (id 3) with `CreateTenant`; each tenant gets its own `admin` user and isolated user store.
- Enabled chat features for both tenants via `PUT /api/services/app/Tenant/UpdateTenantFeatures`:
  - `App.ChatFeature`
  - `App.ChatFeature.TenantToTenant`
  - `App.ChatFeature.GroupChat`
- Test users:
  - `shareduser` in both tenantA and tenantB (same credentials, different `sub`/user ids).
  - `alice` only in tenantA.
  - `bob` only in tenantB.
- Verified tenant data isolation: tenantA user list contains `admin`, `alice`, `shareduser` and does **not** contain `bob`.
- Verified cross-tenant SignalR chat:
  - Created friendship from tenantA admin to tenantB admin with `POST /api/services/app/Friendship/CreateFriendshipRequestByUserName`.
  - Sent message via `/signalr-chat` `SendMessage` from tenantA admin.
  - Confirmed tenantB admin can retrieve the message via `GET /api/services/app/Chat/GetUserChatMessages?UserId=<sender>&TenantId=<senderTenant>`.
- Full automation: `.agents/skills/testing-eaf-docker/scripts/eaf-fullstack-test.py` (23/23 checks passing).
- Manual routine documented in `.agents/skills/testing-eaf-docker/SKILL.md`.
- Use this script for any new feature affecting multi-tenancy, auth, users, SignalR or CORS.

Last session branch: `feature/devin-20260718-priority69-compose-hardening`
Baseline coverage (P40): Line 93.1%, Branch 76.9%, Method 98.1%.
Current coverage (after P42): Line 95.5%, Branch 80.9%, Method 98.6%.
Current coverage (after P43): Line 96.1%, Branch 82.0%, Method 99.1%.
Current coverage (after P44): Line 96.1%, Branch 82.0%, Method 99.1%.
Current coverage (after P45): Line 96.2%, Branch 82.3%, Method 99.1%.
Current coverage (after P46): Line 96.2%, Branch 82.6%, Method 99.1%.
Current coverage (after P47): Line 96.2%, Branch 82.8%, Method 99.1% (4388 tests, 4387 passing, 1 skipped). Build warnings: 141.
Current coverage (after P48): Line 96.3%, Branch 83.0%, Method 99.1% (4388 tests, 4387 passing, 1 skipped). Build warnings: 141.
Current coverage (after P49): Line 96.3%, Branch 82.9%, Method 99.2% (4393 tests, 4392 passing, 1 skipped). Build warnings: 140.
Current coverage (after P50): Line 96.4%, Branch 83.0%, Method 99.4% (4397 tests, 4396 passing, 1 skipped). Build warnings: 142.
Current coverage (after P51): Line 96.4%, Branch 83.0%, Method 99.3% (4401 tests, 4400 passing, 1 skipped). Build warnings: 159.
Current coverage (after P52): Line 96.6%, Branch 83.6%, Method 99.3% (4416 tests, 4415 passing, 1 skipped). Build warnings: 159.
Current coverage (after P53): Line 97.1%, Branch 84.2%, Method 99.4% (4433 tests, 4432 passing, 1 skipped). Build warnings: 163.
Current coverage (after P54): Line 97.2%, Branch 85.1%, Method 99.5% (4467 tests, 4466 passing, 1 skipped). Build warnings: 127.
Current coverage (after P55): Line 97.5%, Branch 85.6%, Method 99.6% (4492 tests, 4491 passing, 1 skipped). Build warnings: 129.
Current coverage (after P56): Line 97.6%, Branch 87.2%, Method 99.6% (4516 tests, 4515 passing, 1 skipped). Build warnings: 154.
Current coverage (after P57): Line 97.7%, Branch 87.5%, Method 99.6% (4533 tests, 4532 passing, 1 skipped). Build warnings: 154.
Current coverage (after P58): Line 97.7%, Branch 89.1%, Method 99.7% (4555 tests, 4554 passing, 1 skipped). Build warnings: 159.
Current coverage (after P59): Line 97.7%, Branch 90.0%, Method 99.8% (4585 tests, 4584 passing, 1 skipped). Build warnings: 161.
Current coverage (after P60): Line 97.8%, Branch 90.2%, Method 99.8% (4593 tests, 4592 passing, 1 skipped). Build warnings: 161.
Current coverage (after P61): Line 97.8%, Branch 90.3%, Method 99.8% (4597 tests, 4596 passing, 1 skipped). Build warnings: 162.
Current coverage (after P62): Line 97.9%, Branch 90.4%, Method 99.8% (4602 tests, 4601 passing, 1 skipped). Build warnings: 162.
Current coverage (after P63): Line 97.9%, Branch 90.5%, Method 99.8% (4604 tests, 4603 passing, 1 skipped). Build warnings: 162.
Current coverage (after P64): Line 97.9%, Branch 90.5%, Method 99.8% (4604 tests, 4603 passing, 1 skipped). Build warnings: 0 (Eaf.sln).
Current coverage (after P65): Line 97.9%, Branch 90.5%, Method 99.8% (4604 tests, 4603 passing, 1 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular).
Current coverage (after P66): Line 97.9%, Branch 90.5%, Method 99.8% (4605 tests, 4604 passing, 1 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular); `Eaf.ApiWithSrc.sln` Swagger validated on `http://localhost:5000/swagger`.
Current coverage (after P67): Line 97.9%, Branch 90.5%, Method 99.8% (4605 tests, 4604 passing, 1 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular); Worker template starts locally against SQL Server Docker.
Current coverage (after P68): Line 97.9%, Branch 90.5%, Method 99.8% (4605 tests, 4604 passing, 0 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular); Docker Compose end-to-end validated (SQL Server, Migrator, API, Worker, Angular); `http://localhost:5000/swagger` and `http://localhost:4200` respond.
Current coverage (after P69): Line 97.9%, Branch 90.5%, Method 99.8% (4605 tests, 4604 passing, 0 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular); `docker-compose.yml` split into `docker-compose.all.yml` (full stack) and `docker-compose.yml` (API + Angular only, driven by environment variables); healthchecks, named volumes, `.env.example`, and `scripts/validate-docker-compose.sh` added.
Current coverage (after P70): Line 97.9%, Branch 90.5%, Method 99.8% (4605 tests, 4604 passing, 0 skipped). Build warnings: 0 (Eaf.sln); template build warnings: 0 (Api, Worker, Angular); added `.github/workflows/docker-compose-validation.yml` to run `docker-compose.all.yml` end-to-end on PRs touching Docker/Compose files; `scripts/validate-docker-compose.sh` now saves container logs to `LOGS_DIR` for CI artifact upload; `docs/development/session-summaries` removed; future prompts moved to `.specs`.

## P70 gotchas
- Added `.github/workflows/docker-compose-validation.yml` triggered on PRs and `workflow_dispatch` when `docker-compose*.yml`, `Dockerfile*` or `scripts/validate-docker-compose.sh` change.
- Workflow builds `Eaf.sln` in Release, sets up Docker Buildx, caches NuGet packages and Docker layers, then runs `scripts/validate-docker-compose.sh` with `COMPOSE_FILE=docker-compose.all.yml`.
- `scripts/validate-docker-compose.sh` now persists container logs to `LOGS_DIR` (when set) before tearing down the stack, so the workflow can upload logs as artifacts on failure.
- Removed `docs/development/session-summaries`; future session prompts and summaries go under `.specs/`.

## P69 gotchas
- Split `docker-compose.yml` into two files:
  - `docker-compose.all.yml` — full stack with SQL Server, Migrator, API, Worker and Angular, plus healthchecks and named volumes (`mssql-data`, `eaf-api-logs`, `eaf-worker-logs`).
  - `docker-compose.yml` — minimal API + Angular stack driven entirely by environment variables; no infrastructure containers, meant for scenarios where SQL Server/Redis are managed externally.
- Added `curl` to the API Dockerfile (`ca-certificates curl`) so the API healthcheck (`/health`) works inside the container.
- Added `procps` to the Worker Dockerfile so the Worker healthcheck (`pgrep -x dotnet`) works inside the container.
- The validation script `scripts/validate-docker-compose.sh` starts the stack, waits for endpoint responses, verifies the migrator exited cleanly and checks Worker logs for `FATAL`/`Unhandled`/`Critical`.
- When running `docker-compose.yml` with the infrastructure from `docker-compose.all.yml`, start only `eaf-sqlserver` and `eaf-migrator` from the full compose (`docker compose -f docker-compose.all.yml up -d eaf-sqlserver eaf-migrator`) and then start the minimal compose; both share the same `eaf-network` bridge.
- The API healthcheck uses `curl -f http://localhost:8001/health`; the Angular healthcheck uses `curl -f http://localhost/` (nginx image includes curl).

## P68 gotchas
- Docker Compose end-to-end stack: `eaf-sqlserver`, `eaf-migrator`, `eaf-api`, `eaf-worker` and `eaf-angular` (nginx) all start and communicate over the `eaf-network` bridge network.
- SQL Server 2022 healthcheck in Docker must use `/opt/mssql-tools18/bin/sqlcmd` and the `-C` (trust certificate) flag; the older `/opt/mssql-tools/bin/sqlcmd` path no longer exists in the `2022-latest` image.
- The Worker `Templates/Worker/Dockerfile` was outdated (context `Templates/Worker`, wrong project path, .NET 8 images); it was rewritten to build from the repository root and target `Templates/Worker/src/Eaf.ProjectName.WorkerService` using .NET 10 images.
- A dedicated `Templates/Api/Dockerfile.migrator` runs `Eaf.ProjectName.Migrator.dll -s` with `ASPNETCORE_Docker_Enabled=true`, executing migrations before API/Worker start via `depends_on`/`service_completed_successfully`.
- The API only exposes Swagger in non-Production environments (`Startup.cs` line 244); for the Docker Compose validation the API service uses `ASPNETCORE_ENVIRONMENT=Staging` so `http://localhost:5000/swagger` responds.
- OpenTelemetry in the API/Worker containers tries to export to `https://otlp.nr-data.net` and logs 404/405; this is non-fatal and does not prevent startup or serving requests.

## P67 gotchas
- Worker template `ProjectNameCoreModule` was missing `MiddlewareCoreModule` in `DependsOn`, so `AbpZeroEntityTypes` (`Tenant`/`User`/`Role`) was not set and startup threw `ArgumentNullException`. Adding `typeof(MiddlewareCoreModule)` fixes the module initialization chain.
- `Eaf.Middleware.Worker/MiddlewareWorkerModule.cs`, `Eaf.Middleware.Application/MiddlewareApplicationModule.cs` and `Eaf.Middleware.Web.Core/MiddlewareWebCoreModule.cs` had XML doc comments placed after `[DependsOn]` attributes, causing `CS1587`; moving the `/// <summary>` block above the attribute resolves the warnings.
- `LdapAuthenticationSource.GetUsersFromActiveDirectoryAsync` uses `System.DirectoryServices.AccountManagement` APIs that are Windows-only. Annotating the method with `[SupportedOSPlatform("windows")]` removes `CA1416` warnings without suppressing the analyzer globally.
- Running the Worker locally requires the same Docker SQL Server setup as the API and environment variables: `DOTNET_ENVIRONMENT=Production`, `ConnectionStrings__Default` (SQL Server with `Encrypt=false`), `Database__Provider=SqlServer`, `Hangfire__IsEnabled=false`, `SqlServerCache__IsEnabled=false`.
- `Eaf.SqliteCache.Tests` test `Set_WithAbsoluteExpiration_ShouldExpireCorrectly` is timing-sensitive and can fail when the full test suite runs in parallel; it passes in isolation and coverage is unaffected.

## P66 gotchas
- `AppConfigurations.BuildConfiguration` and `EafHostBuilderExtensions.UseEafConfiguration` added environment variables **before** JSON files, preventing env-based overrides of `ConnectionStrings` and other settings. Moving `AddEnvironmentVariables` after `appsettings.json`/`appsettings.{Environment}.json` fixes this and matches standard .NET configuration precedence.
- `MiddlewareCoreModule.cs` had an XML doc comment after `DependsOn` attributes, causing `CS1587`; moved the comment above the attributes.
- Template Worker `AppConfigurations.cs` copy was updated with the same precedence fix.
- Added BDD test `Dado_AppsettingsEVariavelDeAmbienteComMesmoNome_Quando_Get_Entao_VariavelDeAmbienteSobrescreveJson` to lock in env override behavior.
- To run `Eaf.ApiWithSrc.sln` locally: start a SQL Server 2022 Docker container, run `Eaf.ProjectName.Migrator` with `ConnectionStrings__LOCAL`, then run `Eaf.ProjectName.Web.Host` with `ConnectionStrings__Default` (or `ProjectName_` prefix via host builder), `Hangfire__IsEnabled=false`, and `SqlServerCache__IsEnabled=false`.
- `GET /api/services/app/About/GetAbout` returns 200 with environment info.
- The `Templates/Api/Dockerfile` must be built from the repository root because the API template references EAF source projects in `src/` via `ProjectReference`.
- The Angular Dockerfile uses `envsubst` to generate `assets/env.js` from `src/assets/env.template.js`; `AppPreBootstrap.getApplicationConfig` reads `window['env']['remoteServiceBaseUrl']` and `window['env']['appBaseUrl']` at runtime to override `appconfig.{env}.json`.

## Docker template notes (P66 follow-up)
- API Dockerfile: build context is repo root; remove `appsettings.{Local|Staging|Production|Development}.json` from image; pass `ConnectionStrings__Default`, `Database__Provider`, `Hangfire__IsEnabled`, `SqlServerCache__IsEnabled`, `RedisCache__IsEnabled`, `App__ServerRootAddress`, `App__ClientRootAddress` and `App__CorsOrigins` as environment variables.
- Angular Dockerfile: Node 20 alpine, `npm install --legacy-peer-deps`; pass `REMOTE_SERVICE_BASE_URL`, `APP_BASE_URL` and `ASPNETCORE_ENVIRONMENT` at runtime.

## P65 gotchas
- Template `Api` and `Worker` build warnings reduced to 0 by adding documented suppressions (`NU1608` for Pomelo and `NuGetAuditSuppress` for `GHSA-rvv3-g6hj-g44x`) to `Templates/Api/common.props` and `Templates/Worker/common.props`.
- `Templates/Api/test/Directory.Build.props` imports `..\\common.props` so test projects in the API template share the same warning-suppression configuration.
- No safe dependency upgrades are available yet: `Pomelo.EntityFrameworkCore.MySql` has no stable EF Core 10 release, and `AutoMapper >= 15` is commercial/copyleft and binary-incompatible with `Abp.AutoMapper 10.4.0`.
- SonarCloud public API returned 0 open `Bug`/`Vulnerability` issues for `afonsoft_EAF2`; quality gate on PR #199 passed with 0 new issues.
- `UserAppServiceBddTests.Dado_UserNamesLdapValidosComTenant_Quando_CreateUsersByLdap_Entao_DeveCriarUsuariosComTenant` showed NSubstitute-related flakiness under parallel execution, but passed on retry without code changes.

## P64 gotchas
- `Eaf.sln` build warnings reduced from 141 to 0 by switching test projects to `<Nullable>annotations</Nullable>`, removing unnecessary `Microsoft.Extensions.*` package references from `Eaf.Middleware.Worker.Tests`, removing unnecessary `new` modifiers in `EafWebhookReceiverBddTests`, replacing obsolete `FormatterServices.GetUninitializedObject` with `RuntimeHelpers.GetUninitializedObject` in `LdapAuthenticationSourceBddTests`, and removing obsolete `ServicePointManager.Expect100Continue` from the Worker template.
- Test projects use nullable annotations-only context to avoid noise from intentional null values in test setup while keeping `?`/`!` syntax available.
- SonarCloud quality gate on PR #198 passed with 0 new issues; no Bug/Vulnerability issues to treat.
- Templates `Api`, `Worker` and `Angular/Eaf.ProjectName.UI` build successfully. Worker template warnings reduced; API template still emits 26 warnings (Pomelo `NU1608` + AutoMapper `NU1903`) and Worker template 6 warnings (AutoMapper `NU1903`). These require dependency version updates that are not yet safe (Pomelo EF Core 10 support not on stable NuGet; AutoMapper >= 15 binary-incompatible with `Abp.AutoMapper 10.4.0`).

## P63 gotchas
- SonarCloud duplication on PR #197 was caused by `Templates/**` boilerplate being included in CPD; adding `sonar.cpd.exclusions=Templates/**` to `.sonarcloud.properties` and `/d:sonar.cpd.exclusions="Templates/**"` to `sonarcloud.sh` resolves it for future PRs.
- `EafHangfireAuthorizationFilter` branches for missing `IPermissionChecker` (returns `true`) and cache-token miss with a remote IP are reachable with a fake `IServiceProvider` that implements `ISupportRequiredService`.
- `PermissionAppService.AddPermission` defensive `if (permission.Children == null)` remains unreachable because ABP `Permission.Children` getter throws `ArgumentNullException` when the backing list is null.
- `LdapSettings.GetContextType` and `AzureActiveDirectoryAuthenticationSource`/`LdapAuthenticationSource` AD/LDAP branches are Windows/infrastructure-limited and cannot be covered on Linux.
- `MiddlewareAppServiceBase.GetCurrentTenant` (`TenantManager.GetById`) always throws `NotImplementedException`, so the closing return is unreachable.
- `EafSqliteCache` outer `catch (SqliteException)` in `Connect` and `ServiceBusQueueAppender.OnClose` catch are practically unreachable in unit tests because `Microsoft.Data.Sqlite` only throws on command execution (caught inside `CheckExistingDb`) and `ServiceBusConnection.CloseAsync` is non-virtual/no-throw without a real service bus.
- `EafHangfireApplicationBuilderExtensions.UseEafHangfire` line `DisplayNameFunc = (context, job) => EafDisplayNameExtensions.Format(context, job)` is only executed when the Hangfire dashboard renders at runtime; unit tests configure the dashboard but do not render it.
- `TokenAuthController` and `MiddlewareWebCoreModule` still contain branches tied to real external services (Google/Facebook/Microsoft/WS-Federation/Redis/SignalR/Hangfire runtime) and are documented as inalcançáveis.

## P62 gotchas
- `SerilogLogger` has an internal parameterless constructor used by `Castle Windsor` reflection; invoke it via reflection in tests to reach 100% method coverage.
- `ChatMessageManager.HandleSenderToReceiverAsync` returns early when an existing friendship is `FriendshipState.Blocked`; this branch is reachable by invoking the private method directly with a blocked `Friendship`.
- `EafSqlServerCache.CompressBytesAsync` catch swallows any compression error and returns the original bytes; passing `null` bytes triggers an `ArgumentNullException` and covers the catch.
- `EafHangfireAuthorizationFilter.IsPermissionGranted` has defensive early returns for `userIdentifier == null` and empty `requiredPermissionName`; both branches are reachable via reflection and default to the standard Hangfire dashboard permissions.
- `Templates/Worker` had legacy `Eaf.*` namespaces and missing `Microsoft.Extensions.*` package versions; aligning it with `Abp.*` types and the API template package versions made the Worker template build with 0 errors.
- `Templates/Api`, `Templates/Worker` and `Templates/Angular/Eaf.ProjectName.UI` all build successfully (Release for .NET, production for Angular). Worker produced 6 warnings (AutoMapper NU1903 + obsolete `ServicePointManager` SYSLIB0014), API 26 warnings (Pomelo/AutoMapper), Angular build completed with no errors.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafSqliteCache`, `ServiceBusQueueAppender`, `TokenAuthController`, `DefaultExternalLoginInfoManager`, `OpenIdConnectAuthProviderApi`, `EafHangfireApplicationBuilderExtensions`, `MiddlewareWebCoreModule`, `LdapSettings`, `UserEmailer` and `AzureActiveDirectoryAuthenticationSource` still contain branches that are Linux/infrastructure-limited or require real external services; document as inalcançáveis for P63.

## P60 gotchas
- `System.Linq.Enumerable.OrderBy` on a single-element list does not invoke the key selector, so `ObjectMapper.Map<List<T>>` stubs returning one item do not cover `OrderBy` lambda sequence points; return at least two elements.
- `Permission.Children` uses `ImmutableList` and its getter throws `ArgumentNullException` when the backing field is null, making the `permission.Children == null` early-return branch in `PermissionAppService.AddPermission` effectively unreachable on this ABP version.
- `EafHangfireAuthorizationFilter.Authorize` returns true when `permissions` is null or when a JWT `sub` claim exists without `tenantId`.
- `HostSettingsAppService.GetAllSettings` catches `Exception` while reading `ExternalLoginProviderSettings` and returns a default instance when the underlying setting value is invalid/missing.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` branches on `ConsoleExporter`, `OtlpEndpoint` and `MeterName`; use a real `IServiceCollection`/`ILoggingBuilder` and assert the returned `IOpenTelemetryBuilder`/logger factory.
- `ChatMessageManager.SendMessageAsync` has branches for existing friendships, updated friend-cache info and missing reverse friendship; use non-null `FriendshipState.Accepted` for both directions and cache entries that already match sender info.
- `RoleAppService.GetRolesForEdit`, `TenantAppService.GetTenantFeaturesForEdit` and `UserAppService.GetUserPermissionsForEdit` all sort returned `FlatPermissionDto`/`FlatFeatureDto` lists by `DisplayName`; two items are required to exercise the `OrderBy` key selector.

## P61 gotchas
- `Templates/Api/src/Eaf.ProjectName.Core/Eaf.ProjectName.Core.csproj` was missing `Microsoft.EntityFrameworkCore`, causing `CS0234` in `AirplaneManager.cs` during `Production Build`; fixed by adding `PackageReference` version `10.0.8`.
- `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs` needed `using Microsoft.EntityFrameworkCore.Infrastructure;` and `MigrateDatabase` must be instance (not static) because `AbpDbContext.Logger` is non-static.
- `Environment.SetEnvironmentVariable` tests in xUnit are flaky when run in parallel with other test classes; avoid relying on global env vars for branch coverage.
- `UserAppService.GetGrantedPermissionsAsync` returns the granted permissions list directly when the user is not null; stub `GetGrantedPermissionsAsync` with a non-empty list to cover the non-empty branch.
- `TokenAuthController.Authenticate` calls `InitializeOptionsAsync` with the resolved tenant id (nullable); a login without explicit tenant passes `(int?)null`.
- `ChatHub` disposes its Windsor container only once due to `_isCallByRelease`; calling `Dispose` twice does not release twice.
- `EafHangfireAuthorizationFilter.Authorize` returns `false` when `permissionChecker.IsGranted` returns false after a valid JWT is supplied.
- `EafSqliteCache.Set` supports combined `slidingExpireTime` and absolute `expiry`; passing both values stores and retrieves the value correctly.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafSqlServerCache`, `EafSqliteCache`, `ServiceBusQueueAppender`, `TokenAuthController`, `ChatHub`, `DefaultExternalLoginInfoManager`, `OpenIdConnectAuthProviderApi`, `EafHangfireApplicationBuilderExtensions`, `EafHangfireAuthorizationFilter`, `MiddlewareWebCoreModule`, `LdapSettings`, `UserEmailer` and `AzureActiveDirectoryAuthenticationSource` still contain branches that are Linux/infrastructure-limited or require real external services; document as inalcançáveis for P62.

## P59 gotchas
- `IRepository<UserToken, long>` configured with `NSubstitute` `Returns` expects `Task<UserToken>`; cast `EafUserToken` to `UserToken` before wrapping with `Task.FromResult`.
- The `BinaryObject` constructor prefixes `FileName` with `{Id}_`; assertions on `FileContentResult.FileDownloadName` must use `binaryObject.FileName` when the controller's `fileName` parameter is `null`.
- `EafOpenTelemetryOptions` constructor reads `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_PROTOCOL` and `OTEL_SERVICE_NAME` environment variables into `OtlpVariables`.
- `ImpersonationManager.GetImpersonatedUserAndIdentity` reconstructs the cache item from `UserToken` repository on cache miss; test `Value` containing `"{impersonatorTenantId}-{impersonatorUserId}"` and `Value` null.
- `WebLogAppService.GetLatestWebLogs` matches level prefixes `IMF`, `DBG`, `WRN`, `ERR`, `FAT`, `FTL`, uppercase names and no-prefix lines.
- `AuditLogListExcelExporter.ExportToFile` uses `_.Exception.IsNullOrEmpty() ? L("Success") : _.Exception`; exercise the `false` branch with a non-empty `Exception`.
- `LanguageAppService.GetLanguages` returns `DefaultLanguageName = null` when no default language is configured.
- `DefaultExternalLoginInfoManager.GetNameAndSurname` falls back to `nameClaim.Value` when `givenName`/`surname` are empty and trims trailing spaces.
- `EafWebhookReceiver` caches `LocalizationSource` per culture; changing `SourceName` invalidates the cached source.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafHangfireApplicationBuilderExtensions`, `EafHangfireAuthorizationFilter`, `ChatHub`, `TokenAuthController`, `EafSqlServerCache`, `EafSqliteCache` and `ServiceBusQueueAppender` still contain Linux/infrastructure-limited branches; document as inalcançáveis for P60.

## P58 gotchas
- `SerilogLogger` disabled branches are best covered with a real Serilog logger configured with `LevelAlias.Off`; `NSubstitute.For<Serilog.ILogger>()` fails at runtime because `ILogger` contains default interface methods that Castle DynamicProxy cannot route.
- `ChatMessageManager.Delete(sharedMessageId)` always calls `_chatMessageRepository.Delete(...)` (with an empty `ids` array when no messages match), so assert `Received(1)` rather than `DidNotReceive`.
- `HostSettingsAppService.UpdateAllSettings` only reaches `UpdateLdapSettingsAsync` when `_ldapModuleConfig.IsEnabled` is `true`; `DeleteAllUsersByAuthSourceAsync` iterates `UserManager.Users`, which can be stubbed through the underlying `UserStore` substitute.
- `EafWorkerBase.L` has two `params object[] args` overloads; passing `Array.Empty<object>()` covers the `args.Length > 0` false branch and returns the raw localized key.
- `UserAppService.CreateUsersByActiveDirectory` keeps `Name`, `Surname`, `EmailAddress` when the Azure AD user already has them and strips the domain only when `UserName` contains `@`.
- `EafSqliteCache.DefaultAbsoluteExpireTime` can be set per instance and combined with `slidingExpireTime`; `ObjectToByteArray(null)` returns an empty byte array and `ByteArrayToObject(null/empty)` returns `default`.
- `EafSqlServerCache.TryGetValue` catch branch is reached when `IDistributedCache.GetAsync` throws; `ByteArrayToObject(null)` and `ByteArrayToObject(Array.Empty<byte>())` cover the null/empty branches.
- `PermissionAppService`, `RoleAppService`, `MiddlewareAppServiceBase`, `TenantAppService`, `LdapAuthenticationSource`, `LdapSettings`, `ChatHub`, `TokenAuthController`, `MiddlewareWebCoreModule`, `EafHangfireApplicationBuilderExtensions`, `EafHangfireAuthorizationFilter`, `ServiceBusQueueAppender` and `OpenIdConnectAuthProviderApi` still contain Linux/infrastructure-limited or sealed-builder branches; document as inalcançáveis for P59.

## P57 gotchas
- `NSubstitute` returns `string.Empty` for unconfigured `string` members, which makes `MiddlewareAppServiceBase.L` and `EafWorkerBase.L` return empty strings when `GetStringOrNull` is not explicitly set. Use `NullLocalizationManager.Instance` or configure `GetStringOrNull` to return `null` so the fallback key is used.
- `AccountAppService.Impersonate` throws `UserFriendlyException` for inactive tenants; set `TenantManager` and `LocalizationManager` and assert the exception message contains `TenantIdIsNotActive`.
- `RoleAppService.GetRoles` filters by permission name using `r.Permissions.Any(...)`; ensure every `Role` in the mocked `Roles` query has a non-null `Permissions` collection.
- `RoleAppService.DeleteRole` removes users from the role before deleting; when `GetUsersInRoleAsync` returns an empty list, `RemoveFromRoleAsync` is not called.
- `TenantAppService.GetTenantFeaturesForEdit` filters features by `FeatureScopes.Tenant`; mock `FeatureManager.GetAll()` with mixed `Tenant` and `Edition` features and fix `ObjectMapper.Map<List<FlatFeatureDto>>` to return one item per source element.
- `AzureActiveDirectoryAuthenticationSource.CreateUserAsync` and `UpdateUserAsync` catch `AbpException` from Graph `GetAsync()` and fall back to creating/updating a basic user with `IsActive = true`.
- `ProfileControllerBase.GetProfilePictureByUser` validates `ModelState`; add a model error to cover the `InvalidRequest` branch.
- `EafHangfireApplicationBuilderExtensions.UseEafHangfire()` without an `optionsAction` still registers the dashboard and server when `EafHangfireOptions.JobExecutionEnabled` is true.
- `ChatHub.DeleteMessage` returns a `Could not find chat message` string when the found `ChatMessage.SharedMessageId` is null.
- `ChatHub.SendMessage` returns `InternalServerError` when either `UserId` or `GroupId` is zero for non-group/user messages.
- `EafWorkerBase.L(string, params object[])` returns the raw key when `args` is null; `LocalizationSource` property caches the resolved `ILocalizationSource` so `GetSource` is called once.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry()` without options and with `ConsoleExporter=false`/`OtlpEndpoint=null` returns a configured `IOpenTelemetryBuilder`.
- `ServiceBusQueueAppender.SendBuffer` skips sending when `StorageType` is empty, returning without invoking the broker.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafSqliteCache`, `EafSqlServerCache`, `LdapSettings`, `EafHangfireAuthorizationFilter`, `TokenAuthController` and `OpenIdConnectAuthProviderApi` still contain branches infeasible on Linux or dependent on real infrastructure (LDAP, Redis, Hangfire/SignalR, MSAL). Document them as inalcançáveis for P58.

## P56 gotchas
- `HostSettingsAppService.UpdateAllSettings` has many branches around null sub-DTOs (`UserManagement`, `Email`, `Security`, `ExternalLoginProvider`, `LogDeleter`, `LoginImpersonator`), empty JSON, `IsNullOrWhiteSpace` ternaries, timezone validation and external-login provider toggles/JSON/claims mapping. Use a full `UpdateAllSettingsInput` with valid JSON for Google/Microsoft/OpenIdConnect/AuthZero and OpenIdConnect claims mapping to exercise them.
- `HostSettingsAppService.GetAllSettings` deserializes `ExternalLoginProvider.Host.*` settings from JSON; `OpenIdConnectClaimsMapping` is a list of `JsonClaimMap` objects. Mock `GetSettingValueForApplicationAsync` to return valid JSON for each host and the claims mapping list.
- `ChatAppService.GetGroupChatMessagesAsync` sets `Side` via a ternary comparing `message.UserId` with current user; mix messages from current user and another user to cover both sides.
- `ChatAppService.MarkGroupMessagesAsReadAsync` has a reverse-messages branch and an online-clients branch. Use messages with different `TargetTenantId` values so the reverse query returns results, and configure `_onlineClientManager.GetAllByUserIdAsync` for the online-clients branch.
- `AzureActiveDirectoryAuthenticationSource.GetUserAsync`/`GetUsersAsync`/`UpdateUserAsync` normalize e-mail when `Mail`/`UserPrincipalName` do not contain `@`, composing `{userName}@{azureTenant}`. The Graph `User.Mail` can be set without `@` while `UserPrincipalName` contains the tenant domain.
- `EafHostBuilderExtensions.UseEafConfiguration` (Core) and `UseAbpConfiguration` (Worker) have branches for `configureLogger == null` and `string.IsNullOrEmpty(prefix)`. Use a real `HostBuilder` in a temp directory with `ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>()))` to avoid runtime exceptions.
- `EafServiceCollectionExtensions.AddEaf<TStartupModule>()` has an `optionsAction == null` path; call `services.AddEaf<WorkerModuleTestDependenciesModule>()` with no arguments.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `TokenAuthController`, `PermissionAppService`, `ServiceBusQueueAppender`, `EafSqliteCache`, `EafSqlServerCache`, `ChatHub`, `EafHangfireAuthorizationFilter`, `EafHangfireApplicationBuilderExtensions`, `OpenIdConnectAuthProviderApi`, `MiddlewareAppServiceBase`, `TenantAppService` and `HostSettingsAppService` still contain Linux-inaccessible or sealed-builder branches. Document as inalcançáveis.

## P55 gotchas
- `UserManager.PasswordValidators` is non-virtual; inject `IPasswordValidator<User>` list into `UserManager` constructor via `CoreManagerTestHelper` overloads to test `TenantManager.CreateWithAdminUserAsync` with a custom validator.
- `UserEmailer` `L` overloads are protected; use `TestUserEmailer` subclass exposing `L(name, args)` and `L(name, culture)` to cover them.
- `HostSettingsAppService.GetLoginImpersonatorAsync` uses `ISettingManager.GetSettingValueForApplicationAsync(string)` (single-param overload) through `GetSettingValueForApplicationAsync<bool>`. Mock the one-param string overload and assert the default value `Enabled = true`.
- `LanguageAppService.GetLanguageTexts` throws `InvalidOperationException` when no default language is found and `UserFriendlyException` when creating a duplicate language. `SkipCount > 0` can be exercised with `GetLanguageTextsInput`.
- `TenantAppService.GetTenantFeaturesForEdit` uses `IFeatureManager` and `IObjectMapper`; `FeatureDefinition` is not public, so substitute `IFeatureManager` and map to `FlatFeatureDto` with `IObjectMapper`.
- `FileController.DownloadTempFile`/`DownloadBinaryFile` return `BadRequest(ModelState)` when `ModelState` is invalid; `BadRequest` returns a `SerializableError`, so assert `BadRequestObjectResult.Value` not null.
- `AboutController.GetAbout` populates `Modules` from `_AbpModuleManager.Modules`; `AbpModuleInfo` constructor requires `(Type, AbpModule, bool)` and `Assembly` is non-virtual, so construct with `Substitute.For<AbpModuleInfo>(typeof(AboutController), Substitute.For<AbpModule>(), false)`.
- `EafHostBuilderExtensions` (Core and Worker) `UseEafConfiguration`/`UseAbpConfiguration` with `prefix` not null cover `config.AddEnvironmentVariables(prefix: prefix)`. Pass `configureAction: null` to use the default action.
- `EafServiceCollectionExtensions.AddEaf` uses `AddCastleLogger` when `castleLoggerFactory` is registered; register `CastleLoggerFactory` in the `ServiceCollection` before calling `AddEaf`.
- `EafOpenTelemetryServiceCollectionExtensions.SetOtlpEnvironmentVariables` catch can be exercised with an OTLP variable key containing `=` (e.g. `OTEL=INVALID`), which makes `Environment.SetEnvironmentVariable` throw `ArgumentException`.
- `EafSqlServerCache.TryGetValue` `if (cached != null)` and `CompressBytesAsync` catch can be exercised by setting a value and reading it back; stub `GetAsync` to return the bytes captured in `SetAsync`.
- `RemoteAuthenticationContextExtensions.AddMappedClaims` returns early when `jsonClaimMap` list is empty.
- `DefaultExternalLoginInfoManager.GetNameAndSurname` returns `(null, null)` when `nameClaim.Value` is empty.
- `ChatHub.Dispose` returns early when `_isCallByRelease` is true; cover with a second `Dispose` call.
- `ChatController.GetUploadedObject` throws `UserFriendlyException` when `ModelState` is invalid.
- `NotificationAppService.DeleteNotification` throws `UserFriendlyException` when the notification belongs to another user.
- `CoreManagerTestHelper` gained overloads to inject `passwordValidators` and reuse `UserManager`/`TenantManager` creation.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `ServiceBusQueueAppender`, `AzureActiveDirectoryAuthenticationSource`, `EafSqliteCache` and `OpenIdConnectAuthProviderApi` still contain Linux-inaccessible or sealed-builder branches. Document as inalcançáveis.

## P54 gotchas
- `ChatMessageManager` overrides `L(string, CultureInfo)` but did not override `L(string, CultureInfo, params object[])`, so the `args` overload fell back to the base `ApplicationService` and ignored the middleware fallback sources. Add `protected override string L(string name, CultureInfo culture, params object[] args)` in `ChatMessageManager` and forward to `MiddlewareLocalizationHelper.Localize`.
- `TokenAuthController` `GetExternalAuthenticationProviders` with `AbpSession.TenantId` set filters by `IsSchemeEnabled` which checks `ClientId`/`ClientSecret` null/empty and then switches by provider name (`OpenIdConnect`, `Microsoft`, `Google`, `AuthZero`, default `Unknown`). Mock `IExternalLoginInfoProvider` per provider to reach each switch case.
- `TokenAuthController` `GetDefaultEnabledProvider` defaults to `System` when none of `Ldap`, `AzureActiveDirectory`, `Microsoft`, `Google`, `AuthZero` or `OpenIdConnect` are enabled. Enable one provider at a time to cover each branch.
- `TokenAuthController.TeamsAuthenticate` throws `AbpException` when `Microsoft_IsEnabled` is false and a different `AbpException` when the provider is enabled but `Microsoft` host is not configured.
- `TokenAuthController.SendTwoFactorAuthCode` throws `UserFriendlyException` on `ModelState` invalid and does not call `_emailSender.SendAsync` when the provider is not `Email`.
- `EmailRealTimeNotifier` `SendNotificationsAsync` only sends e-mail for `MessageNotificationData` and when `UseOnlyIfRequestedAsTarget` is false; for `LocalizableMessageNotificationData` construct `LocalizableString` instead of passing a raw string.
- `MiddlewareLocalizationHelper` `Localize` fallback works for empty `args` and null `source` when the first `SourceNames` throws on `GetSource`.
- `WebLogAppService` `GetLatestWebLogs` returns at most 100 lines; the `ReadLines().Take(100)` branch can be exercised with a file containing more than 100 lines.
- `OpenIdConnectAuthProviderApi` `Surname` falls back to an empty string when the `name` claim has only one word.
- `AzureActiveDirectoryAuthenticationSource` `GetUserAsync`/`GetUsersAsync` normalize e-mail without `@` by composing `{userName}@tenantName` when `mail`/`UserPrincipalName` are missing.
- `ServiceBusQueueAppender` `OnClose` catch branch is unreachable because `ServiceBusConnection` is not sealed but `IsClosedOrClosing` and `CloseAsync` are non-virtual/non-override, so `NSubstitute` cannot force `CloseAsync` to throw. Document as inalcançável.
- `LdapAuthenticationSource` and `MiddlewareWebCoreModule` still contain Linux-inaccessible branches (real LDAP, Hangfire/Redis/SQL Server, `??` fallback). Document as inalcançáveis.

## P53 gotchas
- `AbpUserManager.GetOldUserNameAsync` is `protected virtual` and the admin-rename branch in `UserManager.UpdateWithValidateAsync` is not reachable with pure NSubstitute. Use a `Moq.Mock<UserManager>` with `CallBase = true` and `mock.Protected().Setup<Task<string>>("GetOldUserNameAsync", 1L).ReturnsAsync("admin")` to cover it.
- `UserManager.UpdateWithValidateAsync` with duplicate username can be covered by substituting `CheckDuplicateUsernameOrEmailAddressAsync` to return `IdentityResult.Failed`.
- `UserManager.SetGrantedPermissionsAsync` and `SetRolesAsync` are `virtual`; use `userManager.When(x => x.SetGrantedPermissionsAsync(...)).CallBase()` and configure `GetGrantedPermissionsAsync`/`GrantPermissionAsync` and `AddToRoleAsync`.
- `AppAzureActiveDirectoryAuthenticationSource.GetUserAsync` and `GetUsersAsync` had to be made `virtual` in `AzureActiveDirectoryAuthenticationSource<TUser, TTenant>` so NSubstitute can return mocked users for `UserAppService.CreateUsersByActiveDirectory` tests.
- `UserAppService.CreateUsersByLdap` ignores empty or already-existing usernames; `UserManager.GetUserByLoginAsync` is non-virtual and relies on the underlying `_userRepository` substitute.
- `FriendshipAppService.BlockUser` with online clients calls `_chatCommunicator.SendUserStateChangeToClients`; configure `_onlineClientManager.GetAllByUserIdAsync` to return a list.
- `FriendshipAppService.CreateFriendshipRequestByUserName` with a missing tenancy name throws `UserFriendlyException`; configure `TenantManager.FindByTenancyNameAsync` to return `null` and `LocalizationManager` to avoid null reference.
- `TokenAuthController.Authenticate` and `ExternalAuthenticate` validate `ModelState` and throw `UserFriendlyException` when invalid.
- `MiddlewareCoreModuleIntegrationTests` must use `Abp.AbpBootstrapper.Create(typeof(...), options => options.IocManager = new IocManager())` to avoid `UnitOfWorkDefaultOptions` duplicate registration in the static `IocManager.Instance`.
- `LdapAuthenticationSource` and `MiddlewareWebCoreModule` still contain branches that are infeasible on Linux (real LDAP, Hangfire/Redis/SQL Server, `??` constructor fallback). Document these as inalcançáveis.
- `PermissionAppService` 92.5% branch `permission.Children == null` remains unreachable because `Permission.Children` uses `ImmutableList`.

## P52 gotchas
- `MiddlewareControllerBase.L(string, CultureInfo)` is protected; create a `TestableController` subclass that exposes `CallLWithCulture` to call `L(name, culture)`.
- `EafKeyVaultConfigurationProvider` with an unknown `Provider` falls through `KeyVaultManagerFactory` to `NullKeyVaultManager`; `Load()` completes and `TryGet` returns false.
- `DefaultLanguagesCreator.Create` and `TenantRoleAndUserBuilder.Create` are idempotent; call them twice to cover the duplicate branches and exercise `UsingDbContext` on the `SampleAppDbContext`.
- `ProfileAppService.GetProfilePicture` with `ProfilePictureId` set returns a base64 string from `GetProfilePictureById`. `GetProfilePictureByUser`/`GetFriendProfilePicture` with `ProfilePictureId == null` return empty string. `UpdateProfilePicture` throws `UserFriendlyException` when the decoded image exceeds `MaxProfilPictureBytes` (5 MB); use a generated 24-bit BMP (e.g., 1400x1400) to exceed the limit.
- `ChatAppService.GetUserChatMessages` calls `SetTargetUserNamesAsync`; set `UserManager.GetUserByIdAsync` to return a user to cover the `try` exit path, otherwise the catch path is taken.
- `ChatAppService.MarkAllUnreadMessagesOfUserAsRead` with a valid `UserId` but no matching messages returns early; it also covers the `!messages.Any()` branch and `!reverseMessages.Any()` branch.
- `ChatMessageManager.SendMessageAsync` with `UserManager.GetUserOrNullAsync` returning null throws `UserFriendlyException` with `TargetUserNotFoundProbablyDeleted`. Supplying an online client list covers `HandleReceiverToSenderAsync` `clients.Any()` branch.
- `ChatMessageManager.HandleSenderUserInfoChangeAsync` returns early when `senderAsFriend` info is unchanged or when `friendship` is null; configure `UserFriendsCache` accordingly.
- `PermissionAppService` 92.5% branch `permission.Children == null` is unreachable because `Permission.Children` getter uses `ImmutableList.CreateRange` and throws `ArgumentNullException` when `_children` is null.
- `LdapAuthenticationSource`, `TokenAuthController`, `UserAppService`, `UserManager`, `FriendshipAppService`, `MiddlewareCoreModule`, `MiddlewareWebCoreModule`, `AzureActiveDirectoryAuthenticationSource`, `LdapSettings` and `ServiceBusQueueAppender` had Linux-inaccessible branches or complex mock setup; P53 covered `UserManager`, `UserAppService`, `FriendshipAppService`, `MiddlewareCoreModule`, `TokenAuthController`, `AzureActiveDirectoryAuthenticationSource` and `LdapAuthenticationSource` reachable branches. `MiddlewareWebCoreModule`, `LdapSettings` and `ServiceBusQueueAppender` remain for P54 if no accessible branch is found.

## P51 gotchas
- `WorkerContentFileProvider` uncovered lines were the `if (fileInfo.Exists)` return and `if (directory.Exists)` return. Create a real temp file and subdirectory under `IHostEnvironment.ContentRootPath` to cover both branches.
- `MiddlewareWorkerModule.PreInitialize` registers `Configuration.ReplaceService(typeof(IEmailSenderConfiguration), ...)` as a lambda. The lambda body is not executed during `PreInitialize`; invoke it via `configuration.GetType().GetProperty("ServiceReplaceActions")` and assert the `IEmailSenderConfiguration` component is registered in the `IocContainer`.
- `AuthZeroAuthProviderApi.GetUserInfo` has branches for empty `Endpoint` (throws `AbpException`), non-`https` domain prefix, and the `Picture` base64 conversion (`bytes.Any()`). Supply a mocked `HttpClient` with `HttpStatusCode.OK` and `StringContent` for the picture URL to exercise the base64 branch.
- `LdapAuthenticationSource` and `MiddlewareWebCoreModule` still contain branches that are infeasible on Linux (real LDAP, Hangfire infrastructure, `??` constructor fallback). Document these as inalcançáveis rather than changing production code.
- `UserAppService`, `TokenAuthController`, `MiddlewareControllerBase`, `UserManager`, `ChatAppService`, `FriendshipAppService`, `ChatMessageManager`, `PermissionAppService`, `ProfileAppService`, `MiddlewareCoreModule`, `AzureActiveDirectoryAuthenticationSource`, `LdapSettings`, `EafKeyVaultConfigurationProvider`, `ServiceBusQueueAppender`, `DefaultLanguagesCreator` and `TenantRoleAndUserBuilder` remain as accessible targets for P52.

## P50 gotchas
- `NamespaceStripper.StripGenericNamespace` has a final `for` loop that appends `>` for unclosed `openBracketCount`. A generic name like `System.Collections.Generic.List`1[[Foo]]` (no comma in the type argument) leaves `openBracketCount` > 0 and covers the loop.
- `EafWebHookReceiver` uncovered members were `ReceiverName`, `context`, `CurrentUnitOfWork`, and `L(string, params object[])`. They are reachable via public properties and a `TestWebhookReceiver` subclass that exposes the protected `L` overload and `CurrentUnitOfWork`.
- `DbCommandPool` reached 100% line coverage from existing tests; no new tests needed in P50.
- `UserAppService`, `TokenAuthController`, `MiddlewareWorkerModule` and `WorkerContentFileProvider` still have accessible branches but require more complex mock setup; these remain the main focus for P51.

## P49 gotchas
- The `main` branch advanced after P48 with commit `c4450a3` (Sonar fixes), adding `SanitizeForLog` in `AbpLoginResultTypeHelper` and `Equals`/`GetHashCode` in `PasswordComplexitySetting`. This temporarily lowered the P49 baseline before new BDD tests recovered the metrics.
- `PasswordComplexitySetting.GetHashCode` and `Equals(object)` are reachable and should be covered with identical/equivalent instances and cross-type comparisons.
- `AbpLoginResultTypeHelper.SanitizeForLog` is private and reachable via `CreateExceptionForFailedLoginAttempt`; test `null` input and `\r`/`\n` replacement. The protected `L(string, CultureInfo)` overload can be covered with a `TestableAbpLoginResultTypeHelper` subclass.
- `EafOpenTelemetryServiceCollectionExtensions` line coverage improved from 90.6% to 98.1% due to test coverage added in previous P42–P48 runs; ensure no `OpenTelemetry` test changes are needed unless new branches are added.
- `LdapAuthenticationSource` and `MiddlewareWebCoreModule` still contain branches that are infeasible on Linux (real LDAP, Hangfire infrastructure, `??` constructor fallback). Document these as inalcançáveis rather than changing production code.

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

## P42 gotchas
- `Novell.Directory.Ldap.LdapConnection` `SearchAsync`, `ConnectAsync`, `BindAsync`, `Disconnect` and `Connected` are `virtual` but `IsFinal` in ABP 10.4.0; `Castle DynamicProxy` cannot override them. Use `ILdapSearchResults` (interface) for NSubstitute, and test `CreateLdapContext` branch logic only.
- `HangfireBackgroundJobManager.EnqueueAsync`/`Enqueue` have separate branches for `IBackgroundJob<TArgs>`, `Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>` and `Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>`, with and without `delay`; test each combination.
- `ExpiredEntityLogDeleterWorker` has two `Delete` catch branches based on `MaxDeletionCount` and exception `Message` count; exercise both with `IRepository<EntityChange, long>` returning exceptions.
- `RedisConfigurer.Configure` uses `bool.Parse` on `RedisCache:IsEnabled`/`IsRedisEnabled` and `Configuration.GetValue` for `RedisCache:DatabaseId`; use `ConfigurationBuilder` with `Dictionary<string, string?>` and resolve `IOptions<RedisCacheOptions>` to cover the lambda body.
- `ServiceBusQueueAppender.AppendBuffer` builds a `BrokeredMessage` and logs the body; set `ServiceBusConnection.OperationTimeout = 1ms` to force `ServiceBusTimeoutException` and cover the `SendAsync` catch branch.
- `MiddlewareWorkerModule.PreInitialize` calls `Configuration.Caching.ConfigureAll` and `Configuration.ReplaceService`; `ConfigureAll` is exercised by resolving `ICacheManager`/`GetCache` after `PreInitialize`. Calling all `ServiceReplaceActions` is unsafe because `Castle Windsor` may throw duplicate component registration.
- `MiddlewareWebCoreModule.PostInitialize` uses concrete `AbpStartupConfiguration`; create it via `Type.GetType`/`Activator.CreateInstance` and set `BackgroundJobs`, `Auditing` and `EntityHistory` properties to avoid `NullReferenceException`.
- `FriendshipManager` has three `protected` `L` overloads; create a `TestableFriendshipManager` subclass and call the overloads to cover them.
- `WebContentDirectoryFinder` `CalculateContentRootFolder` throws `DirectoryNotFoundException` when `src/Eaf.Middleware.Web.Host` is not found; test the exception branch and the `directoryInfo.Parent == null` path with a temporary directory.

## P44 gotchas
|- `EafStartupConfigurationExtensions.GetChildren` throws `AbpException` on duplicate keys; use `NSubstitute` for `IConfigurationSection` and `GetChildren` with duplicate children to cover the branch.
|- `IConfigurationSection.Exists()` is an extension method; cannot be mocked with `NSubstitute`. Set `IConfigurationSection.Value` to a non-null value and `GetChildren()` to non-empty to make `Exists()` return true.
|- `AddSession` lambda body in `EafServiceCollectionMiddlewareExtensions.AddEafConfigurer` is only executed when `IOptions<SessionOptions>` is resolved; include `provider.GetRequiredService<IOptions<SessionOptions>>().Value` in the full-config test to reach 100% line coverage.
|- `EafMiddlewareCoreSampleAppModule.Initialize` requires a real `IocManager` and `IAbpStartupConfiguration` with `IAbpAutoMapperConfiguration.Configurators` set to a real `List<Action<IMapperConfigurationExpression>>`; otherwise `Configurators.Add` throws `NullReferenceException`.
|- `Substitute.For<ILdapConnection>()` `SearchAsync` returns `Task<ILdapSearchResults>`; use `.Returns(callInfo => CriarSearchResults())` instead of `.Returns(CriarSearchResults())` when `CriarSearchResults` configures other substitutes, so `NSubstitute` does not lose the last call context.
|- `LdapAuthenticationSource.CreateUserAsync` delegates to `DefaultExternalAuthenticationSource.CreateUserAsync`, which sets `IsEmailConfirmed` and `IsActive` to `true`; do not assert these flags when testing the empty-search result branch.
|- `MiddlewareWebCoreModule.PostInitialize` with `Hangfire:IsEnabled=true` and `Database:Provider=SqlServer` plus `ConnectionStrings:Default` sets `JobStorage.Current = new SqlServerStorage(...)`; set `Auditing.IsEnabled=true` and `EntityHistory.IsEnabled=true` to exercise `SetExpiredAuditWoker`/`SetExpiredHistoryEntityWoker` and `RecurringJob.AddOrUpdate`.

## P46 gotchas
- `LdapAuthenticationSource` `CreateUserAsync`/`UpdateUserAsync` with `tenant == null` reuses the same code paths; add `tenant` null tests to cover the `tenant?.Id` branches and `GetDomain`/`GetContainer` null-tenant fallbacks.
- `LdapAuthenticationSource.CreateLdapContext` has separate branch logic for domain/container strings containing `DC=`, `.` or `\`; test these combinations to raise branch coverage without a real LDAP server.
- `CreateLdapContext` with `userName`/`password` null falls back to configured credentials; the `userName != null` prefix branch only runs when the domain does not contain `DC=` or `.`.
- `HangFireConfigurer.ResolveStorageType` defaults to `InMemory` when `Hangfire:IsInMemoryDatabase=false`, `Database:Provider` is not `SqlServer`/`MSSQL` and `RedisCache:IsEnabled`/`IsRedisEnabled` are false.
- `MiddlewareWebCoreModule.PostInitialize` `JobStorage.Current` is always reassigned; `recurringJobs`/`failedJobs` loops cannot be populated without real Hangfire infrastructure.
- `MiddlewareWebCoreModule.SetAppFolders` `CompositeFileProvider` catch is not triggered by a single `null` `IFileProvider`; it requires an `IEnumerable<IFileProvider>` overload to throw.
- SonarCloud link in READMEs should point to `https://sonarcloud.io/project/overview?id=afonsoft_EAF2`, not `summary/overall?id=afonsoft_EAF2&branch=main`.

## P47 gotchas
- `MiddlewareWebCoreModule.PostInitialize` `RedisConnectionString` null branch (`?? "localhost"`) is not reachable without `RedisStorage` constructor throwing because no Redis server is available; testing it causes `RedisConnectionException` outside the `try` block.
- `MiddlewareWebCoreModule.PostInitialize` `recurringJobs`/`failedJobs` loops cannot be populated with data because `JobStorage.Current` is always reassigned during `PostInitialize` and `InMemoryStorage` returns empty collections.
- `LdapAuthenticationSource.SearchWithLimit` is Windows-only and throws `PlatformNotSupportedException` on Linux; cover it by asserting the exception.
- `EafHostBuilderExtensions.UseAbpConfiguration` and `EafWebHostBuilderExtensions.UseEafConfiguration` have default lambdas with `if (!string.IsNullOrEmpty(prefix))` branches; cover both with `Substitute` `ConfigureAppConfiguration` and `IHostBuilder`/`IWebHostBuilder`.
- `EafServiceCollectionExtensions.AddEafWithoutCreatingServiceProvider` has a `removeConventionalInterceptors` false branch; cover it by passing `removeConventionalInterceptors: false`.
- `Eaf.Middleware.Ldap` assembly line coverage is 68.1%; class `LdapAuthenticationSource<T1, T2>` is at 61.8% due to Windows-only `CreatePrincipalContext`/`UpdateUserFromPrincipal`/`ValidateCredentials` paths.
- `Eaf.Middleware.Web.Core` assembly line coverage is 96.1%; `MiddlewareWebCoreModule` is at 86.2% with branch 70% due to unreachable Hangfire cleanup loops.

## P45 gotchas
- `LdapAuthenticationSource` `GetAttribute` returns `null` when the attribute is missing; the `mail` attribute fallback returns `String.Empty` and the `EmailAddress` is `null` because the `Name` is split by `StringSplitOptions.None` and joined with space.
- `Substitute.For<ILdapConnection>().Connected` returns `false` by default; override `Connected` to `true` to test `TryAuthenticateAsync` success branches.
- `WebContentDirectoryFinder.CalculateContentRootFolder` can be driven to the `coreAssemblyDirectoryPath == null` and `directoryInfo.Parent == null` branches by loading `Eaf.Middleware.Core.dll` into a custom `AssemblyLoadContext` with `LoadFromStream` (no `Location`) or a copy in a temporary directory without solution files.
- `MiddlewareWebCoreModule` constructor calls `GetAppConfiguration` which sets `ASPNETCORE_ENVIRONMENT` before the fallback chain; to cover the `IsNullOrWhiteSpace` true branches, set `env.EnvironmentName` to `null` and clear all `ASPNETCORE_ENVIRONMENT`, `EAF_ENVIRONMENT`, `Hosting:Environment`, `ASPNET_ENV` and `DOTNET_ENVIRONMENT` variables.
- `MiddlewareWebCoreModule.SetAppFolders` `contentRootPath` uses `Directory.GetCurrentDirectory()` when `ContentRootPath` is null; set `Directory.SetCurrentDirectory` to a temporary directory before calling `PostInitialize`.
- `CompositeFileProvider` does not throw when constructed with a single `null` `IFileProvider`; the `SetAppFolders` catch for `CompositeFileProvider` is effectively unreachable without an `IEnumerable<IFileProvider>` overload.
- `EafMiddlewareCoreSampleAppModule.PreInitialize` `AddDbContext` lambda is not executed by the `IAbpEfCoreConfiguration` substitute; use `efCore.When(...).Do(...)` to capture the `Action<AbpDbContextConfiguration<SampleAppDbContext>>` and invoke it with `new AbpDbContextConfiguration<SampleAppDbContext>(connectionString: "", existingConnection: null)`.
- `AbpDbContextConfiguration<TDbContext>` constructor is `(string connectionString, DbConnection existingConnection)`; `DbContextOptions` property is created internally.
- `EafMiddlewareTemplateDbContextConfigurer.Configure` calls `UseSqlServer(connectionString)` and does not throw for empty connection strings.

## P43 gotchas
- `Novell.Directory.Ldap.LdapConnection` `SearchAsync`, `ConnectAsync`, `BindAsync` and `Connected` are `virtual` but `IsFinal` (sealed) in `Novell.Directory.Ldap.NETStandard` 4.0.0; `NSubstitute` cannot override them with `Returns`. Use `ILdapConnection` instead of `LdapConnection` in production code and `Substitute.For<ILdapConnection>()` in tests.
- `Substitute.For<ILdapConnection>()` implements `IDisposable` because `LdapConnection` implements `IDisposable`, even though `ILdapConnection` itself does not. Use `using (var principalContext = await CreateLdapContext(null) as IDisposable)` and then cast to `ILdapConnection`.
- `ILdapSearchResults` `HasMoreAsync()` and `NextAsync()` can be mocked with a `Queue<LdapEntry>` to simulate one or more results and exceptions.
- `CultureHelper` `IsRtl` and `UsingLunarCalendar` depend on `CultureInfo.CurrentUICulture`; set `CurrentUICulture` to `ar-SA` for RTL and `zh-CN` for lunar calendar and restore in `finally`.
- `EntityHistoryConfigurationExtensions.AddAllAuditedEntities` uses `IEntityHistorySelectorList`; wire `When(x => x.Add(...)).Do(...)` to a real `List<NamedTypeSelector>` to assert idempotent insertion.
- `EafSqliteCache` tests must disable connection pooling (`SqliteConnectionStringBuilder.Pooling = false`) and call `SqliteConnection.ClearAllPools()` after deleting test database files.
- `DefaultSettingsCreator` and `DefaultTenantBuilder` can be covered with `EafMiddlewareTestBase` and `UsingDbContext` using the in-memory SQLite `SampleAppDbContext`.
- `EafMiddlewareTemplateDbContextConfigurer.Configure` accepts `DbContextOptionsBuilder` and `IConfigurationRoot`; use `ConfigurationBuilder().AddInMemoryCollection()` to supply connection strings.

## P41 gotchas
- `UserManager.UpdateWithValidateAsync` is public but not virtual; cannot be stubbed with `NSubstitute.Returns`. Use a real `UserManager` with a substitute `UserStore` and `IRepository<User, long>` to exercise the update branch.
- `ILookupNormalizer` in .NET 10 exposes `NormalizeName` and `NormalizeEmail` (no `Normalize` method).
- `AbpUserStore.FindByNameOrEmailAsync` overloads are `(string userNameOrEmailAddress)` and `(int? tenantId, string userNameOrEmailAddress)`, not `(string, CancellationToken)`.
- `User.Identity.GetUserIdentifierOrNull()` uses `AbpClaimTypes.UserId` and `AbpClaimTypes.TenantId` claims, not `MiddlewareCoreConsts.UserIdentifier`.
- `MiddlewareWebCoreModule` `SetAppFolders` uses `Eaf.Middleware.AppFolders` from `Eaf.Middleware.Core/Net/Folder/AppFolders.cs` and swallows `DirectoryNotFoundException` / `ArgumentException`.
- `MiddlewareWebCoreModule.PostInitialize` configures `JobStorage.Current` with `RedisStorage` (when `RedisCache:IsRedisEnabled` true and `RedisCache:DatabaseId` is a valid integer) or `SqlServerStorage` (when `ConnectionStrings:DefaultNameOrConnectionString` is present and `Hangfire:SqlServer:UseJobsV2` is true).
- `ExpiredAuditLogDeleterWorker` (Hangfire) has two `Delete` catch branches — exercise both above and below `MaxDeletionCount`.
- `UserAppService.NotificationNewUser` is private; invoke via reflection and set `_notificationPublisher`/`_webhookPublisher` to simulate failures and cover the catch blocks.
- `SwaggerOperationFilter` and `SwaggerEnumParameterFilter` are now 100% covered; `TokenAuthController` is at 90.1% and `MiddlewareWebCoreModule` is at 84.8%.

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

## P48 gotchas
- `MiddlewareWebCoreModule` constructor calls `AppConfigurations.Get` before its fallback chain; `ASPNETCORE_ENVIRONMENT` is always set to a non-null value, so the `??` left-null branches in lines 68, 70 and 72 are unreachable.
- `MiddlewareWebCoreModule.PostInitialize` `RedisConnectionString` null branch (`?? "localhost"`) remains unreachable on Linux because `new RedisStorage("localhost")` throws `RedisConnectionException` before `GetConnection`.
- `MiddlewareWebCoreModule.PostInitialize` `recurringJobs`/`failedJobs` loops remain unreachable because `JobStorage.Current` is always reassigned to a fresh `InMemoryStorage`/`SqlServerStorage`/`RedisStorage` and returns empty collections.
- `LdapAuthenticationSource` `CreateLdapContext` `Connected`/`BindAsync`/`SearchConstraints` branches remain unreachable without a real LDAP server or a mockable `ILdapConnection` factory.
- `LdapAuthenticationSource` Windows-only methods (`CreatePrincipalContext`, `UpdateUserFromPrincipal`, `ValidateCredentials`, `SearchWithLimit`) are covered on Linux by asserting `PlatformNotSupportedException`/`NotImplementedException`.

## Notable classes with remaining low coverage (target for P49)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (61.8%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (86.2%)

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

## GameHub backport (feature/eaf-gamehub-backport)
- Centralized CORS in `Eaf.Middleware.Web.Core.Configuration.EafCorsConfiguration.AddEafCors` with origin reflection, wildcard subdomain support, and all headers sent by `EafHttpInterceptor`; template API `Startup.cs` registers it.
- Public error handling: `EafErrorCodes`, `EafPublicErrorMiddleware`, `PublicErrorApplicationBuilderExtensions`, and `EafExceptionFilter` (IExceptionFilter + IAsyncExceptionFilter, Order = 1000 because exception filters execute in reverse order) return `PublicErrorContract` 400 for `UserFriendlyException`.
- Multi-tenant login fallback: `login.component.ts` calls `loginService.authenticate` when `availableTenants` returns empty; `select-tenant` component exposes a "Login as Host" link.
- JWT parsing: `TokenService` in the Angular eaf-ng2-module decodes `sub`, `unique_name`, `name`, `role`, `tenantid`, etc.; backend `TokenAuthController.CreateJwtClaims` adds a `tenantid` claim from `user.TenantId`.
- Tenant header: ABP 10.5 default `TenantIdResolveKey` is `Abp-TenantId` (dash). All EAF clients and the backend must use `Abp-TenantId` as both cookie and header name (`eaf.multiTenancy.tenantIdCookieName`). The aligned files are `EafHttpInterceptor`, `AppPreBootstrap`, `app-auth.service`, `eaf.js`, `MiddlewareControllerBase.SetTenantIdCookie`, `ConsoleApiClient` and `EafCorsConfiguration`. Never send a tenant header/cookie with `null`/empty value; omit it to keep the host context.
- SignalR modernization: `SignalRHelper` uses `@microsoft/signalr` `HubConnectionBuilder` with `accessTokenFactory`; backend `AuthConfigurer.SetToken` reads `access_token` query parameter for `/signalr*` paths.
- Public error UI: `EafHttpConfiguration.handleNonEafErrorResponse` detects `PublicErrorContract` bodies (`message`/`code`) and shows the server message instead of the generic `An error has occurred!` modal.
- Topbar session: `TopBarComponent.setCurrentLoginInformations` guards `appSession.user` so the topbar no longer stays blank when the session is not yet re-initialized.
- Mobile responsiveness: `styles.css` gains `100dvh`, touch targets, centered login drawer.
- Admin UX: reusable `app-status-badge` and `app-empty-state` components; `p-table` loading state in tenants/users components.
- Docker full-stack (`docker-compose.all.yml`) validated: host admin login, tenant creation, tenant admin login with `Abp-TenantId: 2`, `GetAvailableTenants` returns `PublicErrorContract` 400 on invalid credentials, token contains `tenantid`.
- Test notes: `Eaf.Middleware.Web.Core.Tests` gained `EafCorsConfigurationBddTests`, `EafPublicErrorMiddlewareBddTests`, `EafExceptionFilterBddTests` and new SignalR query-string tests in `AuthConfigurerBddTests`; Angular got `token.service.spec.ts` and `login.service.spec.ts` plus updated `login.component.spec.ts`.

## Sonar fix (2026-07-12)
Branch: `feature/devin-20260712-fix-sonar-jwt`
- `src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/MiddlewareJwtSecurityTokenHandler.cs`:
  - S2583 (line 117): `tokenValidityValueInClaims` is never null, so remove the unreachable `?? user.SecurityStamp` fallback from the cache `Set` call.
  - S6667/S2139 (line 131-138): `catch` clauses in `ValidateToken` were logging the exception and rethrowing it, which violates both S6667 (pass exception to log) and S2139 (log or rethrow, not both). Removed the `DebugFormat` calls so the `catch` blocks simply rethrow.
- Tests: `Eaf.Middleware.Web.Core.Tests` passed (662 tests).
