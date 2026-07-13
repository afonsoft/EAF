using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests.KeyVault
{
    public class NullKeyVaultManagerTests
    {
        [Fact]
        public void NullKeyVaultManager_ShouldImplementIKeyVaultManager()
        {
            // Arrange & Act
            var type = typeof(NullKeyVaultManager);

            // Assert
            type.ShouldNotBeNull();
            typeof(IKeyVaultManager).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void NullKeyVaultManager_ShouldBeInstantiable()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();

            // Act
            var manager = new NullKeyVaultManager(logger);

            // Assert
            manager.ShouldNotBeNull();
            manager.ShouldBeOfType<NullKeyVaultManager>();
        }
    }
}
