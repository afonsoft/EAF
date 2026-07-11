using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Eaf.Middleware.Net.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Emailing
{
    public class MiddlewareSmtpEmailSenderConfigurationBddTests
    {
        [Fact]
        public void Dado_ConfiguracoesCriptografadas_Quando_AcessarPropriedadesSmtp_Entao_DeveDescriptografarValores()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var configuration = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            var host = Criptografar("smtp.example.com");
            var userName = Criptografar("user@example.com");
            var password = Criptografar("secret");
            var domain = Criptografar("example.com");

            settingManager.GetSettingValue(EmailSettingNames.Smtp.Host).Returns(host);
            settingManager.GetSettingValue(EmailSettingNames.Smtp.UserName).Returns(userName);
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Password).Returns(password);
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Domain).Returns(domain);

            configuration.Host.ShouldBe("smtp.example.com");
            configuration.UserName.ShouldBe("user@example.com");
            configuration.Password.ShouldBe("secret");
            configuration.Domain.ShouldBe("example.com");
        }

        [Fact]
        public void Dado_Configuracao_Quando_AcessarPort_Entao_DeveRetornarValorInteiro()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var configuration = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            settingManager.GetSettingValue(EmailSettingNames.Smtp.Port).Returns("587");

            configuration.Port.ShouldBe(587);
        }

        [Fact]
        public void Dado_Configuracao_Quando_AcessarEnableSsl_Entao_DeveRetornarValorBooleano()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var configuration = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            settingManager.GetSettingValue(EmailSettingNames.Smtp.EnableSsl).Returns("true");

            configuration.EnableSsl.ShouldBeTrue();
        }

        private static string Criptografar(string value)
        {
            return SimpleStringCipher.Instance.Encrypt(value);
        }
    }
}
