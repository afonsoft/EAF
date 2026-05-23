using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests.KeyVault.OCI
{
    public class OCIKeyVaultManagerTests
    {
        [Fact]
        public void OCIKeyVaultManager_ShouldImplementIKeyVaultManager()
        {
            // Arrange & Act
            var type = typeof(OCIKeyVaultManager);

            // Assert
            type.ShouldNotBeNull();
            typeof(IKeyVaultManager).IsAssignableFrom(type).ShouldBeTrue();
        }
    }
}
