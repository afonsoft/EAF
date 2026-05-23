using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class AuthenticateResultModelTests
    {
        [Fact]
        public void AuthenticateResultModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new AuthenticateResultModel
            {
                AccessToken = "access_token_123",
                EncryptedAccessToken = "encrypted_token_456",
                ExpireInSeconds = 3600,
                PasswordResetCode = "reset_code",
                RequiresTwoFactorVerification = true,
                ReturnUrl = "/dashboard",
                ShouldResetPassword = false,
                TwoFactorAuthProviders = new List<string> { "Email", "Phone" },
                TwoFactorRememberClientToken = "remember_token",
                UserId = 123
            };

            // Assert
            model.AccessToken.ShouldBe("access_token_123");
            model.EncryptedAccessToken.ShouldBe("encrypted_token_456");
            model.ExpireInSeconds.ShouldBe(3600);
            model.PasswordResetCode.ShouldBe("reset_code");
            model.RequiresTwoFactorVerification.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/dashboard");
            model.ShouldResetPassword.ShouldBeFalse();
            model.TwoFactorAuthProviders.ShouldContain("Email");
            model.TwoFactorAuthProviders.ShouldContain("Phone");
            model.TwoFactorRememberClientToken.ShouldBe("remember_token");
            model.UserId.ShouldBe(123);
        }

        [Fact]
        public void AuthenticateResultModel_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var model = new AuthenticateResultModel();

            // Assert
            model.AccessToken.ShouldBeNull();
            model.EncryptedAccessToken.ShouldBeNull();
            model.ExpireInSeconds.ShouldBe(0);
            model.PasswordResetCode.ShouldBeNull();
            model.RequiresTwoFactorVerification.ShouldBeFalse();
            model.ReturnUrl.ShouldBeNull();
            model.ShouldResetPassword.ShouldBeFalse();
            model.TwoFactorAuthProviders.ShouldBeNull();
            model.TwoFactorRememberClientToken.ShouldBeNull();
            model.UserId.ShouldBe(0);
        }

        [Fact]
        public void AuthenticateResultModel_TwoFactorAuthProviders_ShouldBeNullByDefault()
        {
            // Arrange & Act
            var model = new AuthenticateResultModel();

            // Assert
            model.TwoFactorAuthProviders.ShouldBeNull();
        }

        [Fact]
        public void AuthenticateResultModel_UserId_ShouldAcceptLongValues()
        {
            // Arrange & Act
            var model = new AuthenticateResultModel
            {
                UserId = long.MaxValue
            };

            // Assert
            model.UserId.ShouldBe(long.MaxValue);
        }

        [Fact]
        public void AuthenticateResultModel_ExpireInSeconds_ShouldAcceptIntValues()
        {
            // Arrange & Act
            var model = new AuthenticateResultModel
            {
                ExpireInSeconds = 7200
            };

            // Assert
            model.ExpireInSeconds.ShouldBe(7200);
        }
    }
}
