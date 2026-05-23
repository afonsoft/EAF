using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Eaf.Middleware.Worker.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Emailing
{
    public class MiddlewareMailKitSmtpBuilderTests
    {
        [Fact]
        public void MiddlewareMailKitSmtpBuilder_ShouldBeInstantiable()
        {
            // Arrange
            var smtpEmailSenderConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            var eafMailKitConfiguration = Substitute.For<IAbpMailKitConfiguration>();

            // Act
            var builder = new MiddlewareMailKitSmtpBuilder(smtpEmailSenderConfiguration, eafMailKitConfiguration);

            // Assert
            builder.ShouldNotBeNull();
            builder.ShouldBeOfType<MiddlewareMailKitSmtpBuilder>();
        }
    }
}
