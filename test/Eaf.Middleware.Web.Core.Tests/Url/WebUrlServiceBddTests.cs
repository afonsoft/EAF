using Eaf.Middleware.Configuration;
using Eaf.Middleware.Web.Url;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Url
{
    /// <summary>
    /// Testes BDD para WebUrlService e WebUrlServiceBase seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebUrlServiceBddTests
    {
        private WebUrlService CreateService(Dictionary<string, string?> configData)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var accessor = Substitute.For<IAppConfigurationAccessor>();
            accessor.Configuration.Returns(config);

            return new WebUrlService(accessor);
        }

        [Fact]
        public void Dado_WebUrlService_SemTenant_Quando_GetSiteRootAddress_Entao_DeveRetornarUrlSemPlaceholder()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://localhost:8000/" }
            });

            service.GetSiteRootAddress().ShouldBe("http://localhost:8000/");
        }

        [Fact]
        public void Dado_WebUrlService_SemTenant_Quando_GetServerRootAddress_Entao_DeveRetornarServerUrl()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://api.example.com/" },
                { "App:ClientRootAddress", "http://app.example.com/" }
            });

            service.GetServerRootAddress().ShouldBe("http://api.example.com/");
        }

        [Fact]
        public void Dado_WebUrlService_ComTenantPlaceholder_Quando_GetSiteRootAddress_Entao_DeveSubstituirTenant()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://{TENANCY_NAME}.example.com/" }
            });

            service.GetSiteRootAddress("acme").ShouldBe("http://acme.example.com/");
        }

        [Fact]
        public void Dado_WebUrlService_ComTenantPlaceholder_SemTenancyName_Quando_GetSiteRootAddress_Entao_DeveRemoverPlaceholder()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://{TENANCY_NAME}.example.com/" }
            });

            service.GetSiteRootAddress(null).ShouldBe("http://example.com/");
        }

        [Fact]
        public void Dado_WebUrlService_SemPlaceholder_Quando_SupportsTenancyNameInUrl_Entao_DeveRetornarFalse()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://localhost:8000/" }
            });

            service.SupportsTenancyNameInUrl.ShouldBeFalse();
        }

        [Fact]
        public void Dado_WebUrlService_ComPlaceholder_Quando_SupportsTenancyNameInUrl_Entao_DeveRetornarTrue()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://{TENANCY_NAME}.example.com/" }
            });

            service.SupportsTenancyNameInUrl.ShouldBeTrue();
        }

        [Fact]
        public void Dado_WebUrlService_Quando_GetRedirectAllowedExternalWebSites_Entao_DeveRetornarListaDeSites()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://localhost:8000/" },
                { "App:RedirectAllowedExternalWebSites", "http://site1.com,http://site2.com" }
            });

            var sites = service.GetRedirectAllowedExternalWebSites();
            sites.Count.ShouldBe(2);
            sites.ShouldContain("http://site1.com");
        }

        [Fact]
        public void Dado_WebUrlService_SemRedirectConfig_Quando_GetRedirectAllowedExternalWebSites_Entao_DeveRetornarListaVazia()
        {
            var service = CreateService(new Dictionary<string, string?>
            {
                { "App:ServerRootAddress", "http://localhost:8001/" },
                { "App:ClientRootAddress", "http://localhost:8000/" }
            });

            var sites = service.GetRedirectAllowedExternalWebSites();
            sites.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_WebUrlService_SemConfiguracao_Quando_GetSiteRootAddress_Entao_DeveRetornarPadrao()
        {
            var service = CreateService(new Dictionary<string, string?>());

            service.GetSiteRootAddress().ShouldBe("http://localhost:8000/");
        }

        [Fact]
        public void Dado_WebUrlService_SemConfiguracao_Quando_GetServerRootAddress_Entao_DeveRetornarPadrao()
        {
            var service = CreateService(new Dictionary<string, string?>());

            service.GetServerRootAddress().ShouldBe("http://localhost:8001/");
        }
    }
}
