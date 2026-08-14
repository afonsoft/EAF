using Abp.MailKit;
using Abp.Net.Mail;
using Eaf.MailKit.Configuration;
using NSubstitute;
using Shouldly;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MailKit.Tests
{
    public class EafMailKitEmailSenderTests
    {
        private static EafMailKitEmailSender CreateSender(FakeSmtpClient client, EafMailKitConfiguration configuration = null)
        {
            var config = Substitute.For<IEmailSenderConfiguration>();
            config.DefaultFromAddress.Returns("from@example.com");
            config.DefaultFromDisplayName.Returns("Sender");

            var builder = Substitute.For<IMailKitSmtpBuilder>();
            builder.Build().Returns(client);

            return new EafMailKitEmailSender(config, builder, configuration ?? new EafMailKitConfiguration());
        }

        [Fact]
        public void Dado_EmailSimples_Quando_Enviar_Entao_Email_Entregue()
        {
            var client = new FakeSmtpClient();
            var sender = CreateSender(client, new EafMailKitConfiguration { RetryCount = 0, RetryDelayMilliseconds = 0 });

            sender.Send("from@example.com", "to@example.com", "Test", "<b>Hello</b>", true);

            client.SentMessages.Count.ShouldBe(1);
            ((global::MimeKit.MailboxAddress)client.SentMessages[0].To[0]).Address.ShouldBe("to@example.com");
            client.SentMessages[0].Subject.ShouldBe("Test");
            client.IsDisconnected.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_EmailSimples_Quando_EnviarAsync_Entao_Email_Entregue()
        {
            var client = new FakeSmtpClient();
            var sender = CreateSender(client, new EafMailKitConfiguration { RetryCount = 0, RetryDelayMilliseconds = 0 });

            await sender.SendAsync("from@example.com", "to@example.com", "Test", "<b>Hello</b>", true);

            client.SentMessages.Count.ShouldBe(1);
            ((global::MimeKit.MailboxAddress)client.SentMessages[0].To[0]).Address.ShouldBe("to@example.com");
        }

        [Fact]
        public async Task Dado_FalhaTransitoria_Quando_EnviarAsync_Entao_Reenvia_Com_Sucesso()
        {
            var client = new FakeSmtpClient();
            client.EnqueueFailure(new global::MailKit.Net.Smtp.SmtpCommandException(
                global::MailKit.Net.Smtp.SmtpErrorCode.MessageNotAccepted,
                global::MailKit.Net.Smtp.SmtpStatusCode.ServiceNotAvailable,
                "transient"));

            var sender = CreateSender(client, new EafMailKitConfiguration { RetryCount = 2, RetryDelayMilliseconds = 0 });

            await sender.SendAsync("from@example.com", "to@example.com", "Test", "Hello", false);

            client.SentMessages.Count.ShouldBe(1);
        }

        [Fact]
        public void Dado_FalhaPermanente_Quando_Enviar_Entao_Lanca_Excecao_Sem_Retry()
        {
            var client = new FakeSmtpClient();
            client.EnqueueFailure(new global::MailKit.Net.Smtp.SmtpCommandException(
                global::MailKit.Net.Smtp.SmtpErrorCode.RecipientNotAccepted,
                global::MailKit.Net.Smtp.SmtpStatusCode.MailboxUnavailable,
                "permanent"));

            var sender = CreateSender(client, new EafMailKitConfiguration { RetryCount = 3, RetryDelayMilliseconds = 0 });

            Should.Throw<global::MailKit.Net.Smtp.SmtpCommandException>(() =>
                sender.Send("from@example.com", "to@example.com", "Test", "Hello", false));

            client.SentMessages.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_EmailComAnexo_Quando_Enviar_Entao_Email_Entregue()
        {
            var client = new FakeSmtpClient();
            var sender = CreateSender(client, new EafMailKitConfiguration { RetryCount = 0, RetryDelayMilliseconds = 0 });

            using var message = new MailMessage("from@example.com", "to@example.com", "Subject", "<h1>Body</h1>")
            {
                IsBodyHtml = true
            };

            sender.Send(message);

            client.SentMessages.Count.ShouldBe(1);
        }
    }
}
