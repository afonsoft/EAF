using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    /// <summary>
    /// Testes BDD para NullKeyVaultManager seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NullKeyVaultManagerBddTests
    {
        private readonly EafKeyVaultOptions _options;
        private readonly ILogger _mockLogger;

        public NullKeyVaultManagerBddTests()
        {
            _options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            _mockLogger = Substitute.For<ILogger>();
        }

        #region Testes do Construtor

        [Fact]
        public void Dado_OpcoesELoggerValidos_Quando_CriarNullKeyVaultManager_Entao_DeveInicializarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var logger = Substitute.For<ILogger>();

            // Quando
            var manager = new NullKeyVaultManager(options, logger);

            // Então
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OpcoesNulas_Quando_CriarNullKeyVaultManager_Entao_DeveAceitarOpcoesNulas()
        {
            // Dado
            EafKeyVaultOptions options = null!;
            var logger = Substitute.For<ILogger>();

            // Quando & Então
            Should.NotThrow(() => new NullKeyVaultManager(options, logger));
        }

        [Fact]
        public void Dado_LoggerNulo_Quando_CriarNullKeyVaultManager_Entao_DeveAceitarLoggerNulo()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            ILogger logger = null!;

            // Quando & Então
            Should.NotThrow(() => new NullKeyVaultManager(options, logger));
        }

        #endregion

        #region Testes GetKeyValues

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_ChamarGetKeyValues_Entao_DeveRetornarDicionarioVazio()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            var resultado = manager.GetKeyValues();

            // Então
            resultado.ShouldNotBeNull();
            resultado.ShouldBeOfType<Dictionary<string, string>>();
            resultado.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_NullKeyVaultManager_Quando_ChamarGetKeyValuesAsync_Entao_DeveRetornarDicionarioVazio()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            var resultado = await manager.GetKeyValuesAsync();

            // Então
            resultado.ShouldNotBeNull();
            resultado.ShouldBeOfType<Dictionary<string, string>>();
            resultado.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_ChamarGetKeyValues_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.GetKeyValues();

            // Então
            _mockLogger.Received(1).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_NullKeyVaultManager_Quando_ChamarGetKeyValuesAsync_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            await manager.GetKeyValuesAsync();

            // Então
            _mockLogger.Received(2).Debug("NullKeyVaultManager : NotImplementedException"); // Uma para GetKeyValuesAsync e uma para GetKeyValues
            manager.ShouldNotBeNull();
        }

        #endregion

        #region Testes GetValue

        [Theory]
        [InlineData("chave-teste")]
        [InlineData("")]
        [InlineData("chave-com-caracteres-especiais-!@#$%")]
        [InlineData("chave-muito-longa-com-muitos-caracteres-para-testar-limites")]
        public void Dado_ChaveQualquer_Quando_ChamarGetValue_Entao_DeveRetornarNull(string chave)
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            var resultado = manager.GetValue(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Theory]
        [InlineData("chave-teste")]
        [InlineData("")]
        [InlineData("chave-com-caracteres-especiais-!@#$%")]
        [InlineData("chave-muito-longa-com-muitos-caracteres-para-testar-limites")]
        public async Task Dado_ChaveQualquer_Quando_ChamarGetValueAsync_Entao_DeveRetornarNull(string chave)
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            var resultado = await manager.GetValueAsync(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarGetValue_Entao_DeveRetornarNull()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = null!;

            // Quando
            var resultado = manager.GetValue(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_ChaveNula_Quando_ChamarGetValueAsync_Entao_DeveRetornarNull()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = null!;

            // Quando
            var resultado = await manager.GetValueAsync(chave);

            // Então
            resultado.ShouldBeNull();
        }

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_ChamarGetValue_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.GetValue("teste");

            // Então
            _mockLogger.Received(1).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_NullKeyVaultManager_Quando_ChamarGetValueAsync_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            await manager.GetValueAsync("teste");

            // Então
            _mockLogger.Received(2).Debug("NullKeyVaultManager : NotImplementedException"); // Uma para GetValueAsync e uma para GetValue
            manager.ShouldNotBeNull();
        }

        #endregion

        #region Testes SetValue

        [Theory]
        [InlineData("chave-teste", "valor-teste")]
        [InlineData("", "")]
        [InlineData("chave-especial-!@#", "valor-especial-$%^")]
        [InlineData("chave-longa", "valor-muito-longo-com-muitos-caracteres-para-testar-limites-de-tamanho")]
        public void Dado_ChaveEValorValidos_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao(string chave, string valor)
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Theory]
        [InlineData("chave-teste", "valor-teste")]
        [InlineData("", "")]
        [InlineData("chave-especial-!@#", "valor-especial-$%^")]
        [InlineData("chave-longa", "valor-muito-longo-com-muitos-caracteres-para-testar-limites-de-tamanho")]
        public async Task Dado_ChaveEValorValidos_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao(string chave, string valor)
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = null!;
            string valor = "valor-teste";

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Fact]
        public async Task Dado_ChaveNula_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = null!;
            string valor = "valor-teste";

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        [Fact]
        public void Dado_ValorNulo_Quando_ChamarSetValue_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = "chave-teste";
            string valor = null!;

            // Quando & Então
            Should.NotThrow(() => manager.SetValue(chave, valor));
        }

        [Fact]
        public async Task Dado_ValorNulo_Quando_ChamarSetValueAsync_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            string chave = "chave-teste";
            string valor = null!;

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync(chave, valor));
        }

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_ChamarSetValue_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.SetValue("teste", "valor");

            // Então
            _mockLogger.Received(1).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_NullKeyVaultManager_Quando_ChamarSetValueAsync_Entao_DeveRegistrarLogDebug()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            await manager.SetValueAsync("teste", "valor");

            // Então
            _mockLogger.Received(2).Debug("NullKeyVaultManager : NotImplementedException"); // Uma para SetValueAsync e uma para SetValue
            manager.ShouldNotBeNull();
        }

        #endregion

        #region Testes de Comportamento Consistente

        [Fact]
        public void Dado_MultiplasChaves_Quando_ChamarGetValue_Entao_DeveSempreRetornarNull()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            var chaves = new[] { "chave1", "chave2", "chave3", "", null, "chave-especial-!@#" };

            // Quando & Então
            foreach (var chave in chaves)
            {
                var resultado = manager.GetValue(chave);
                resultado.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Dado_MultiplasChaves_Quando_ChamarGetValueAsync_Entao_DeveSempreRetornarNull()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            var chaves = new[] { "chave1", "chave2", "chave3", "", null, "chave-especial-!@#" };

            // Quando & Então
            foreach (var chave in chaves)
            {
                var resultado = await manager.GetValueAsync(chave);
                resultado.ShouldBeNull();
            }
        }

        [Fact]
        public void Dado_SequenciaDeOperacoes_Quando_ExecutarSetEGet_Entao_DeveManterComportamentoConsistente()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.SetValue("chave", "valor");
            var resultado1 = manager.GetValue("chave");
            manager.SetValue("chave", "novo-valor");
            var resultado2 = manager.GetValue("chave");
            var todosValores = manager.GetKeyValues();

            // Então
            resultado1.ShouldBeNull();
            resultado2.ShouldBeNull();
            todosValores.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_SequenciaDeOperacoesAsync_Quando_ExecutarSetEGet_Entao_DeveManterComportamentoConsistente()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            await manager.SetValueAsync("chave", "valor");
            var resultado1 = await manager.GetValueAsync("chave");
            await manager.SetValueAsync("chave", "novo-valor");
            var resultado2 = await manager.GetValueAsync("chave");
            var todosValores = await manager.GetKeyValuesAsync();

            // Então
            resultado1.ShouldBeNull();
            resultado2.ShouldBeNull();
            todosValores.Count.ShouldBe(0);
        }

        #endregion

        #region Testes de Performance e Concorrência

        [Fact]
        public async Task Dado_MultiplasOperacoesSimultaneas_Quando_ExecutarConcorrentemente_Entao_DeveManterConsistencia()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
            var tasks = new List<Task>();

            // Quando
            for (int i = 0; i < 50; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await manager.SetValueAsync($"chave-{index}", $"valor-{index}");
                    var valor = await manager.GetValueAsync($"chave-{index}");
                    var valores = await manager.GetKeyValuesAsync();

                    // Verificações dentro da task
                    valor.ShouldBeNull();
                    valores.Count.ShouldBe(0);
                }));
            }

            // Então
            await Should.NotThrowAsync(async () => await Task.WhenAll(tasks));
        }

        [Fact]
        public void Dado_OperacoesSequenciaisRapidas_Quando_ExecutarEmLoop_Entao_DeveManterPerformance()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando & Então
            Should.NotThrow(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    manager.SetValue($"chave-{i}", $"valor-{i}");
                    var valor = manager.GetValue($"chave-{i}");
                    var valores = manager.GetKeyValues();

                    valor.ShouldBeNull();
                    valores.Count.ShouldBe(0);
                }
            });
        }

        #endregion

        #region Testes de Validação de Interface

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_VerificarTipos_Entao_DeveImplementarInterfaceCorreta()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando & Então
            manager.ShouldBeAssignableTo<IKeyVaultManager>();
        }

        [Fact]
        public void Dado_NullKeyVaultManager_Quando_VerificarMetodos_Entao_DeveTerTodosMetodosPublicos()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);
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

        #region Testes de Logging Detalhado

        [Fact]
        public void Dado_MultiplasChamadasParaGetValue_Quando_Executar_Entao_DeveRegistrarLogParaCadaChamada()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.GetValue("chave1");
            manager.GetValue("chave2");
            manager.GetValue("chave3");

            // Então
            _mockLogger.Received(3).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_MultiplasChamadasParaSetValue_Quando_Executar_Entao_DeveRegistrarLogParaCadaChamada()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.SetValue("chave1", "valor1");
            manager.SetValue("chave2", "valor2");
            manager.SetValue("chave3", "valor3");

            // Então
            _mockLogger.Received(3).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_MixDeOperacoes_Quando_Executar_Entao_DeveRegistrarLogParaTodasOperacoes()
        {
            // Dado
            var manager = new NullKeyVaultManager(_options, _mockLogger);

            // Quando
            manager.GetKeyValues();      // 1 log
            manager.GetValue("teste");   // 1 log
            manager.SetValue("teste", "valor"); // 1 log

            // Então
            _mockLogger.Received(3).Debug("NullKeyVaultManager : NotImplementedException");
            manager.ShouldNotBeNull();
        }

        #endregion
    }
}