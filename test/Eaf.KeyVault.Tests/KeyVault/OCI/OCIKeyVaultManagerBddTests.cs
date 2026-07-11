using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace Eaf.KeyVault.Tests.KeyVault.OCI
{
    /// <summary>
    /// Testes BDD para OCIKeyVaultManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class OCIKeyVaultManagerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarInterface_Entao_DeveImplementarIKeyVaultManager()
        {
            typeof(OCIKeyVaultManager).ShouldNotBeNull();
            typeof(IKeyVaultManager).IsAssignableFrom(typeof(OCIKeyVaultManager)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_OptionsComConfigFileInvalido_Quando_Construir_Entao_DeveLogarErroELancarExcecao()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            options.Oci.ConfigFile = "/tmp/oci-config-invalid-" + Guid.NewGuid();
            var logger = Substitute.For<ILogger>();

            // Quando & Então
            Should.Throw<Exception>(() => new OCIKeyVaultManager(options, logger));
            logger.Received(1).ErrorFormat(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void Dado_Instancia_Quando_SetValue_Entao_DeveLancarNotImplementedException()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            Should.Throw<NotImplementedException>(() => sut.SetValue("key", "value"));
        }

        [Fact]
        public void Dado_Instancia_Quando_SetValueAsync_Entao_DeveLancarNotImplementedException()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            Should.Throw<NotImplementedException>(() => sut.SetValueAsync("key", "value"));
        }
    }
}
