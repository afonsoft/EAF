using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para AppSettingProvider seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AppSettingProviderBddTests
    {
        private readonly AppSettingProvider _provider;

        public AppSettingProviderBddTests()
        {
            var configData = new Dictionary<string, string>
            {
                { "App:ServerRootAddress", "http://localhost:8001" },
                { "App:ClientRootAddress", "http://localhost:8000" },
                { "Authentication:Google:IsEnabled", "true" },
                { "Authentication:Google:ClientId", "google-client-id" },
                { "Authentication:Google:ClientSecret", "google-secret" },
                { "Authentication:Microsoft:IsEnabled", "false" },
                { "Authentication:OpenId:IsEnabled", "false" },
                { "Authentication:AuthZero:IsEnabled", "false" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var accessor = Substitute.For<IAppConfigurationAccessor>();
            accessor.Configuration.Returns(config);

            _provider = new AppSettingProvider(accessor);
        }

        [Fact]
        public void Dado_AppSettingProvider_Quando_GetSettingDefinitions_Entao_DeveRetornarDefinicoes()
        {
            var settings = _provider.GetSettingDefinitions(null).ToList();

            settings.ShouldNotBeNull();
            settings.Count.ShouldBeGreaterThan(50);
        }

        [Fact]
        public void Dado_AppSettingProvider_Quando_GetSettingDefinitions_Entao_DeveConterThemeSettings()
        {
            var settings = _provider.GetSettingDefinitions(null).ToList();
            var names = settings.Select(s => s.Name).ToList();

            names.ShouldContain("App.UiManagement.Theme");
        }

        [Fact]
        public void Dado_AppSettingProvider_Quando_GetSettingDefinitions_Entao_DeveConterSettingsComScope()
        {
            var settings = _provider.GetSettingDefinitions(null).ToList();

            settings.Any(s => s.Scopes.HasFlag(Abp.Configuration.SettingScopes.Application)).ShouldBeTrue();
            settings.Any(s => s.Scopes.HasFlag(Abp.Configuration.SettingScopes.Tenant)).ShouldBeTrue();
        }
    }
}
