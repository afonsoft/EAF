using Eaf.KeyVault;
using Eaf.Hosting.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.AspNetCore.Tests
{
    public class EafKeyVaultHostWebExtensionsTests
    {
        [Fact]
        public void UseEafKeyVault_WithoutOptions_ShouldConfigureDefaultOptions()
        {
            // Arrange
            var webHostBuilder = Substitute.For<IWebHostBuilder>();
            var serviceCollection = new ServiceCollection();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();

            webHostBuilder.ConfigureServices(Arg.Do<Action<IServiceCollection>>(action => action(serviceCollection)))
                .Returns(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(
                action => action(new WebHostBuilderContext(), configurationBuilder)))
                .Returns(webHostBuilder);

            // Act
            var result = webHostBuilder.UseEafKeyVault();

            // Assert
            result.ShouldBe(webHostBuilder);
            webHostBuilder.Received(1).ConfigureServices(Arg.Any<Action<IServiceCollection>>());
            webHostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void UseEafKeyVault_WithOptions_ShouldConfigureCustomOptions()
        {
            // Arrange
            var webHostBuilder = Substitute.For<IWebHostBuilder>();
            var serviceCollection = new ServiceCollection();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();

            webHostBuilder.ConfigureServices(Arg.Do<Action<IServiceCollection>>(action => action(serviceCollection)))
                .Returns(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(
                action => action(new WebHostBuilderContext(), configurationBuilder)))
                .Returns(webHostBuilder);

            // Act
            var result = webHostBuilder.UseEafKeyVault(options =>
            {
                options.Provider = EnumKeyVault.Azure;
            });

            // Assert
            result.ShouldBe(webHostBuilder);
            webHostBuilder.Received(1).ConfigureServices(Arg.Any<Action<IServiceCollection>>());
            webHostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void UseEafKeyVault_ShouldAddOptionsToServiceCollection()
        {
            // Arrange
            var webHostBuilder = Substitute.For<IWebHostBuilder>();
            var serviceCollection = new ServiceCollection();

            webHostBuilder.ConfigureServices(Arg.Do<Action<IServiceCollection>>(action => action(serviceCollection)))
                .Returns(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>())
                .Returns(webHostBuilder);

            // Act
            webHostBuilder.UseEafKeyVault();

            // Assert
            serviceCollection.ShouldContain(descriptor =>
                descriptor.ServiceType == typeof(IConfigureOptions<EafKeyVaultOptions>));
        }

        [Fact]
        public void UseEafKeyVault_WithNullOptions_ShouldSetProviderToNone()
        {
            // Arrange
            var webHostBuilder = Substitute.For<IWebHostBuilder>();
            var serviceCollection = new ServiceCollection();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();

            webHostBuilder.ConfigureServices(Arg.Do<Action<IServiceCollection>>(action => action(serviceCollection)))
                .Returns(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(
                action => action(new WebHostBuilderContext(), configurationBuilder)))
                .Returns(webHostBuilder);

            // Act
            webHostBuilder.UseEafKeyVault(null);

            // Assert
            serviceCollection.ShouldContain(descriptor =>
                descriptor.ServiceType == typeof(IConfigureOptions<EafKeyVaultOptions>));
        }

        [Fact]
        public void UseEafKeyVault_ShouldAddConfigurationSource()
        {
            // Arrange
            var webHostBuilder = Substitute.For<IWebHostBuilder>();
            var serviceCollection = new ServiceCollection();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();

            webHostBuilder.ConfigureServices(Arg.Do<Action<IServiceCollection>>(action => action(serviceCollection)))
                .Returns(webHostBuilder);
            webHostBuilder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(
                action => action(new WebHostBuilderContext(), configurationBuilder)))
                .Returns(webHostBuilder);

            // Act
            webHostBuilder.UseEafKeyVault();

            // Assert
            configurationBuilder.Received(1).Add(Arg.Any<EafKeyVaultConfigurationSource>());
        }
    }
}