using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ExternalAuthenticateResultModelTests
    {
        [Fact]
        public void ExternalAuthenticateResultModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new ExternalAuthenticateResultModel
            {
                AccessToken = "token_123",
                EncryptedAccessToken = "encrypted_token",
                ExpireInSeconds = 3600,
                ReturnUrl = "/dashboard",
                WaitingForActivation = true,
                UserId = 123
            };

            // Assert
            model.AccessToken.ShouldBe("token_123");
            model.EncryptedAccessToken.ShouldBe("encrypted_token");
            model.ExpireInSeconds.ShouldBe(3600);
            model.ReturnUrl.ShouldBe("/dashboard");
            model.WaitingForActivation.ShouldBeTrue();
            model.UserId.ShouldBe(123);
        }

        [Fact]
        public void ExternalAuthenticateResultModel_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var model = new ExternalAuthenticateResultModel();

            // Assert
            model.AccessToken.ShouldBeNull();
            model.EncryptedAccessToken.ShouldBeNull();
            model.ExpireInSeconds.ShouldBe(0);
            model.ReturnUrl.ShouldBeNull();
            model.WaitingForActivation.ShouldBeFalse();
            model.UserId.ShouldBe(0);
        }
    }
}
