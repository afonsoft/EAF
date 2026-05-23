using Abp.Configuration;
using Abp.Dependency;
using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.AzureActiveDirectory.Authentication;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Middleware
{
    public class AzureActiveDirectory_Tests : EafMiddlewareTestBase
    {
        [Fact]
        public void Should_Enable_AzureActiveDirectory_Authentication_When_Enabled_In_Settings()
        {
            // Arrange
            var settingManager = Resolve<ISettingManager>();
            settingManager.ChangeSettingForApplication(AzureActiveDirectorySettingNames.IsEnabled, "true");

            // Act
            var authenticationSource = settingManager.GetSettingValue<bool>(AzureActiveDirectorySettingNames.IsEnabled);

            // Assert
            authenticationSource.ShouldBeTrue();
        }

        [Fact]
        public void Should_Not_Enable_AzureActiveDirectory_Authentication_When_Disabled_In_Settings()
        {
            // Arrange
            var settingManager = Resolve<ISettingManager>();
            settingManager.ChangeSettingForApplication(AzureActiveDirectorySettingNames.IsEnabled, "false");

            // Act
            var authenticationSource = settingManager.GetSettingValue<bool>(AzureActiveDirectorySettingNames.IsEnabled);

            // Assert
            authenticationSource.ShouldBeFalse();
        }
    }
}