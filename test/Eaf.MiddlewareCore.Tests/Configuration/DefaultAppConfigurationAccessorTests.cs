using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Configuration.Tests
{
    /// <summary>
    /// Testes para DefaultAppConfigurationAccessor usando estilo BDD
    /// </summary>
    public class DefaultAppConfigurationAccessorTests
    {
        [Fact]
        public void Constructor_DeveCriarInstanciaValida()
        {
            // Dado & Quando
            var accessor = new DefaultAppConfigurationAccessor();

            // Então
            accessor.ShouldNotBeNull();
            accessor.ShouldBeOfType<DefaultAppConfigurationAccessor>();
            accessor.Configuration.ShouldNotBeNull();
        }

        [Fact]
        public void Configuration_DeveRetornarConfiguracaoQuandoExistir()
        {
            // Dado
            var accessor = new DefaultAppConfigurationAccessor();

            // Quando
            var configuration = accessor.Configuration;

            // Então
            configuration.ShouldNotBeNull();
            configuration.ShouldBeAssignableTo<IConfigurationRoot>();
        }

        [Fact]
        public void Configuration_DeveSerDoTipoIConfigurationRoot()
        {
            // Dado
            var accessor = new DefaultAppConfigurationAccessor();

            // Quando
            var configuration = accessor.Configuration;

            // Então
            configuration.ShouldNotBeNull();
            configuration.ShouldBeAssignableTo<IConfigurationRoot>();
        }
    }
}
