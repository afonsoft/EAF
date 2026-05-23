using Eaf.Middleware.Configuration;
using Eaf.Middleware.Web.Url;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Url
{
    public class WebUrlServiceBaseTests
    {
        private class TestWebUrlService : WebUrlServiceBase
        {
            public TestWebUrlService(IAppConfigurationAccessor cfg) : base(cfg) { }
            public override string ServerRootAddressFormatKey => "App:ServerRootAddress";
            public override string WebSiteRootAddressFormatKey => "App:ClientRootAddress";
        }

        private TestWebUrlService Build(Dictionary<string, string> values)
        {
            var cfgRoot = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
            var accessor = Substitute.For<IAppConfigurationAccessor>();
            accessor.Configuration.Returns(cfgRoot);
            return new TestWebUrlService(accessor);
        }

        [Fact]
        public void GetServerRootAddress_UsesDefaults_WhenMissing()
        {
            var svc = Build(new Dictionary<string, string>());
            svc.GetServerRootAddress().ShouldBe("http://localhost:8001/");
            svc.GetSiteRootAddress().ShouldBe("http://localhost:8000/");
        }

        [Fact]
        public void GetServerRootAddress_UsesConfig_WhenPresent()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:ServerRootAddress"] = "https://api.example.com/",
                ["App:ClientRootAddress"] = "https://app.example.com/"
            });
            svc.GetServerRootAddress().ShouldBe("https://api.example.com/");
            svc.GetSiteRootAddress().ShouldBe("https://app.example.com/");
        }

        [Fact]
        public void SupportsTenancyNameInUrl_TrueWhenFormatHasPlaceholder()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:ClientRootAddress"] = "https://{TENANCY_NAME}.example.com/"
            });
            svc.SupportsTenancyNameInUrl.ShouldBeTrue();
        }

        [Fact]
        public void SupportsTenancyNameInUrl_FalseWhenNoPlaceholder()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:ClientRootAddress"] = "https://example.com/"
            });
            svc.SupportsTenancyNameInUrl.ShouldBeFalse();
        }

        [Fact]
        public void GetSiteRootAddress_ReplacesTenancyName()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:ClientRootAddress"] = "https://{TENANCY_NAME}.example.com/"
            });
            svc.GetSiteRootAddress("acme").ShouldBe("https://acme.example.com/");
        }

        [Fact]
        public void GetSiteRootAddress_EmptyTenancyRemovesPlaceholder()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:ClientRootAddress"] = "https://{TENANCY_NAME}.example.com/"
            });
            svc.GetSiteRootAddress().ShouldBe("https://example.com/");
        }

        [Fact]
        public void GetRedirectAllowedExternalWebSites_ReturnsEmptyWhenUnset()
        {
            var svc = Build(new Dictionary<string, string>());
            svc.GetRedirectAllowedExternalWebSites().ShouldBeEmpty();
        }

        [Fact]
        public void GetRedirectAllowedExternalWebSites_SplitsOnComma()
        {
            var svc = Build(new Dictionary<string, string>
            {
                ["App:RedirectAllowedExternalWebSites"] = "a.com,b.com,c.com"
            });
            var list = svc.GetRedirectAllowedExternalWebSites();
            list.Count.ShouldBe(3);
            list.ShouldContain("a.com");
            list.ShouldContain("b.com");
            list.ShouldContain("c.com");
        }
    }
}
