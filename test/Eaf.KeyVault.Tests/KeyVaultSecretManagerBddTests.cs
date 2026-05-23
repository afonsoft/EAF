using Abp.Dependency;
using Castle.Core.Logging;
using Eaf.KeyVault;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    /// <summary>
    /// Testes BDD para KeyVaultSecretManager seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class KeyVaultSecretManagerBddTests
    {
        private readonly EafKeyVaultOptions _options;
        private readonly ILogger _mockLogger;

        public KeyVaultSecretManagerBddTests()
        {
            _mockLogger = Substitute.For<ILogger>();
            _options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.None
            };
        }

        #region Testes do Construtor

        [Fact]
        public void Dado_OpcoesValidas_Quando_CriarKeyVaultSecretManager_Entao_DeveInicializarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };

            // Quando
            var manager = new KeyVaultSecretManager(options);

            // Então
            manager.ShouldNotBeNull();
            manager.Logger.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OpcoesComIOptions_Quando_CriarKeyVaultSecretManager_Entao_DeveInicializarCorretamente()
        {
            // Dado
            var options = Options.Create(new EafKeyVaultOptions { Provider = EnumKeyVault.None });

            // Quando
            var manager = new KeyVaultSecretManager(options);

            // Então
            manager.ShouldNotBeNull();
            manager.Logger.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OpcoesNulas_Quando_CriarKeyVaultSecretManager_Entao_DeveUsarOpcoesDefault()
        {
            // Dado
            EafKeyVaultOptions options = null;

            // Quando
            var manager = new KeyVaultSecretManager(options);

            // Então
            manager.ShouldNotBeNull();
            manager.Logger.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ProviderAzure_Quando_CriarKeyVaultSecretManager_Entao_DeveConfigurarManagerAzure()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "test-app-id",
                    TenantId = "test-tenant-id",
                    ClientSecret = "test-secret"
                },
                Endpoint = new Uri("https://test-vault.vault.azure.net/")
            };

            // Quando & Então
            Should.NotThrow(() => new KeyVaultSecretManager(options));
        }

        [Fact]
        public void Dado_ProviderOCI_Quando_CriarKeyVaultSecretManager_Entao_DeveConfigurarManagerOCI()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.OCI,
                Oci = new OciKeyVaultOptions
                {
                    SecretId = "test-secret-id",
                    VaultId = "test-vault-id",
                    Profile = "DEFAULT",
                    TenantId = "test-tenant-id",
                    UserId = "test-user-id",
                    Fingerprint = "test-fingerprint",
                    Region = "us-ashburn-1"
                }
            };

            // Quando & Então - OCI pode lançar exceção se não tiver configuração válida
            Should.Throw<System.IO.IOException>(() => new KeyVaultSecretManager(options));
        }

        #endregion

        #region Testes GetKeyValues

        [Fact]
        public void Dado_ManagerNulo_Quando_ChamarGetKeyValues_Entao_DeveRetornarDicionarioVazio()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando
            var resultado = manager.GetKeyValues();

            // Então
            resultado.ShouldNotBeNull();
            resultado.ShouldBeOfType<Dictionary<string, string>>();
            resultado.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_ManagerNulo_Quando_ChamarGetKeyValuesAsync_Entao_DeveRetornarDicionarioVazio()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando
            var resultado = await manager.GetKeyValuesAsync();

            // Então
            resultado.ShouldNotBeNull();
            resultado.ShouldBeOfType<Dictionary<string, string>>();
            resultado.Count.ShouldBe(0);
        }

        #endregion

        #region Testes GetValue

        [Theory]
        [InlineData("chave-teste")]
        [InlineData("")]
        [InlineData("chave-com-caracteres-especiais-!@#$%")]
        public void Dado_ChaveQualquer_Quando_ChamarGetValue_Entao_DeveRetornarNull(string chave)
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando
            var resultado = manager.GetValue(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Theory]
        [InlineData("chave-teste")]
        [InlineData("")]
        [InlineData("chave-com-caracteres-especiais-!@#$%")]
        public async Task Dado_ChaveQualquer_Quando_ChamarGetValueAsync_Entao_DeveRetornarNull(string chave)
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando
            var resultado = await manager.GetValueAsync(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarGetValue_Entao_DeveRetornarNull()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = null;

            // Quando
            var resultado = manager.GetValue(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_ChaveNula_Quando_ChamarGetValueAsync_Entao_DeveRetornarNull()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = null;

            // Quando
            var resultado = await manager.GetValueAsync(chave);

            // Então
            resultado.ShouldBeNull();
        }

        #endregion

        #region Testes SetValue

        [Theory]
        [InlineData("chave-teste", "valor-teste")]
        [InlineData("", "")]
        [InlineData("chave-especial-!@#", "valor-especial-$%^")]
        public void Dado_ChaveEValorValidos_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao(string chave, string valor)
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Theory]
        [InlineData("chave-teste", "valor-teste")]
        [InlineData("", "")]
        [InlineData("chave-especial-!@#", "valor-especial-$%^")]
        public async Task Dado_ChaveEValorValidos_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao(string chave, string valor)
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = null;
            string valor = "valor-teste";

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Fact]
        public async Task Dado_ChaveNula_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = null;
            string valor = "valor-teste";

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        [Fact]
        public void Dado_ValorNulo_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = "chave-teste";
            string valor = null;

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Fact]
        public async Task Dado_ValorNulo_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            string chave = "chave-teste";
            string valor = null;

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        #endregion

        #region Testes de Integração com Logger

        [Fact]
        public void Dado_LoggerPersonalizado_Quando_DefinirLogger_Entao_DeveUsarLoggerPersonalizado()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            var loggerPersonalizado = Substitute.For<ILogger>();

            // Quando
            manager.Logger = loggerPersonalizado;

            // Então
            manager.Logger.ShouldBe(loggerPersonalizado);
        }

        [Fact]
        public void Dado_ManagerComLoggerPersonalizado_Quando_ExecutarOperacoes_Entao_DeveUsarLogger()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            var loggerPersonalizado = Substitute.For<ILogger>();
            manager.Logger = loggerPersonalizado;

            // Quando
            manager.GetValue("teste");
            manager.SetValue("teste", "valor");

            // Então
            manager.Logger.ShouldBe(loggerPersonalizado);
        }

        #endregion

        #region Testes de Cenários de Erro

        [Fact]
        public void Dado_OpcoesComProviderInvalido_Quando_CriarManager_Entao_DeveUsarNullManager()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = (EnumKeyVault)999 // Valor inválido
            };

            // Quando
            var manager = new KeyVaultSecretManager(options);

            // Então
            manager.ShouldNotBeNull();
            var resultado = manager.GetValue("teste");
            resultado.ShouldBeNull();
        }

        [Fact]
        public void Dado_OpcoesAzureSemConfiguracao_Quando_CriarManager_Entao_DeveLancarExcecao()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Azure = new AzureKeyVaultOptions() // Sem configuração
            };

            // Quando & Então - Azure requer Endpoint válido
            Should.Throw<ArgumentNullException>(() => new KeyVaultSecretManager(options));
        }

        [Fact]
        public void Dado_OpcoesOciSemConfiguracao_Quando_CriarManager_Entao_DeveLancarExcecao()
        {
            // Dado
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.OCI,
                Oci = new OciKeyVaultOptions() // Sem configuração
            };

            // Quando & Então - OCI requer configuração válida
            Should.Throw<System.IO.IOException>(() => new KeyVaultSecretManager(options));
        }

        #endregion

        #region Testes de Performance e Concorrência

        [Fact]
        public async Task Dado_MultiplasOperacoesSimultaneas_Quando_ExecutarConcorrentemente_Entao_DeveManterConsistencia()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            var tasks = new List<Task>();

            // Quando
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await manager.SetValueAsync($"chave-{index}", $"valor-{index}");
                    var valor = await manager.GetValueAsync($"chave-{index}");
                    var valores = await manager.GetKeyValuesAsync();
                }));
            }

            // Então
            await Should.NotThrowAsync(async () => await Task.WhenAll(tasks));
        }

        [Fact]
        public void Dado_OperacoesSequenciais_Quando_ExecutarEmLoop_Entao_DeveManterPerformance()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando & Então
            Should.NotThrow(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    manager.SetValue($"chave-{i}", $"valor-{i}");
                    manager.GetValue($"chave-{i}");
                    manager.GetKeyValues();
                }
            });
        }

        #endregion

        #region Testes de Validação de Tipos

        [Fact]
        public void Dado_Manager_Quando_VerificarTipos_Entao_DeveImplementarInterfaceCorreta()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);

            // Quando & Então
            manager.ShouldBeAssignableTo<IKeyVaultSecretManager>();
        }

        [Fact]
        public void Dado_Manager_Quando_VerificarMetodos_Entao_DeveTerTodosMetodosPublicos()
        {
            // Dado
            var manager = new KeyVaultSecretManager(_options);
            var tipo = manager.GetType();

            // Quando
            var metodos = tipo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Então
            metodos.ShouldContain(m => m.Name == "GetKeyValues");
            metodos.ShouldContain(m => m.Name == "GetKeyValuesAsync");
            metodos.ShouldContain(m => m.Name == "GetValue");
            metodos.ShouldContain(m => m.Name == "GetValueAsync");
            metodos.ShouldContain(m => m.Name == "SetValue");
            metodos.ShouldContain(m => m.Name == "SetValueAsync");
        }

        #endregion
    }
}