using Abp.Configuration;
using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Eaf.Middleware.Net.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net
{
    /// <summary>
    /// Testes BDD para as classes de configuração de e-mail do middleware seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EmailingConfigurationBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarEmailTemplateProvider_Entao_DeveImplementarIEmailTemplateProvider()
        {
            var provider = new EmailTemplateProvider();

            provider.ShouldBeAssignableTo<IEmailTemplateProvider>();
        }

        [Fact]
        public void Dado_SettingManager_Quando_CriarMiddlewareSmtpEmailSenderConfiguration_Entao_DeveHerdarSmtpEmailSenderConfiguration()
        {
            var settingManager = Substitute.For<ISettingManager>();

            var config = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            config.ShouldBeAssignableTo<SmtpEmailSenderConfiguration>();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarMiddlewareMailKitSmtpBuilder_Entao_DeveHerdarDefaultMailKitSmtpBuilder()
        {
            var smtpConfig = Substitute.For<ISmtpEmailSenderConfiguration>();
            var mailKitConfig = Substitute.For<IAbpMailKitConfiguration>();

            var builder = new MiddlewareMailKitSmtpBuilder(smtpConfig, mailKitConfig);

            builder.ShouldBeAssignableTo<DefaultMailKitSmtpBuilder>();
        }
    }
}
