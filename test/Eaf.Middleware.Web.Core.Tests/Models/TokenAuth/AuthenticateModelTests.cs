using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class AuthenticateModelTests
    {
        [Fact]
        public void AuthenticateModel_ShouldHaveRequiredProperties()
        {
            // Arrange & Act
            var model = new AuthenticateModel
            {
                UserNameOrEmailAddress = "test@example.com",
                Password = "password123",
                RememberClient = true,
                ReturnUrl = "/dashboard",
                SingleSignIn = true,
                TwoFactorVerificationCode = "123456",
                TwoFactorRememberClientToken = "token123",
                CaptchaResponse = "captcha_token"
            };

            // Assert
            model.UserNameOrEmailAddress.ShouldBe("test@example.com");
            model.Password.ShouldBe("password123");
            model.RememberClient.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/dashboard");
            model.SingleSignIn.ShouldBe(true);
            model.TwoFactorVerificationCode.ShouldBe("123456");
            model.TwoFactorRememberClientToken.ShouldBe("token123");
            model.CaptchaResponse.ShouldBe("captcha_token");
        }

        [Fact]
        public void AuthenticateModel_UserNameOrEmailAddress_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("UserNameOrEmailAddress");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_UserNameOrEmailAddress_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("UserNameOrEmailAddress");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(MaxLengthAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_Password_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("Password");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(RequiredAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_Password_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("Password");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(MaxLengthAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_Password_ShouldHaveDisableAuditingAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("Password");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(Abp.Auditing.DisableAuditingAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_CaptchaResponse_ShouldHaveDisableAuditingAttribute()
        {
            // Arrange
            var model = new AuthenticateModel();
            var property = model.GetType().GetProperty("CaptchaResponse");
            property.ShouldNotBeNull();

            // Act
            var attributes = property.GetCustomAttributes(typeof(Abp.Auditing.DisableAuditingAttribute), false);

            // Assert
            attributes.ShouldNotBeEmpty();
        }

        [Fact]
        public void AuthenticateModel_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var model = new AuthenticateModel();

            // Assert
            model.UserNameOrEmailAddress.ShouldBeNull();
            model.Password.ShouldBeNull();
            model.RememberClient.ShouldBeFalse();
            model.ReturnUrl.ShouldBeNull();
            model.SingleSignIn.ShouldBeNull();
            model.TwoFactorVerificationCode.ShouldBeNull();
            model.TwoFactorRememberClientToken.ShouldBeNull();
            model.CaptchaResponse.ShouldBeNull();
        }
    }
}
