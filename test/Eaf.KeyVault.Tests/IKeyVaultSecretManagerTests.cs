using Eaf.KeyVault;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class IKeyVaultSecretManagerTests
    {
        [Fact]
        public void IKeyVaultSecretManager_ShouldBeInterface()
        {
            // Arrange & Act
            var type = typeof(IKeyVaultSecretManager);

            // Assert
            type.ShouldNotBeNull();
            type.IsInterface.ShouldBeTrue();
        }

        [Fact]
        public void IKeyVaultSecretManager_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(IKeyVaultSecretManager);
            var methods = type.GetMethods();

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
