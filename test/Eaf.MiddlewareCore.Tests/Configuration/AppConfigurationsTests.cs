using System;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Configuration.Tests
{
    /// <summary>
    /// Testes para AppConfigurations usando estilo BDD
    /// </summary>
    public class AppConfigurationsTests
    {
        [Fact]
        public void Get_DeveRetornarConfiguracaoQuandoDiretorioExiste()
        {
            // Dado
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Quando
            var configuration = AppConfigurations.Get(currentDirectory);

            // Então
            configuration.ShouldNotBeNull();
            configuration.ShouldBeAssignableTo<Microsoft.Extensions.Configuration.IConfigurationRoot>();
        }

        [Fact]
        public void Get_ComDiretorioNulo_DeveLancarExcecao()
        {
            // Dado & Quando & Então
            Should.Throw<ArgumentNullException>(() => AppConfigurations.Get(null));
        }

        [Fact]
        public void Get_ComDiretorioVazio_DeveLancarExcecao()
        {
            // Dado & Quando & Então
            Should.Throw<ArgumentException>(() => AppConfigurations.Get(""));
        }
    }
}
