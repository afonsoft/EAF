using Abp.Configuration.Startup;
using Eaf.Middleware.Web.Configuration;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Configuration
{
    /// <summary>
    /// Testes BDD para EafStartupConfigurationExtensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafStartupConfigurationExtensionsBddTests
    {
        private readonly IAbpStartupConfiguration _configuration;

        public EafStartupConfigurationExtensionsBddTests()
        {
            _configuration = Substitute.For<IAbpStartupConfiguration>();
        }

        #region SetConfiguration com IEnumerable

        [Fact]
        public void Dado_SectionsVazias_Quando_SetConfiguration_Entao_NaoDeveChamarSet()
        {
            // Dado
            var sections = new List<IConfigurationSection>();

            // Quando
            _configuration.SetConfiguration(sections);

            // Entao
            _configuration.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public void Dado_SectionComFilhos_Quando_SetConfiguration_Entao_DeveChamarSet()
        {
            // Dado
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TestSection:Key1", "Value1" },
                { "TestSection:Key2", "Value2" }
            });
            var configRoot = builder.Build();
            var sections = new List<IConfigurationSection> { configRoot.GetSection("TestSection") };

            // Quando
            _configuration.SetConfiguration(sections);

            // Entao
            _configuration.Received(1).Set("TestSection", Arg.Any<object>());
        }

        #endregion

        #region SetConfiguration com unica section

        [Fact]
        public void Dado_SectionUnica_Quando_SetConfiguration_Entao_DeveChamarSet()
        {
            // Dado
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MySection:SubKey", "SubValue" }
            });
            var configRoot = builder.Build();
            var section = configRoot.GetSection("MySection");

            // Quando
            _configuration.SetConfiguration(section);

            // Entao
            _configuration.Received(1).Set("MySection", Arg.Any<object>());
        }

        [Fact]
        public void Dado_SectionSemFilhos_Quando_SetConfiguration_Entao_NaoDeveChamarSet()
        {
            // Dado
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>());
            var configRoot = builder.Build();
            var section = configRoot.GetSection("EmptySection");

            // Quando
            _configuration.SetConfiguration(section);

            // Entao
            _configuration.DidNotReceive().Set(Arg.Any<string>(), Arg.Any<object>());
        }

        #endregion

        #region Nested sections

        [Fact]
        public void Dado_SectionComFilhosAninhados_Quando_SetConfiguration_Entao_DeveChamarSetComDicionario()
        {
            // Dado
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Parent:Child:GrandChild", "DeepValue" }
            });
            var configRoot = builder.Build();
            var section = configRoot.GetSection("Parent");

            // Quando
            _configuration.SetConfiguration(section);

            // Entao
            _configuration.Received(1).Set("Parent", Arg.Any<object>());
        }

        #endregion
    }
}
