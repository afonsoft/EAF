using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class SendTwoFactorAuthCodeModelTests
    {
        [Fact]
        public void SendTwoFactorAuthCodeModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new SendTwoFactorAuthCodeModel
            {
                Provider = "Email"
            };

            // Assert
            model.Provider.ShouldBe("Email");
        }

        [Fact]
        public void SendTwoFactorAuthCodeModel_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var model = new SendTwoFactorAuthCodeModel();

            // Assert
            model.Provider.ShouldBeNull();
        }
    }
}
