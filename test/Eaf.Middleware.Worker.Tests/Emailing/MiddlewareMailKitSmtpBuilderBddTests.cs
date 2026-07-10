using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Eaf.Middleware.Worker.Emailing;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using NSubstitute;
using Shouldly;
using System;
using System.Net;
using System.Text;
using System.Threading;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Emailing
{
    public class MiddlewareMailKitSmtpBuilderBddTests
    {
        private readonly ISmtpEmailSenderConfiguration _smtpConfiguration;
        private readonly IAbpMailKitConfiguration _mailKitConfiguration;

        public MiddlewareMailKitSmtpBuilderBddTests()
        {
            _smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            _smtpConfiguration.Host.Returns("localhost");
            _smtpConfiguration.Port.Returns(25);
            _smtpConfiguration.UserName.Returns("user");
            _smtpConfiguration.Password.Returns("pass");
            _smtpConfiguration.UseDefaultCredentials.Returns(false);
            _smtpConfiguration.EnableSsl.Returns(false);

            _mailKitConfiguration = Substitute.For<IAbpMailKitConfiguration>();
            _mailKitConfiguration.SecureSocketOption.Returns((SecureSocketOptions?)null);
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarBuilder_Entao_DeveInicializarCorretamente()
        {
            // Dado & Quando
            var sut = new MiddlewareMailKitSmtpBuilder(_smtpConfiguration, _mailKitConfiguration);

            // Então
            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<DefaultMailKitSmtpBuilder>();
        }

        [Fact]
        public void Dado_Configuracao_Quando_ConfigureClient_Entao_DeveDefinirCallbackDeValidacao()
        {
            // Dado
            var sut = new TestableMiddlewareMailKitSmtpBuilder(_smtpConfiguration, _mailKitConfiguration);
            var client = new TestableSmtpClient();

            // Quando
            sut.ConfigureClientPublic(client);

            // Então
            client.ServerCertificateValidationCallback.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ConfiguracaoComUseDefaultCredentialsTrue_Quando_ConfigureClient_Entao_NaoDeveAutenticar()
        {
            // Dado
            _smtpConfiguration.UseDefaultCredentials.Returns(true);
            var sut = new TestableMiddlewareMailKitSmtpBuilder(_smtpConfiguration, _mailKitConfiguration);
            var client = new TestableSmtpClient();

            // Quando
            sut.ConfigureClientPublic(client);

            // Então
            client.AuthenticateCalled.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ConfiguracaoComUseDefaultCredentialsFalse_Quando_ConfigureClient_Entao_DeveAutenticar()
        {
            // Dado
            var sut = new TestableMiddlewareMailKitSmtpBuilder(_smtpConfiguration, _mailKitConfiguration);
            var client = new TestableSmtpClient();

            // Quando
            sut.ConfigureClientPublic(client);

            // Então
            client.AuthenticateCalled.ShouldBeTrue();
            var credential = client.AuthenticateCredentials as NetworkCredential;
            credential.ShouldNotBeNull();
            credential.UserName.ShouldBe("user");
            credential.Password.ShouldBe("pass");
        }

        private sealed class TestableMiddlewareMailKitSmtpBuilder : MiddlewareMailKitSmtpBuilder
        {
            public TestableMiddlewareMailKitSmtpBuilder(
                ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
                IAbpMailKitConfiguration eafMailKitConfiguration)
                : base(smtpEmailSenderConfiguration, eafMailKitConfiguration)
            {
            }

            public void ConfigureClientPublic(SmtpClient client) => ConfigureClient(client);
        }

        private sealed class TestableSmtpClient : SmtpClient
        {
            public bool AuthenticateCalled { get; private set; }
            public ICredentials? AuthenticateCredentials { get; private set; }

            public override void Authenticate(Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default)
            {
                AuthenticateCalled = true;
                AuthenticateCredentials = credentials;
            }

            public override void Connect(string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
            {
                // no-op
            }
        }
    }
}
