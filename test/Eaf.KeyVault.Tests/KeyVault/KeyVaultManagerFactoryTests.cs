using Castle.Core.Logging;
using Shouldly;
using System;
using Xunit;

namespace Eaf.KeyVault.Tests.KeyVault
{
    /// <summary>
    /// Testes para KeyVaultManagerFactory — Spec 84.
    /// </summary>
    public class KeyVaultManagerFactoryTests
    {
        private readonly KeyVaultManagerFactory _factory;

        public KeyVaultManagerFactoryTests()
        {
            _factory = new KeyVaultManagerFactory(NullLogger.Instance);
        }

        #region Constructor

        [Fact]
        public void Dado_LoggerNulo_Quando_Instanciar_Entao_DeveUsarNullLogger()
        {
            // Dado & Quando
            var factory = new KeyVaultManagerFactory(null);

            // Então
            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_LoggerValido_Quando_Instanciar_Entao_DeveCriarInstancia()
        {
            // Dado & Quando
            var factory = new KeyVaultManagerFactory(NullLogger.Instance);

            // Então
            factory.ShouldNotBeNull();
        }

        #endregion

        #region Create - Azure Provider

        [Fact]
        public void Dado_ProviderAzure_SemEndpoint_Quando_Create_Entao_DeveLancarArgumentNullException()
        {
            // Dado — Azure requer Endpoint (vaultUri)
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.Azure };

            // Quando & Então — sem Endpoint, o AzureKeyVaultManager lança exceção na construção
            Should.Throw<ArgumentNullException>(() => _factory.Create(options));
        }

        [Fact]
        public void Dado_ProviderAzure_ComEndpoint_Quando_Create_Entao_DeveRetornarAzureKeyVaultManager()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new Uri("https://myvault.vault.azure.net")
            };

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeOfType<AzureKeyVaultManager>();
        }

        #endregion

        #region Create - OCI Provider

        [Fact]
        public void Dado_ProviderOCI_SemConfigFile_Quando_Create_Entao_DeveLancarIOException()
        {
            // Dado — OCI requer arquivo de config ~/.oci/config
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.OCI };

            // Quando & Então — sem config OCI, lança IOException
            Should.Throw<System.IO.IOException>(() => _factory.Create(options));
        }

        #endregion

        #region Create - None/Default Provider (Null Object Pattern)

        [Fact]
        public void Dado_ProviderNone_Quando_Create_Entao_DeveRetornarNullKeyVaultManager()
        {
            // Dado
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NullKeyVaultManager>();
        }

        [Fact]
        public void Dado_OptionsNulo_Quando_Create_Entao_DeveRetornarNullKeyVaultManager()
        {
            // Dado
            EafKeyVaultOptions options = null;

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NullKeyVaultManager>();
        }

        [Fact]
        public void Dado_ProviderDesconhecido_Quando_Create_Entao_DeveRetornarNullKeyVaultManager()
        {
            // Dado
            var options = new EafKeyVaultOptions { Provider = (EnumKeyVault)999 };

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NullKeyVaultManager>();
        }

        #endregion

        #region Interface Contract

        [Fact]
        public void Dado_Factory_Quando_VerificarInterface_Entao_DeveImplementarIKeyVaultManagerFactory()
        {
            // Dado & Quando
            var type = typeof(KeyVaultManagerFactory);

            // Então
            typeof(IKeyVaultManagerFactory).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void Dado_Factory_Quando_VerificarVisibilidade_Entao_DeveSerInternal()
        {
            // Dado & Quando
            var type = typeof(KeyVaultManagerFactory);

            // Então
            type.IsPublic.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ProviderNone_Quando_Create_Entao_ResultadoDeveImplementarIKeyVaultManager()
        {
            // Dado
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldBeAssignableTo<IKeyVaultManager>();
        }

        [Fact]
        public void Dado_ProviderAzureComEndpoint_Quando_Create_Entao_ResultadoDeveImplementarIKeyVaultManager()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new Uri("https://test.vault.azure.net")
            };

            // Quando
            var result = _factory.Create(options);

            // Então
            result.ShouldBeAssignableTo<IKeyVaultManager>();
        }

        #endregion
    }
}
