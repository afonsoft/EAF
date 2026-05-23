using Eaf.Middleware.AzureActiveDirectory.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.AzureActiveDirectory.Authentication
{
    public class AzureActiveDirectoryAuthenticationSourceTests
    {
        [Fact]
        public void AzureActiveDirectoryAuthenticationSource_ShouldBeAbstract()
        {
            // Arrange & Act
            var type = typeof(AzureActiveDirectoryAuthenticationSource<,>);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
        }
    }
}
