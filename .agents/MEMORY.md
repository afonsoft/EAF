# EAF Coverage Audit Memory

Last session branch: `devin/1783704603-priority25-coverage-audit`
Baseline coverage (P24): Line 63.9%, Branch 48.1%, Method 83.4%.
Current coverage (after P25): Line 66.4%, Branch 49.7%, Method 85.0%.

## Mocking gotchas
- `UserManager.GetUserByLoginAsync(string userName, int? tanantId)` is non-virtual; cannot be mocked with `NSubstitute.Returns`. Tests must rely on the underlying `_userRepository` substitute defaulting to null.
- `AbpUserManager.GetOldUserNameAsync` is protected virtual; the admin-rename branch in `UserManager.UpdateWithValidateAsync` is not reachable with NSubstitute without reflection.
- `IEmailSender.SendAsync` returns `Task` non-generic. To simulate failure, use `emailSender.SendAsync(...).Returns(Task.FromException(new Exception(...)))` — `Throws`/`ThrowsAsync` from `NSubstitute.ExceptionExtensions` is not applicable.
- `SimpleStringCipher.Instance.Encrypt` defaults to `SimpleStringCipher.DefaultPassPhrase` (`gsKnGZ041HLL4IM8`). Web.Core/Worker classes that decrypt token/userId use `MiddlewareCoreConsts.DefaultPassPhrase` (`gsKxGZ012HLL3MI5`). Tests must pass the correct passphrase to `Encrypt`.
- `PerformContext` has no parameterless constructor; create a real instance with `new PerformContext(null, Substitute.For<IStorageConnection>(), new BackgroundJob("id", null, DateTime.UtcNow), Substitute.For<IJobCancellationToken>())`.
- `SmtpClient` is not easily mocked with `NSubstitute` because `Authenticate`/`Connect` are non-virtual/intercept complex. Prefer a `TestableSmtpClient : SmtpClient` that overrides `Authenticate(Encoding, ICredentials, ct)` and `Connect(...)`.
- `BinaryObject` constructor signature is `(int? tenantId, byte[] bytes, string fileType, string fileName)`; the `Id` is generated, so tests that need a specific `Id` must set `binaryObject.Id = fileId` after construction.

## Coverage command
- `bash run-tests-with-coverage.sh` requires `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet` because the script does not export `DOTNET_ROOT`.

## Notable classes with remaining low coverage
- `Eaf.Middleware.Web.Swagger.SwaggerExtensions` (0%) / `SwaggerOperationFilter` (0%) / `SwaggerNullableParameterFilter` (0%) / `SwaggerEnumParameterFilter` (0%)
- `Eaf.Middleware.Web.WebHooks.EafWebhookDefinitionProvider` (0%)
- `Eaf.Middleware.Web.UiCustomization.Metronic.*` (0%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` / `SerilogMvcLoggingAttribute` (0%)
- `Eaf.Notifications.EmailRealTimeNotifier` (0%)
- `Eaf.WebHooks.EafWebHookReceiver` (0%)
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` (16.4%)
- `Eaf.Middleware.Worker.EafServiceCollectionExtensions` / `EafHostBuilderExtensions` (0%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
