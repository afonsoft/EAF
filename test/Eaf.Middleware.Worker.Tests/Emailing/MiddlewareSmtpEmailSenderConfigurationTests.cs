using Abp.Configuration;
using Eaf.Middleware.Worker.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Emailing
{
    public class MiddlewareSmtpEmailSenderConfigurationTests
    {
        [Fact]
        public void MiddlewareSmtpEmailSenderConfiguration_ShouldBeInstantiable()
        {
            // Arrange
            var settingManager = Substitute.For<ISettingManager>();

            // Act
            var config = new MiddlewareSmtpEmailSenderConfiguration(settingManager);

            // Assert
            config.ShouldNotBeNull();
            config.ShouldBeOfType<MiddlewareSmtpEmailSenderConfiguration>();
        }
    }
}
