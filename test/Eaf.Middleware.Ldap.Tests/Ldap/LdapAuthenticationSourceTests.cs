using Eaf.Middleware.Ldap.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests.Ldap.Authentication
{
    public class LdapAuthenticationSourceTests
    {
        [Fact]
        public void LdapAuthenticationSource_ShouldBeAbstract()
        {
            // Arrange & Act
            var type = typeof(LdapAuthenticationSource<,>);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
        }
    }
}
