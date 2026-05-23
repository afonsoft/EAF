using Abp.UI;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authorization
{
    public class ExternalLoginProviderSettingsTests
    {
        [Fact]
        public void GoogleExternalLoginProviderSettings_IsValid_TrueWhenClientIdAndSecret()
        {
            var s = new GoogleExternalLoginProviderSettings { ClientId = "c", ClientSecret = "s", UserInfoEndpoint = "u" };
            s.IsValid().ShouldBeTrue();
            s.ClientId.ShouldBe("c");
            s.ClientSecret.ShouldBe("s");
            s.UserInfoEndpoint.ShouldBe("u");
        }

        [Fact]
        public void GoogleExternalLoginProviderSettings_IsValid_FalseWhenMissing()
        {
            new GoogleExternalLoginProviderSettings().IsValid().ShouldBeFalse();
            new GoogleExternalLoginProviderSettings { ClientId = "c" }.IsValid().ShouldBeFalse();
            new GoogleExternalLoginProviderSettings { ClientSecret = "s" }.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void MicrosoftExternalLoginProviderSettings_IsValid()
        {
            new MicrosoftExternalLoginProviderSettings().IsValid().ShouldBeFalse();
            var s = new MicrosoftExternalLoginProviderSettings { ClientId = "c", ClientSecret = "s", TenantId = "t" };
            s.IsValid().ShouldBeTrue();
            s.TenantId.ShouldBe("t");
        }

        [Fact]
        public void OpenIdConnectExternalLoginProviderSettings_ValidConfig()
        {
            var s = new OpenIdConnectExternalLoginProviderSettings
            {
                Authority = "https://accounts.google.com",
                ClientId = "c",
                ClientSecret = "s",
                LoginUrl = "lurl",
                ValidateIssuer = true
            };
            s.IsValid().ShouldBeTrue();
            s.Authority.ShouldBe("https://accounts.google.com");
            s.ClientId.ShouldBe("c");
            s.ClientSecret.ShouldBe("s");
            s.LoginUrl.ShouldBe("lurl");
            s.ValidateIssuer.ShouldBeTrue();
        }

        [Fact]
        public void OpenIdConnectExternalLoginProviderSettings_ThrowsWhenAuthorityNotHttps()
        {
            var s = new OpenIdConnectExternalLoginProviderSettings
            {
                Authority = "http://insecure",
                ClientId = "c"
            };
            Should.Throw<UserFriendlyException>(() => s.IsValid());
        }

        [Fact]
        public void AuthZeroExternalLoginProviderSettings_IsValid()
        {
            new AuthZeroExternalLoginProviderSettings().IsValid().ShouldBeFalse();
            var s = new AuthZeroExternalLoginProviderSettings { ClientId = "c", ClientSecret = "s", Endpoint = "e" };
            s.IsValid().ShouldBeTrue();
            s.Endpoint.ShouldBe("e");
        }

        [Fact]
        public void JsonClaimMap_ShouldSet()
        {
            var m = new JsonClaimMap { Claim = "c", Key = "k" };
            m.Claim.ShouldBe("c");
            m.Key.ShouldBe("k");
        }

        [Fact]
        public void JsonClaimMapDto_ShouldSet()
        {
            var m = new JsonClaimMapDto { Claim = "c", Key = "k" };
            m.Claim.ShouldBe("c");
            m.Key.ShouldBe("k");
        }

        [Fact]
        public void ExternalLoginProviderInfo_CtorAndDefaults()
        {
            var info = new ExternalLoginProviderInfo("Google", "c", "s", "t", typeof(string));
            info.Name.ShouldBe("Google");
            info.ClientId.ShouldBe("c");
            info.ClientSecret.ShouldBe("s");
            info.TenantId.ShouldBe("t");
            info.ProviderApiType.ShouldBe(typeof(string));
            info.AdditionalParams.ShouldNotBeNull();
            info.AdditionalParams.Count.ShouldBe(0);
            info.ClaimMappings.ShouldNotBeNull();
            info.ClaimMappings.Count.ShouldBe(0);
        }

        [Fact]
        public void ExternalLoginProviderInfo_Ctor_WithAdditionalData()
        {
            var additional = new Dictionary<string, string> { ["k"] = "v" };
            var claims = new List<JsonClaimMap> { new JsonClaimMap { Claim = "c", Key = "k" } };
            var info = new ExternalLoginProviderInfo("N", "c", "s", "t", typeof(string), additional, claims);
            info.AdditionalParams.Count.ShouldBe(1);
            info.ClaimMappings.Count.ShouldBe(1);
        }

        [Fact]
        public void ExternalAuthUserInfo_ShouldSet()
        {
            var info = new ExternalAuthUserInfo
            {
                EmailAddress = "a@b.com",
                Name = "n",
                Provider = "g",
                ProviderKey = "pk",
                Surname = "s",
                Picture = "p",
                AccessCode = "ac"
            };
            info.EmailAddress.ShouldBe("a@b.com");
            info.Name.ShouldBe("n");
            info.Provider.ShouldBe("g");
            info.ProviderKey.ShouldBe("pk");
            info.Surname.ShouldBe("s");
            info.Picture.ShouldBe("p");
            info.AccessCode.ShouldBe("ac");
        }
    }
}
