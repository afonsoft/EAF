using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class NullKeyVaultManagerTests
    {
        private readonly ILogger _logger;

        public NullKeyVaultManagerTests()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public void GetKeyValues_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act
            var result = manager.GetKeyValues();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Dictionary<string, string>>();
            result.ShouldBeEmpty();
            _logger.Received().Debug("NullKeyVaultManager : NotImplementedException");
        }

        [Fact]
        public async Task GetKeyValuesAsync_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act
            var result = await manager.GetKeyValuesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Dictionary<string, string>>();
            result.ShouldBeEmpty();
            _logger.Received().Debug("NullKeyVaultManager : NotImplementedException");
        }

        [Fact]
        public void GetValue_ShouldReturnNull()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);
            var key = "test-key";

            // Act
            var result = manager.GetValue(key);

            // Assert
            result.ShouldBeNull();
            _logger.Received().Debug("NullKeyVaultManager : NotImplementedException");
        }

        [Fact]
        public async Task GetValueAsync_ShouldReturnNull()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);
            var key = "test-key";

            // Act
            var result = await manager.GetValueAsync(key);

            // Assert
            result.ShouldBeNull();
            _logger.Received().Debug("NullKeyVaultManager : NotImplementedException");
        }

        [Fact]
        public void SetValue_ShouldNotThrow()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);
            var key = "test-key";
            var value = "test-value";

            // Act & Assert
            Should.NotThrow(() => manager.SetValue(key, value));
            _logger.Received().Debug("NullKeyVaultManager : NotImplementedException");
        }

        [Fact]
        public async Task SetValueAsync_ShouldNotThrow()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);
            var key = "test-key";
            var value = "test-value";

            // Act & Assert
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(key, value));
            _logger.Received(2).Debug("NullKeyVaultManager : NotImplementedException"); // Called twice: once in SetValueAsync, once in SetValue
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("simple-key")]
        [InlineData("key-with-special-chars!@#$%")]
        [InlineData("very-long-key-name-with-many-characters-to-test-edge-cases")]
        public void GetValue_WithDifferentKeys_ShouldAlwaysReturnNull(string? key)
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act
            var result = manager.GetValue(key);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("simple-value")]
        [InlineData("value-with-special-chars!@#$%")]
        [InlineData("very-long-value-with-many-characters-to-test-edge-cases")]
        public void SetValue_WithDifferentValues_ShouldNotThrow(string? value)
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act & Assert
            Should.NotThrow(() => manager.SetValue("test-key", value));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Should.NotThrow(() => new NullKeyVaultManager(null));
        }

        [Fact]
        public void MultipleOperations_ShouldBeConsistent()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act & Assert
            manager.GetKeyValues().ShouldBeEmpty();
            manager.GetValue("key1").ShouldBeNull();
            manager.GetValue("key2").ShouldBeNull();

            Should.NotThrow(() => manager.SetValue("key1", "value1"));
            Should.NotThrow(() => manager.SetValue("key2", "value2"));

            // Values should still be null after setting
            manager.GetValue("key1").ShouldBeNull();
            manager.GetValue("key2").ShouldBeNull();
            manager.GetKeyValues().ShouldBeEmpty();
        }

        [Fact]
        public async Task MultipleAsyncOperations_ShouldBeConsistent()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act & Assert
            (await manager.GetKeyValuesAsync()).ShouldBeEmpty();
            (await manager.GetValueAsync("key1")).ShouldBeNull();
            (await manager.GetValueAsync("key2")).ShouldBeNull();

            await Should.NotThrowAsync(async () => await manager.SetValueAsync("key1", "value1"));
            await Should.NotThrowAsync(async () => await manager.SetValueAsync("key2", "value2"));

            // Values should still be null after setting
            (await manager.GetValueAsync("key1")).ShouldBeNull();
            (await manager.GetValueAsync("key2")).ShouldBeNull();
            (await manager.GetKeyValuesAsync()).ShouldBeEmpty();
        }

        [Fact]
        public void GetKeyValues_MultipleCalls_ShouldReturnNewInstancesButEmpty()
        {
            // Arrange
            var manager = new NullKeyVaultManager(_logger);

            // Act
            var result1 = manager.GetKeyValues();
            var result2 = manager.GetKeyValues();

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();
            result1.ShouldBeEmpty();
            result2.ShouldBeEmpty();
            // Should be different instances
            ReferenceEquals(result1, result2).ShouldBeFalse();
        }
    }
}