using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ProviderModelTests
    {
        [Fact]
        public void ProviderModel_ShouldSetAllProperties()
        {
            // Arrange & Act
            var model = new ProviderModel
            {
                UsernameOrEmailAddress = "user@example.com",
                AuthenticationSource = "Google",
                Tenant = new TenantModal
                {
                    Name = "Tenant Name",
                    TenancyName = "tenant",
                    Id = 123
                }
            };

            // Assert
            model.UsernameOrEmailAddress.ShouldBe("user@example.com");
            model.AuthenticationSource.ShouldBe("Google");
            model.Tenant.ShouldNotBeNull();
            model.Tenant.Name.ShouldBe("Tenant Name");
            model.Tenant.TenancyName.ShouldBe("tenant");
            model.Tenant.Id.ShouldBe(123);
        }

        [Fact]
        public void ProviderModel_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var model = new ProviderModel();

            // Assert
            model.UsernameOrEmailAddress.ShouldBeNull();
            model.AuthenticationSource.ShouldBeNull();
            model.Tenant.ShouldBeNull();
        }

        [Fact]
        public void TenantModal_ShouldSetAllProperties()
        {
            // Arrange & Act
            var tenant = new TenantModal
            {
                Name = "Test Tenant",
                TenancyName = "test",
                Id = 456
            };

            // Assert
            tenant.Name.ShouldBe("Test Tenant");
            tenant.TenancyName.ShouldBe("test");
            tenant.Id.ShouldBe(456);
        }
    }
}
