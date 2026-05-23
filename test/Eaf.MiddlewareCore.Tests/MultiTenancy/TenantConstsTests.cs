using Eaf.Middleware.MultiTenancy;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy
{
    public class TenantConstsTests
    {
        [Fact]
        public void DefaultTenantName_ShouldNotBeEmpty()
        {
            // Act
            var defaultTenantName = TenantConsts.DefaultTenantName;

            // Assert
            defaultTenantName.ShouldNotBeNull();
            defaultTenantName.ShouldNotBeEmpty();
        }

        [Fact]
        public void DefaultTenantName_ShouldBeDefault()
        {
            // Act
            var defaultTenantName = TenantConsts.DefaultTenantName;

            // Assert
            defaultTenantName.ShouldBe("Default");
        }

        [Fact]
        public void MaxNameLength_ShouldBe128()
        {
            // Act
            var maxNameLength = TenantConsts.MaxNameLength;

            // Assert
            maxNameLength.ShouldBe(128);
        }

        [Fact]
        public void TenancyNameRegex_ShouldNotBeEmpty()
        {
            // Act
            var tenancyNameRegex = TenantConsts.TenancyNameRegex;

            // Assert
            tenancyNameRegex.ShouldNotBeNull();
            tenancyNameRegex.ShouldNotBeEmpty();
        }

        [Fact]
        public void TenancyNameRegex_ShouldMatchValidTenantName()
        {
            // Arrange
            var validTenantName = "MyTenant123";
            var regex = new System.Text.RegularExpressions.Regex(TenantConsts.TenancyNameRegex);

            // Act
            var isMatch = regex.IsMatch(validTenantName);

            // Assert
            isMatch.ShouldBeTrue();
        }

        [Fact]
        public void TenancyNameRegex_ShouldNotMatchInvalidTenantName()
        {
            // Arrange
            var invalidTenantName = "123Invalid";
            var regex = new System.Text.RegularExpressions.Regex(TenantConsts.TenancyNameRegex);

            // Act
            var isMatch = regex.IsMatch(invalidTenantName);

            // Assert
            isMatch.ShouldBeFalse();
        }

        [Fact]
        public void TenancyNameRegex_ShouldMatchTenantWithHyphen()
        {
            // Arrange
            var validTenantName = "My-Tenant";
            var regex = new System.Text.RegularExpressions.Regex(TenantConsts.TenancyNameRegex);

            // Act
            var isMatch = regex.IsMatch(validTenantName);

            // Assert
            isMatch.ShouldBeTrue();
        }

        [Fact]
        public void TenancyNameRegex_ShouldMatchTenantWithUnderscore()
        {
            // Arrange
            var validTenantName = "My_Tenant";
            var regex = new System.Text.RegularExpressions.Regex(TenantConsts.TenancyNameRegex);

            // Act
            var isMatch = regex.IsMatch(validTenantName);

            // Assert
            isMatch.ShouldBeTrue();
        }

        [Fact]
        public void TenantConsts_ShouldBeInstantiable()
        {
            // Arrange & Act
            var tenantConsts = new TenantConsts();

            // Assert
            tenantConsts.ShouldNotBeNull();
            tenantConsts.ShouldBeOfType<TenantConsts>();
        }
    }
}
