using Abp.Configuration;
using Abp.Net.Mail;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration.Host;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Timing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class HostSettingsAppServiceBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var emailSender = Substitute.For<IEmailSender>();
            var timeZoneService = Substitute.For<ITimeZoneService>();
            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            var ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            var sut = new HostSettingsAppService(emailSender, timeZoneService, settingDefinitionManager, azureConfig, ldapConfig);
            sut.ShouldNotBeNull();
        }
    }
}
