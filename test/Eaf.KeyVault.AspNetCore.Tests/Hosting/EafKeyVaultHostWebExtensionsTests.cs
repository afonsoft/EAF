using Microsoft.AspNetCore.Hosting;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.AspNetCore.Tests.Hosting
{
    public class EafKeyVaultHostWebExtensionsTests
    {
        [Fact]
        public void EafKeyVaultHostWebExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(EafKeyVaultHostWebExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void EafKeyVaultHostWebExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(EafKeyVaultHostWebExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
