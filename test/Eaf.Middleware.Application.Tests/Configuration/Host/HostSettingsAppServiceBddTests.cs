using Abp.Configuration;
using Abp.Net.Mail;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration.Host;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Timing;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    /// <summary>
    /// Testes BDD para HostSettingsAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class HostSettingsAppServiceBddTests
    {
        private static ISettingManager CreateSettingManager()
        {
            var settingManager = Substitute.For<ISettingManager>();

            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>())
                .Returns(x => ResolveSettingValue(x.Arg<string>()));

            settingManager.GetSettingValueAsync(Arg.Any<string>())
                .Returns(x => ResolveSettingValue(x.Arg<string>()));

            settingManager.ChangeSettingForApplicationAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.CompletedTask);

            return settingManager;
        }

        private static ISettingDefinitionManager CreateSettingDefinitionManager()
        {
            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();

            settingDefinitionManager.GetSettingDefinition(Arg.Any<string>())
                .Returns(x => new SettingDefinition(x.Arg<string>(), ResolveSettingValue(x.Arg<string>())));

            return settingDefinitionManager;
        }

        private static string ResolveSettingValue(string name)
        {
            if (name.Contains("Port") ||
                name.Contains("RequiredLength") ||
                name.Contains("MaxFailedAccessAttemptsBeforeLockout") ||
                name.Contains("DefaultAccountLockoutSeconds") ||
                name.Contains("TokenExpiration") ||
                name.Contains("ExpiredDays") ||
                name.Contains("DeletedQuantity"))
            {
                return "0";
            }

            if (name.EndsWith("Enabled") ||
                name.Contains("Require") ||
                name.Contains("IsCookieConsent") ||
                name.Contains("UseCaptcha") ||
                name.Contains("AllowOneConcurrentLoginPerUser") ||
                name.Contains("StoreExternalTokenInformation") ||
                name.Contains("EnableSsl") ||
                name.Contains("UseDefaultCredentials"))
            {
                return "false";
            }

            return string.Empty;
        }

        private static HostSettingsAppService CreateSut(ISettingManager? settingManager = null)
        {
            var emailSender = Substitute.For<IEmailSender>();
            var timeZoneService = Substitute.For<ITimeZoneService>();
            timeZoneService.GetDefaultTimezoneAsync(Arg.Any<SettingScopes>(), Arg.Any<int?>())
                .Returns("UTC");

            var settingDefinitionManager = CreateSettingDefinitionManager();
            var azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            var ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            var sut = new HostSettingsAppService(emailSender, timeZoneService, settingDefinitionManager, azureConfig, ldapConfig);
            sut.SettingManager = settingManager ?? CreateSettingManager();

            return sut;
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = CreateSut();
            sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAllSettingsAnonymous

        [Fact]
        public async Task Dado_ConfiguracoesPadrao_Quando_GetAllSettingsAnonymous_Entao_DeveRetornarDtoCompleto()
        {
            // Dado
            var sut = CreateSut();

            // Quando
            var result = await sut.GetAllSettingsAnonymous();

            // Então
            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        #endregion

        #region GetAllSettings

        [Fact]
        public async Task Dado_ConfiguracoesPadrao_Quando_GetAllSettings_Entao_DeveRetornarDtoCompleto()
        {
            // Dado
            var sut = CreateSut();

            // Quando
            var result = await sut.GetAllSettings();

            // Então
            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        #endregion

        #region UpdateAllSettings

        [Fact]
        public async Task Dado_InputNulo_Quando_UpdateAllSettings_Entao_DeveRetornarSemErro()
        {
            // Dado
            var sut = CreateSut();

            // Quando
            await sut.UpdateAllSettings(null);

            // Então
            await sut.SettingManager.DidNotReceive()
                .ChangeSettingForApplicationAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_ExternalLoginProviderConfigurado_Quando_UpdateAllSettings_Entao_DeveAtualizarConfiguracoes()
        {
            // Dado
            var sut = CreateSut();
            var input = new HostSettingsEditDto
            {
                ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto
                {
                    Google = new GoogleExternalLoginProviderSettings
                    {
                        ClientId = "client-id",
                        ClientSecret = "client-secret"
                    },
                    Google_IsEnabled = true
                }
            };

            // Quando
            await sut.UpdateAllSettings(input);

            // Então
            await sut.SettingManager.Received()
                .ChangeSettingForApplicationAsync(
                    Arg.Any<string>(),
                    Arg.Is<string>(v => v == "true"));
        }

        #endregion

        #region SendTestEmail

        [Fact]
        public async Task Dado_InputValido_Quando_SendTestEmail_Entao_DeveChamarEmailSender()
        {
            // Dado
            var emailSender = Substitute.For<IEmailSender>();
            var timeZoneService = Substitute.For<ITimeZoneService>();
            var settingDefinitionManager = CreateSettingDefinitionManager();
            var azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            var ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            var sut = new HostSettingsAppService(emailSender, timeZoneService, settingDefinitionManager, azureConfig, ldapConfig)
            {
                SettingManager = CreateSettingManager()
            };

            var input = new SendTestEmailInput
            {
                EmailAddress = "test@example.com"
            };

            // Quando
            await sut.SendTestEmail(input);

            // Então
            await emailSender.Received(1).SendAsync(
                Arg.Is<string>(x => x == "test@example.com"),
                Arg.Any<string>(),
                Arg.Any<string>());
        }

        #endregion
    }
}
