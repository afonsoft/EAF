using Abp.Configuration.Startup;
using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    public class EafStartupConfigurationExtensionsBddTests
    {
        [Fact]
        public void Dado_ConfiguracaoComSecaoAninhada_Quando_SetConfiguration_Entao_DeveChamarSet()
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var section = CriarSecao("Section1", new Dictionary<string, string>
            {
                { "Section1:Key1", "Value1" },
                { "Section1:Key2", "Value2" }
            });

            configuration.SetConfiguration(section);

            configuration.Received(1).Set("Section1", Arg.Any<Dictionary<string, object>>());
        }

        [Fact]
        public void Dado_ConfiguracaoComColecaoDeSecoes_Quando_SetConfiguration_Entao_DeveChamarSetParaCadaSecao()
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var section1 = CriarSecao("Section1", new Dictionary<string, string> { { "Section1:Key", "Value" } });
            var section2 = CriarSecao("Section2", new Dictionary<string, string> { { "Section2:Key", "Value" } });

            configuration.SetConfiguration(new List<IConfigurationSection> { section1, section2 });

            configuration.Received(2).Set(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
        }

        [Fact]
        public void Dado_SecaoNaoExistente_Quando_SetConfiguration_Entao_NaoDeveChamarSet()
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var section = CriarSecao("SectionEmpty", new Dictionary<string, string>());

            configuration.SetConfiguration(section);

            configuration.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
        }

        [Fact]
        public void Dado_SecaoSemFilhos_Quando_SetConfiguration_Entao_NaoDeveChamarSet()
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var section = CriarSecao("SectionSingle", new Dictionary<string, string> { { "SectionSingle", "Value" } });

            configuration.SetConfiguration(section);

            configuration.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
        }

        private static IConfigurationSection CriarSecao(string key, Dictionary<string, string> values)
        {
            var valuesWithNullable = values.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(valuesWithNullable)
                .Build();

            return configuration.GetSection(key);
        }
    }
}
