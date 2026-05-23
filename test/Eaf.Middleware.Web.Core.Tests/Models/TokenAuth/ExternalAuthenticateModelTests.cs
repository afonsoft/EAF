using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ExternalAuthenticateModelTests
    {
        [Fact]
        public void ExternalAuthenticateModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Google",
                ProviderAccessCode = "access_code_123",
                ProviderKey = "provider_key_456",
                ReturnUrl = "/home"
            };

            // Assert
            model.AuthProvider.ShouldBe("Google");
            model.ProviderAccessCode.ShouldBe("access_code_123");
            model.ProviderKey.ShouldBe("provider_key_456");
            model.ReturnUrl.ShouldBe("/home");
        }

        [Fact]
        public void ExternalAuthenticateModel_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var model = new ExternalAuthenticateModel();

            // Assert
            model.AuthProvider.ShouldBeNull();
            model.ProviderAccessCode.ShouldBeNull();
            model.ProviderKey.ShouldBeNull();
            model.ReturnUrl.ShouldBeNull();
        }
    }
}
