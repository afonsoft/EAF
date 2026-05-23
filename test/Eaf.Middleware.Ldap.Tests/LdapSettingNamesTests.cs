using Eaf.Middleware.Ldap.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests
{
    public class LdapSettingNamesTests
    {
        [Fact]
        public void LdapSettingNames_ShouldHaveCorrectValues()
        {
            // Assert
            LdapSettingNames.IsEnabled.ShouldBe("Eaf.Middleware.Ldap.IsEnabled");
            LdapSettingNames.Domain.ShouldBe("Eaf.Middleware.Ldap.Domain");
            LdapSettingNames.UserName.ShouldBe("Eaf.Middleware.Ldap.UserName");
            LdapSettingNames.Password.ShouldBe("Eaf.Middleware.Ldap.Password");
            LdapSettingNames.Container.ShouldBe("Eaf.Middleware.Ldap.Container");
            LdapSettingNames.ContextType.ShouldBe("Eaf.Middleware.Ldap.ContextType");
            LdapSettingNames.LdapProvider.ShouldBe("LDAP");
        }

        [Fact]
        public void LdapSettingNames_ShouldBeConstant()
        {
            // Arrange & Act
            var isEnabled1 = LdapSettingNames.IsEnabled;
            var isEnabled2 = LdapSettingNames.IsEnabled;
            var domain1 = LdapSettingNames.Domain;
            var domain2 = LdapSettingNames.Domain;

            // Assert
            isEnabled1.ShouldBe(isEnabled2);
            domain1.ShouldBe(domain2);
        }

        [Fact]
        public void LdapSettingNames_ShouldNotBeNullOrEmpty()
        {
            // Assert
            LdapSettingNames.IsEnabled.ShouldNotBeNullOrEmpty();
            LdapSettingNames.Domain.ShouldNotBeNullOrEmpty();
            LdapSettingNames.UserName.ShouldNotBeNullOrEmpty();
            LdapSettingNames.Password.ShouldNotBeNullOrEmpty();
            LdapSettingNames.Container.ShouldNotBeNullOrEmpty();
            LdapSettingNames.ContextType.ShouldNotBeNullOrEmpty();
            LdapSettingNames.LdapProvider.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void LdapSettingNames_ShouldFollowNamingConvention()
        {
            // Assert - Most setting names should start with "Eaf.Middleware.Ldap."
            LdapSettingNames.IsEnabled.ShouldStartWith("Eaf.Middleware.Ldap.");
            LdapSettingNames.Domain.ShouldStartWith("Eaf.Middleware.Ldap.");
            LdapSettingNames.UserName.ShouldStartWith("Eaf.Middleware.Ldap.");
            LdapSettingNames.Password.ShouldStartWith("Eaf.Middleware.Ldap.");
            LdapSettingNames.Container.ShouldStartWith("Eaf.Middleware.Ldap.");
            LdapSettingNames.ContextType.ShouldStartWith("Eaf.Middleware.Ldap.");
        }

        [Fact]
        public void LdapSettingNames_LdapProvider_ShouldBeSpecialValue()
        {
            // Assert - LdapProvider is a special constant
            LdapSettingNames.LdapProvider.ShouldBe("LDAP");
        }
    }
}