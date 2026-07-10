using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Eaf.Middleware.Worker.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Emailing
{
    public class MiddlewareSmtpEmailSenderConfigurationBddTests
    {
        [Fact]
        public void Dado_SettingsCriptografados_Quando_AcessarHost_Entao_DeveRetornarValorDescriptografado()
        {
            // Dado
            var host = "smtp.eaf.test";
            var encryptedHost = SimpleStringCipher.Instance.Encrypt(host);
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Host).Returns(encryptedHost);

            var sut = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Quando
            var result = sut.Host;

            // Então
            result.ShouldBe(host);
        }

        [Fact]
        public void Dado_SettingsCriptografados_Quando_AcessarUserName_Entao_DeveRetornarValorDescriptografado()
        {
            // Dado
            var userName = "user@eaf.test";
            var encryptedUserName = SimpleStringCipher.Instance.Encrypt(userName);
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(EmailSettingNames.Smtp.UserName).Returns(encryptedUserName);

            var sut = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Quando
            var result = sut.UserName;

            // Então
            result.ShouldBe(userName);
        }

        [Fact]
        public void Dado_SettingsCriptografados_Quando_AcessarPassword_Entao_DeveRetornarValorDescriptografado()
        {
            // Dado
            var password = "P@ssw0rd";
            var encryptedPassword = SimpleStringCipher.Instance.Encrypt(password);
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Password).Returns(encryptedPassword);

            var sut = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Quando
            var result = sut.Password;

            // Então
            result.ShouldBe(password);
        }

        [Fact]
        public void Dado_SettingsCriptografados_Quando_AcessarDomain_Entao_DeveRetornarValorDescriptografado()
        {
            // Dado
            var domain = "eaf.local";
            var encryptedDomain = SimpleStringCipher.Instance.Encrypt(domain);
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Domain).Returns(encryptedDomain);

            var sut = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Quando
            var result = sut.Domain;

            // Então
            result.ShouldBe(domain);
        }

        [Fact]
        public void Dado_SettingManagerComValorVazio_Quando_AcessarHost_Entao_DeveLancarExcecao()
        {
            // Dado
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(EmailSettingNames.Smtp.Host).Returns("");

            var sut = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Quando & Então
            Should.Throw<Abp.AbpException>(() => sut.Host.ToString());
        }
    }
}
