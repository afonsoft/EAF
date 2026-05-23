using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests
{
    public class AzureActiveDirectorySettingNamesTests
    {
        [Fact]
        public void AzureActiveDirectorySettingNames_ShouldHaveCorrectValues()
        {
            // Assert
            AzureActiveDirectorySettingNames.IsEnabled.ShouldBe("Eaf.Middleware.AzureActiveDirectory.IsEnabled");
            AzureActiveDirectorySettingNames.ClientId.ShouldBe("Eaf.Middleware.AzureActiveDirectory.ClientId");
            AzureActiveDirectorySettingNames.ClientSecret.ShouldBe("Eaf.Middleware.AzureActiveDirectory.ClientSecret");
            AzureActiveDirectorySettingNames.Tenant.ShouldBe("Eaf.Middleware.AzureActiveDirectory.Tenant");
            AzureActiveDirectorySettingNames.ActiveDirectoryProvider.ShouldBe("ActiveDirectory");
        }

        [Fact]
        public void AzureActiveDirectorySettingNames_ShouldBeConstant()
        {
            // Arrange & Act
            var isEnabled1 = AzureActiveDirectorySettingNames.IsEnabled;
            var isEnabled2 = AzureActiveDirectorySettingNames.IsEnabled;
            var clientId1 = AzureActiveDirectorySettingNames.ClientId;
            var clientId2 = AzureActiveDirectorySettingNames.ClientId;

            // Assert
            isEnabled1.ShouldBe(isEnabled2);
            clientId1.ShouldBe(clientId2);
        }

        [Fact]
        public void AzureActiveDirectorySettingNames_ShouldNotBeNullOrEmpty()
        {
            // Assert
            AzureActiveDirectorySettingNames.IsEnabled.ShouldNotBeNullOrEmpty();
            AzureActiveDirectorySettingNames.ClientId.ShouldNotBeNullOrEmpty();
            AzureActiveDirectorySettingNames.ClientSecret.ShouldNotBeNullOrEmpty();
            AzureActiveDirectorySettingNames.Tenant.ShouldNotBeNullOrEmpty();
            AzureActiveDirectorySettingNames.ActiveDirectoryProvider.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void AzureActiveDirectorySettingNames_ShouldFollowNamingConvention()
        {
            // Assert - All setting names should start with "Eaf.Middleware.AzureActiveDirectory." except ActiveDirectoryProvider
            AzureActiveDirectorySettingNames.IsEnabled.ShouldStartWith("Eaf.Middleware.AzureActiveDirectory.");
            AzureActiveDirectorySettingNames.ClientId.ShouldStartWith("Eaf.Middleware.AzureActiveDirectory.");
            AzureActiveDirectorySettingNames.ClientSecret.ShouldStartWith("Eaf.Middleware.AzureActiveDirectory.");
            AzureActiveDirectorySettingNames.Tenant.ShouldStartWith("Eaf.Middleware.AzureActiveDirectory.");
        }

        [Fact]
        public void ActiveDirectoryProvider_ShouldHaveCorrectValue()
        {
            // Assert
            AzureActiveDirectorySettingNames.ActiveDirectoryProvider.ShouldBe("ActiveDirectory");
        }
    }
}