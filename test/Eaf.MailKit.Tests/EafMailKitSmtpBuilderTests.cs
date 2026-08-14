using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Eaf.MailKit.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MailKit.Tests
{
    public class EafMailKitSmtpBuilderTests
    {
        private TestEafMailKitSmtpBuilder CreateBuilder(EafMailKitConfiguration configuration)
        {
            var smtpConfig = Substitute.For<ISmtpEmailSenderConfiguration>();
            smtpConfig.Host.Returns("smtp.example.com");
            smtpConfig.Port.Returns(587);
            smtpConfig.UserName.Returns("user");
            smtpConfig.Password.Returns("password");
            smtpConfig.UseDefaultCredentials.Returns(true);

            var abpConfig = Substitute.For<IAbpMailKitConfiguration>();

            return new TestEafMailKitSmtpBuilder(smtpConfig, abpConfig, configuration);
        }

        [Fact]
        public void Dado_ValidacaoDesabilitada_Quando_ConfigurarCliente_Entao_Callback_Ignora_Certificado()
        {
            var builder = CreateBuilder(new EafMailKitConfiguration { DisableCertificateValidation = true });
            var client = builder.TestConfigureClient();

            client.ServerCertificateValidationCallback.ShouldNotBeNull();
            client.ServerCertificateValidationCallback(null, null, null, System.Net.Security.SslPolicyErrors.None).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ValidacaoHabilitada_Quando_ConfigurarCliente_Entao_Callback_Eh_Nulo()
        {
            var builder = CreateBuilder(new EafMailKitConfiguration { DisableCertificateValidation = false });
            var client = builder.TestConfigureClient();

            client.ServerCertificateValidationCallback.ShouldBeNull();
        }

        private class TestEafMailKitSmtpBuilder : EafMailKitSmtpBuilder
        {
            public TestEafMailKitSmtpBuilder(
                ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
                IAbpMailKitConfiguration abpMailKitConfiguration,
                EafMailKitConfiguration configuration)
                : base(smtpEmailSenderConfiguration, abpMailKitConfiguration, configuration)
            {
            }

            public global::MailKit.Net.Smtp.SmtpClient TestConfigureClient()
            {
                var client = new FakeSmtpClient();
                ConfigureClient(client);
                return client;
            }
        }
    }
}
