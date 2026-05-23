using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ExternalLoginProviderInfoModelTests
    {
        [Fact]
        public void ExternalLoginProviderInfoModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new ExternalLoginProviderInfoModel
            {
                Name = "Google",
                ClientId = "google_client_id",
                TenantId = "tenant_123",
                AdditionalParams = new Dictionary<string, string>
                {
                    { "scope", "email profile" },
                    { "redirect_uri", "https://example.com/callback" }
                }
            };

            // Assert
            model.Name.ShouldBe("Google");
            model.ClientId.ShouldBe("google_client_id");
            model.TenantId.ShouldBe("tenant_123");
            model.AdditionalParams.ShouldContainKey("scope");
            model.AdditionalParams["scope"].ShouldBe("email profile");
            model.AdditionalParams.ShouldContainKey("redirect_uri");
        }

        [Fact]
        public void ExternalLoginProviderInfoModel_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var model = new ExternalLoginProviderInfoModel();

            // Assert
            model.Name.ShouldBeNull();
            model.ClientId.ShouldBeNull();
            model.TenantId.ShouldBeNull();
            model.AdditionalParams.ShouldBeNull();
        }

        [Fact]
        public void ExternalLoginProviderInfoModel_CanHaveEmptyAdditionalParams()
        {
            // Arrange & Act
            var model = new ExternalLoginProviderInfoModel
            {
                Name = "Microsoft",
                AdditionalParams = new Dictionary<string, string>()
            };

            // Assert
            model.Name.ShouldBe("Microsoft");
            model.AdditionalParams.ShouldBeEmpty();
        }
    }
}
