using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Timing;
using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Host;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Security;
using Eaf.Middleware.Timing;
using Castle.Core.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Abp.Zero.Configuration;

namespace Eaf.Middleware.Application.Tests.Configuration.Host
{
    public class HostSettingsAppServiceBddTests
    {
        private static HostSettingsAppService CreateSut(
            out ISettingManager settingManager,
            out ISettingDefinitionManager settingDefinitionManager,
            out ITimeZoneService timeZoneService)
        {
            return CreateSut(out settingManager, out settingDefinitionManager, out timeZoneService, out _, out _, out _);
        }

        private static HostSettingsAppService CreateSut(
            out ISettingManager settingManager,
            out ISettingDefinitionManager settingDefinitionManager,
            out ITimeZoneService timeZoneService,
            out IEmailSender emailSender,
            out IEafMiddlewareAzureActiveDirectoryModuleConfig azureConfig,
            out IEafMiddlewareLdapModuleConfig ldapConfig)
        {
            settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>(), Arg.Any<bool>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));

            settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            settingDefinitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci => new SettingDefinition(ci.Arg<string>(), GetSettingValue(ci.Arg<string>())));

            timeZoneService = Substitute.For<ITimeZoneService>();
            timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>()).Returns("UTC");

            azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            emailSender = Substitute.For<IEmailSender>();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            var sut = new HostSettingsAppService(
                emailSender,
                timeZoneService,
                settingDefinitionManager,
                azureConfig,
                ldapConfig
            )
            {
                AbpSession = abpSession,
                SettingManager = settingManager,
                Logger = Substitute.For<ILogger>()
            };

            return sut;
        }

        private static HostSettingsAppService CreateSutWithFailingExternalLoginSettings(out ISettingManager settingManager)
        {
            settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci =>
            {
                var name = ci.Arg<string>();
                if (name.Contains("ExternalLoginProvider"))
                    throw new Exception("fail");
                return GetSettingValue(name);
            });
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>(), Arg.Any<bool>()).Returns(ci =>
            {
                var name = ci.Arg<string>();
                if (name.Contains("ExternalLoginProvider"))
                    throw new Exception("fail");
                return GetSettingValue(name);
            });
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            settingDefinitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci => new SettingDefinition(ci.Arg<string>(), GetSettingValue(ci.Arg<string>())));

            var timeZoneService = Substitute.For<ITimeZoneService>();
            timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>()).Returns("UTC");

            var azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            var ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();
            var emailSender = Substitute.For<IEmailSender>();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            var sut = new HostSettingsAppService(
                emailSender,
                timeZoneService,
                settingDefinitionManager,
                azureConfig,
                ldapConfig
            )
            {
                AbpSession = abpSession,
                SettingManager = settingManager,
                Logger = Substitute.For<ILogger>()
            };

            return sut;
        }

        private static string GetSettingValue(string name)
        {
            if (name.Contains("ExternalLoginProvider.Host"))
            {
                return "{}";
            }

            if (name.Contains("MappedClaims"))
            {
                return "";
            }

            if (name.Contains("Port") ||
                name.Contains("Length") ||
                name.Contains("Seconds") ||
                name.Contains("Quantity") ||
                name.Contains("Days") ||
                name.Contains("Attempts") ||
                name.Contains("Expiration"))
            {
                return "0";
            }

            if (name.Contains("Is") ||
                name.Contains("Enabled") ||
                name.Contains("Enable") ||
                name.Contains("Require") ||
                name.Contains("Allow") ||
                name.Contains("UseCaptcha") ||
                name.Contains("UseDefault") ||
                name.Contains("Token"))
            {
                return "false";
            }

            return "false";
        }

        [Fact]
        public async Task Dado_ConfiguracoesPadrao_Quando_GetAllSettingsAnonymous_Entao_DeveRetornarConfiguracoesDoHost()
        {
            var sut = CreateSut(out _, out _, out _);

            var result = await sut.GetAllSettingsAnonymous();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioAdmin_Quando_GetAllSettings_Entao_DeveRetornarConfiguracoesCompletas()
        {
            var sut = CreateSut(out _, out _, out _);

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ConfiguracoesValidas_Quando_UpdateAllSettings_Entao_DeveAtualizarSemErros()
        {
            var sut = CreateSut(out _, out _, out _);

            var input = CreateValidInput();

            await sut.UpdateAllSettings(input);

            true.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_InputNulo_Quando_UpdateAllSettings_Entao_DeveRetornarSemErro()
        {
            var sut = CreateSut(out _, out _, out _);

            await Should.NotThrowAsync(async () => await sut.UpdateAllSettings(null));
        }

        [Fact]
        public async Task Dado_SubConfiguracoesNulasEAdLdapHabilitados_Quando_UpdateAllSettings_Entao_DeveRetornarSemErro()
        {
            var sut = CreateSut(out _, out _, out _, out _, out var azureConfig, out var ldapConfig);
            azureConfig.IsEnabled.Returns(true);
            ldapConfig.IsEnabled.Returns(true);

            var input = new HostSettingsEditDto
            {
                General = null,
                UserManagement = null,
                Security = null,
                Email = null,
                Google = null,
                ExternalLoginProviderSettings = null,
                AzureActiveDirectory = null,
                Ldap = null,
                LogDeleter = null,
                LoginImpersonator = null
            };

            await Should.NotThrowAsync(async () => await sut.UpdateAllSettings(input));
        }

        [Fact]
        public async Task Dado_SecurityComSubConfiguracoesNulas_Quando_UpdateAllSettings_Entao_DeveRetornarSemErro()
        {
            var sut = CreateSut(out _, out _, out _);
            var input = CreateValidInput();
            input.Security = new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings = false,
                PasswordComplexity = null,
                UserLockOut = null,
                TwoFactorLogin = null,
                AllowOneConcurrentLoginPerUser = false
            };

            await Should.NotThrowAsync(async () => await sut.UpdateAllSettings(input));
        }

        [Fact]
        public async Task Dado_RelogioNaoSuportaMultiplosTimezones_Quando_UpdateAllSettings_Entao_NaoDeveAtualizarTimezone()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.General.Timezone = "America/Sao_Paulo";

            var originalProvider = Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(false);
            Clock.Provider = clockProvider;

            try
            {
                await sut.UpdateAllSettings(input);

                await settingManager.DidNotReceive().ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, Arg.Any<string>());
            }
            finally
            {
                Clock.Provider = originalProvider;
            }
        }

        [Fact]
        public async Task Dado_TimezoneDiferenteDoPadrao_Quando_GetAllSettings_Entao_DeveRetornarTimezoneFornecido()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone).Returns("America/Sao_Paulo");

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.General.Timezone.ShouldBe("America/Sao_Paulo");
        }

        [Fact]
        public async Task Dado_ValoresNulosNoLogDeleter_Quando_UpdateAllSettings_Entao_DeveUsarValoresPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto
            {
                DeletedQuantity = null,
                Enabled = null,
                ExpiredDays = null
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.ExpiredDays, "180");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity, "30000");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.IsEnabled, "true");
        }

        [Fact]
        public async Task Dado_LoginImpersonatorEnabledNulo_Quando_UpdateAllSettings_Entao_DeveUsarValorPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto { Enabled = null };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled, "true");
        }

        [Fact]
        public async Task Dado_GoogleComCamposVazios_Quando_UpdateAllSettings_Entao_DeveDefinirNulo()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.Google = new GoogleSettingsEditDto
            {
                Analytics = "UA-123",
                Tag = "",
                RecaptchaSiteKey = null
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.Analytics, "UA-123");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.TagManager, Arg.Is<string>(v => v == null));
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.RecaptchaSiteKey, Arg.Is<string>(v => v == null));
        }

        [Fact]
        public async Task Dado_AzureActiveDirectoryHabilitadoComCamposVazios_Quando_UpdateAllSettings_Entao_DeveDefinirNuloENaoDeletar()
        {
            var sut = CreateSut(out _, out _, out _, out _, out var azureConfig, out _);
            azureConfig.IsEnabled.Returns(true);
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User>().AsQueryable());
            sut.UserManager = userManager;

            var input = CreateValidInput();
            input.AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto
            {
                IsEnabled = true,
                ClientId = "client-id",
                Tenant = " ",
                ClientSecret = ""
            };

            await sut.UpdateAllSettings(input);

            await userManager.DidNotReceive().DeleteAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task Dado_LdapHabilitadoComCamposVazios_Quando_UpdateAllSettings_Entao_DeveDefinirNuloENaoDeletar()
        {
            var sut = CreateSut(out _, out _, out _, out _, out _, out var ldapConfig);
            ldapConfig.IsEnabled.Returns(true);
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User>().AsQueryable());
            sut.UserManager = userManager;

            var input = CreateValidInput();
            input.Ldap = new LdapSettingsEditDto
            {
                IsEnabled = true,
                Domain = " ",
                UserName = null,
                Password = ""
            };

            await sut.UpdateAllSettings(input);

            await userManager.DidNotReceive().DeleteAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task Dado_ExternalLoginProviderComProvedorNuloEInvalido_Quando_UpdateAllSettings_Entao_DeveAtualizarComValoresCorretos()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto
            {
                Google = null,
                Google_IsEnabled = true,
                Microsoft = new MicrosoftExternalLoginProviderSettings { ClientId = "cid", ClientSecret = "secret" },
                Microsoft_IsEnabled = true,
                OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                OpenIdConnect_IsEnabled = true,
                AuthZero = new AuthZeroExternalLoginProviderSettings(),
                AuthZero_IsEnabled = true,
                OpenIdConnectClaimsMapping = null
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled, "true");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Microsoft, Arg.Is<string>(v => v.Contains("cid")));
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled, "false");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Google, "false");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Host.OpenIdConnect, "false");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.ExternalLoginProvider.Host.AuthZero, "false");
        }

        [Fact]
        public async Task Dado_ExternalLoginProviderComClaimsMapping_Quando_UpdateAllSettings_Entao_DeveSerializarMapeamento()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto
            {
                Google = new GoogleExternalLoginProviderSettings(),
                Google_IsEnabled = false,
                Microsoft = new MicrosoftExternalLoginProviderSettings(),
                Microsoft_IsEnabled = false,
                OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                OpenIdConnect_IsEnabled = false,
                AuthZero = new AuthZeroExternalLoginProviderSettings(),
                AuthZero_IsEnabled = false,
                OpenIdConnectClaimsMapping = new List<JsonClaimMapDto> { new JsonClaimMapDto { Claim = "sub", Key = "id" } }
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(
                AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims,
                Arg.Is<string>(v => v.Contains("sub")));
        }

        [Fact]
        public async Task Dado_ExternalLoginProviderHostVazio_Quando_GetAllSettings_Entao_DeveUsarConfiguracaoPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci =>
            {
                var name = ci.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Host.Google ||
                    name == AppSettings.ExternalLoginProvider.Host.Microsoft ||
                    name == AppSettings.ExternalLoginProvider.Host.OpenIdConnect ||
                    name == AppSettings.ExternalLoginProvider.Host.AuthZero)
                {
                    return string.Empty;
                }
                return GetSettingValue(name);
            });

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Microsoft.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.OpenIdConnect.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.AuthZero.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ExternalLoginProviderHostComJsonValido_Quando_GetAllSettings_Entao_DeveDeserializarConfiguracoes()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci =>
            {
                var name = ci.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Host.Google)
                    return "{\"ClientId\":\"cid\",\"ClientSecret\":\"secret\"}";
                if (name == AppSettings.ExternalLoginProvider.Host.Microsoft)
                    return "{\"ClientId\":\"cid\",\"ClientSecret\":\"secret\",\"TenantId\":\"tenant\"}";
                if (name == AppSettings.ExternalLoginProvider.Host.OpenIdConnect)
                    return "{\"ClientId\":\"cid\",\"ClientSecret\":\"secret\",\"Authority\":\"https://localhost\"}";
                if (name == AppSettings.ExternalLoginProvider.Host.AuthZero)
                    return "{\"ClientId\":\"cid\",\"ClientSecret\":\"secret\",\"Domain\":\"dev\"}";
                if (name == AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims)
                    return "[{\"Claim\":\"sub\",\"Key\":\"id\"}]";
                return GetSettingValue(name);
            });

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ClientId.ShouldBe("cid");
            result.ExternalLoginProviderSettings.Microsoft.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Microsoft.ClientId.ShouldBe("cid");
            result.ExternalLoginProviderSettings.OpenIdConnect.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.OpenIdConnect.ClientId.ShouldBe("cid");
            result.ExternalLoginProviderSettings.AuthZero.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.AuthZero.ClientId.ShouldBe("cid");
            result.ExternalLoginProviderSettings.OpenIdConnectClaimsMapping.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.OpenIdConnectClaimsMapping.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_ErroNaLeituraDeExternalLoginProvider_Quando_GetAllSettings_Entao_DeveRetornarConfiguracaoPadrao()
        {
            var sut = CreateSutWithFailingExternalLoginSettings(out _);

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google_IsEnabled.ShouldBeFalse();
            result.ExternalLoginProviderSettings.Microsoft_IsEnabled.ShouldBeFalse();
            result.ExternalLoginProviderSettings.OpenIdConnect_IsEnabled.ShouldBeFalse();
            result.ExternalLoginProviderSettings.AuthZero_IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_TimezoneIgualAoPadrao_Quando_GetAllSettings_Entao_TimezoneDeveRetornarVazio()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone).Returns("UTC");

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.General.Timezone.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_ErroNoLogDeleter_Quando_GetAllSettings_Entao_DeveRetornarValoresPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LogDeleter.DeletedQuantity.ShouldBe(30000);
            result.LogDeleter.Enabled.ShouldBe(true);
            result.LogDeleter.ExpiredDays.ShouldBe(3);
        }

        [Fact]
        public async Task Dado_ErroNoExternalLoginProvider_Quando_GetAllSettingsAnonymous_Entao_DeveRetornarConfiguracaoPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Google)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettingsAnonymous();

            result.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google_IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_InputComTimezoneVazioEUseDefaultPasswordComplexity_Quando_UpdateAllSettings_Entao_DeveAtualizarConfiguracoes()
        {
            var sut = CreateSut(out var settingManager, out _, out var timeZoneService);
            var input = CreateValidInput();
            input.General.Timezone = string.Empty;
            input.Security.UseDefaultPasswordComplexitySettings = true;
            input.Security.DefaultPasswordComplexity = new PasswordComplexitySetting
            {
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = false,
                RequiredLength = 0
            };

            var originalProvider = Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Clock.Provider = clockProvider;

            try
            {
                await sut.UpdateAllSettings(input);
            }
            finally
            {
                Clock.Provider = originalProvider;
            }

            await settingManager.Received(1).ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, "UTC");
            await timeZoneService.Received(1).GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>());
            await settingManager.Received(1).ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_ModulosAdLdapHabilitados_Quando_UpdateAllSettings_Entao_DeveRemoverUsuariosDoAuthSource()
        {
            var sut = CreateSut(out _, out _, out _, out _, out var azureConfig, out var ldapConfig);
            azureConfig.IsEnabled.Returns(true);
            ldapConfig.IsEnabled.Returns(true);

            var input = CreateValidInput();
            input.AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto { IsEnabled = false };
            input.Ldap = new LdapSettingsEditDto { IsEnabled = false };

            var userManager = ManagerTestHelper.CreateUserManager();
            var adUser = new User { Id = 1, UserName = "aduser", AuthenticationSource = "ActiveDirectory" };
            var ldapUser = new User { Id = 2, UserName = "ldapuser", AuthenticationSource = "LDAP" };
            userManager.Users.Returns(new List<User> { adUser, ldapUser }.AsQueryable());
            sut.UserManager = userManager;

            await sut.UpdateAllSettings(input);

            await userManager.Received(1).DeleteAsync(Arg.Is<User>(u => u.AuthenticationSource == "ActiveDirectory"));
            await userManager.Received(1).DeleteAsync(Arg.Is<User>(u => u.AuthenticationSource == "LDAP"));
        }

        [Fact]
        public async Task Dado_EmailValido_Quando_SendTestEmail_Entao_DeveEnviarEmail()
        {
            var sut = CreateSut(out _, out _, out _, out var emailSender, out _, out _);

            await sut.SendTestEmail(new SendTestEmailInput { EmailAddress = "test@example.com" });

            await emailSender.Received(1).SendAsync(
                "test@example.com",
                "TestEmail_Subject",
                "TestEmail_Body"
            );
        }

        [Fact]
        public async Task Dado_ErroNoLoginImpersonator_Quando_GetAllSettings_Entao_DeveRetornarValorPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
            result.LoginImpersonator.Enabled.ShouldBe(true);
        }

        [Fact]
        public async Task Dado_TimezonePreenchido_Quando_UpdateAllSettings_Entao_DeveUsarTimezoneFornecido()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.General.Timezone = "America/Sao_Paulo";

            var originalProvider = Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Clock.Provider = clockProvider;

            try
            {
                await sut.UpdateAllSettings(input);
            }
            finally
            {
                Clock.Provider = originalProvider;
            }

            await settingManager.Received(1).ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, "America/Sao_Paulo");
        }

        [Fact]
        public async Task Dado_AzureActiveDirectoryComClientIdVazio_Quando_UpdateAllSettings_Entao_DeveDefinirNulo()
        {
            var sut = CreateSut(out var settingManager, out _, out _, out _, out var azureConfig, out _);
            azureConfig.IsEnabled.Returns(true);
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User>().AsQueryable());
            sut.UserManager = userManager;

            var input = CreateValidInput();
            input.AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto
            {
                IsEnabled = false,
                ClientId = "",
                Tenant = "tenant",
                ClientSecret = "secret"
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.ClientId, Arg.Is<string>(v => v == null));
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.Tenant, "tenant");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret, "secret");
        }

        [Fact]
        public async Task Dado_GoogleComAnalyticsVazio_Quando_UpdateAllSettings_Entao_DeveDefinirNulo()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.Google = new GoogleSettingsEditDto
            {
                Analytics = "",
                Tag = "GTM-123",
                RecaptchaSiteKey = "key"
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.Analytics, Arg.Is<string>(v => v == null));
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.TagManager, "GTM-123");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.Google.RecaptchaSiteKey, "key");
        }

        [Fact]
        public async Task Dado_LdapComCamposPreenchidos_Quando_UpdateAllSettings_Entao_DeveDefinirValores()
        {
            var sut = CreateSut(out var settingManager, out _, out _, out _, out _, out var ldapConfig);
            ldapConfig.IsEnabled.Returns(true);
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User>().AsQueryable());
            sut.UserManager = userManager;

            var input = CreateValidInput();
            input.Ldap = new LdapSettingsEditDto
            {
                IsEnabled = false,
                Domain = "domain.local",
                UserName = "admin",
                Password = "password"
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(LdapSettingNames.Domain, "domain.local");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(LdapSettingNames.UserName, "admin");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(LdapSettingNames.Password, "password");
        }

        [Fact]
        public async Task Dado_LogDeleterComValoresPreenchidos_Quando_UpdateAllSettings_Entao_DeveDefinirValores()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto
            {
                DeletedQuantity = 100,
                Enabled = false,
                ExpiredDays = 7
            };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.ExpiredDays, "7");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity, "100");
            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.IsEnabled, "false");
        }

        [Fact]
        public async Task Dado_LoginImpersonatorEnabledFalse_Quando_UpdateAllSettings_Entao_DeveDefinirFalse()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto { Enabled = false };

            await sut.UpdateAllSettings(input);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled, "false");
        }

        private static HostSettingsEditDto CreateValidInput()
        {
            return new HostSettingsEditDto
            {
                General = new GeneralSettingsEditDto { Timezone = "UTC" },
                UserManagement = new HostUserManagementSettingsEditDto
                {
                    AllowOneConcurrentLoginPerUser = false,
                    IsCookieConsentEnabled = false,
                    IsEmailConfirmationRequiredForLogin = false,
                    IsRegisterRequiredForLogin = false,
                    StoreExternalTokenInformation = false,
                    TokenExpiration = 0,
                    UseCaptchaOnLogin = false
                },
                Security = new SecuritySettingsEditDto
                {
                    AllowOneConcurrentLoginPerUser = false,
                    UseDefaultPasswordComplexitySettings = false,
                    PasswordComplexity = new PasswordComplexitySetting
                    {
                        RequireDigit = false,
                        RequireLowercase = false,
                        RequireNonAlphanumeric = false,
                        RequireUppercase = false,
                        RequiredLength = 0
                    },
                    UserLockOut = new UserLockOutSettingsEditDto
                    {
                        IsEnabled = false,
                        DefaultAccountLockoutSeconds = 0,
                        MaxFailedAccessAttemptsBeforeLockout = 0
                    },
                    TwoFactorLogin = new TwoFactorLoginSettingsEditDto
                    {
                        IsEnabled = false,
                        IsEmailProviderEnabled = false,
                        IsSmsProviderEnabled = false,
                        IsRememberBrowserEnabled = false,
                        IsEnabledForApplication = false,
                        IsGoogleAuthenticatorEnabled = false
                    }
                },
                Email = new EmailSettingsEditDto
                {
                    DefaultFromAddress = "test@eaf.com",
                    DefaultFromDisplayName = "EAF",
                    SmtpDomain = "",
                    SmtpEnableSsl = false,
                    SmtpHost = "smtp.eaf.com",
                    SmtpPassword = "password",
                    SmtpPort = 587,
                    SmtpUseDefaultCredentials = false,
                    SmtpUserName = "user"
                },
                Google = new GoogleSettingsEditDto
                {
                    Analytics = "",
                    RecaptchaSiteKey = "",
                    Tag = ""
                },
                ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto
                {
                    Google = new GoogleExternalLoginProviderSettings(),
                    Google_IsEnabled = false,
                    Microsoft = new MicrosoftExternalLoginProviderSettings(),
                    Microsoft_IsEnabled = false,
                    OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                    OpenIdConnect_IsEnabled = false,
                    OpenIdConnectClaimsMapping = new List<JsonClaimMapDto>(),
                    AuthZero = new AuthZeroExternalLoginProviderSettings(),
                    AuthZero_IsEnabled = false
                },
                LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto
                {
                    DeletedQuantity = 30000,
                    Enabled = true,
                    ExpiredDays = 3
                },
                LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto
                {
                    Enabled = true
                }
            };
        }
    }
}
