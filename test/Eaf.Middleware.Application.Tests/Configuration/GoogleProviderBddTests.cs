using Abp.Configuration;
using Eaf.Middleware.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class GoogleProviderBddTests
    {
        [Fact]
        public void Dado_ProviderGoogle_Quando_ObterDefinicoes_Entao_DeveRetornarConfiguracoesEsperadas()
        {
            var providerType = typeof(GoogleAppService).Assembly.GetType("Eaf.Middleware.Configuration.GoogleProvider");
            providerType.ShouldNotBeNull();

            var provider = Activator.CreateInstance(providerType!);
            provider.ShouldNotBeNull();

            var method = providerType!.GetMethod("GetSettingDefinitions", new[] { typeof(SettingDefinitionProviderContext) });
            method.ShouldNotBeNull();

            var context = new SettingDefinitionProviderContext(Substitute.For<ISettingDefinitionManager>());
            var result = (IEnumerable<SettingDefinition>)method!.Invoke(provider, new object[] { context })!;

            var definitions = result.ToList();
            definitions.Count.ShouldBe(3);
            definitions.Any(d => d.Name == EafMiddlewareSettingNames.Google.Analytics).ShouldBeTrue();
            definitions.Any(d => d.Name == EafMiddlewareSettingNames.Google.TagManager).ShouldBeTrue();
            definitions.Any(d => d.Name == EafMiddlewareSettingNames.Google.RecaptchaSiteKey).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ProviderGoogle_Quando_Criar_Entao_DeveSerSettingProvider()
        {
            var providerType = typeof(GoogleAppService).Assembly.GetType("Eaf.Middleware.Configuration.GoogleProvider");
            providerType.ShouldNotBeNull();
            typeof(SettingProvider).IsAssignableFrom(providerType!).ShouldBeTrue();
        }
    }
}
