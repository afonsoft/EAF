using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Localization.Sources;
using Eaf.Middleware.Localization;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Localization
{
    /// <summary>
    /// Testes BDD para MiddlewareLocalizationConfigurer seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareLocalizationConfigurerBddTests
    {
        #region Configure

        [Fact]
        public void Dado_LocalizationConfiguration_Quando_Configure_Entao_DeveAdicionarSource()
        {
            // Dado
            var config = Substitute.For<ILocalizationConfiguration>();
            var sources = Substitute.For<ILocalizationSourceList>();
            config.Sources.Returns(sources);

            // Quando
            MiddlewareLocalizationConfigurer.Configure(config);

            // Entao
            sources.Received(1).Add(Arg.Is<ILocalizationSource>(s => s.Name == "EafCore"));
        }

        #endregion
    }
}
