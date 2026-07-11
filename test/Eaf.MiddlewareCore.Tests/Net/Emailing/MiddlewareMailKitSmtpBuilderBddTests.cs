using Abp.Net.Mail.Smtp;
using Abp.MailKit;
using Eaf.Middleware.Net.Emailing;
using MailKit.Net.Smtp;
using MailKit.Security;
using NSubstitute;
using Shouldly;
using System;
using System.Net;
using System.Text;
using System.Threading;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Emailing
{
    public class MiddlewareMailKitSmtpBuilderBddTests
    {
        [Fact]
        public void Dado_SmtpClient_Quando_Configurar_Entao_DeveIgnorarValidacaoDeCertificado()
        {
            var smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            smtpConfiguration.Host.Returns("smtp.example.com");
            smtpConfiguration.Port.Returns(587);
            smtpConfiguration.EnableSsl.Returns(false);
            smtpConfiguration.UseDefaultCredentials.Returns(false);
            smtpConfiguration.UserName.Returns("user");
            smtpConfiguration.Password.Returns("pass");

            var mailKitConfiguration = Substitute.For<IAbpMailKitConfiguration>();
            var builder = new TestableMiddlewareMailKitSmtpBuilder(smtpConfiguration, mailKitConfiguration);
            var client = new TestSmtpClient();

            builder.ConfigureForTest(client);

            client.ServerCertificateValidationCallback.ShouldNotBeNull();
            client.ServerCertificateValidationCallback(null, null, null, System.Net.Security.SslPolicyErrors.None).ShouldBeTrue();
        }

        [Fact]
        public void Dado_MiddlewareMailKitSmtpBuilder_Quando_CriarInstancia_Entao_DeveEstarValido()
        {
            var smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            var mailKitConfiguration = Substitute.For<IAbpMailKitConfiguration>();

            var builder = new MiddlewareMailKitSmtpBuilder(smtpConfiguration, mailKitConfiguration);

            builder.ShouldNotBeNull();
        }

        private class TestableMiddlewareMailKitSmtpBuilder : MiddlewareMailKitSmtpBuilder
        {
            public TestableMiddlewareMailKitSmtpBuilder(
                ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
                IAbpMailKitConfiguration eafMailKitConfiguration)
                : base(smtpEmailSenderConfiguration, eafMailKitConfiguration)
            {
            }

            public void ConfigureForTest(SmtpClient client) => ConfigureClient(client);
        }

        private class TestSmtpClient : SmtpClient
        {
            public override void Connect(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken)
            {
            }

            public override void Authenticate(Encoding encoding, ICredentials credentials, CancellationToken cancellationToken)
            {
            }
        }
    }
}
