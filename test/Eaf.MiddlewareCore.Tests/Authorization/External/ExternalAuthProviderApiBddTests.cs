using Castle.Core.Logging;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.AuthZero;
using Eaf.Middleware.Core.Authentication.External.Google;
using Eaf.Middleware.Core.Authentication.External.Microsoft;
using Eaf.Middleware.Core.Authentication.External.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    public class ExternalAuthProviderApiBddTests
    {
        [Fact]
        public async Task Dado_GoogleProviderConfigurado_Quando_GetUserInfo_Entao_DeveRetornarDadosMapeados()
        {
            var handler = CriarHandler("https://google.com/userinfo", "{\"id\":\"123\",\"name\":\"Alice Smith\",\"given_name\":\"Alice\",\"family_name\":\"Smith\",\"email\":\"alice@example.com\"}");
            var factory = CriarHttpClientFactory(handler);
            var api = new GoogleAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Google", typeof(GoogleAuthProviderApi), new Dictionary<string, string>
            {
                { "UserInfoEndpoint", "https://google.com/userinfo" }
            });

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("Google");
            result.ProviderKey.ShouldBe("123");
            result.Name.ShouldBe("Alice Smith");
            result.Surname.ShouldBe("Smith");
            result.EmailAddress.ShouldBe("alice@example.com");
            result.AccessCode.ShouldBe("access-token");
        }

        [Fact]
        public async Task Dado_GoogleProviderSemEndpoint_Quando_GetUserInfo_Entao_DeveLancarExcecao()
        {
            var factory = CriarHttpClientFactory(new TestHttpMessageHandler());
            var api = new GoogleAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Google", typeof(GoogleAuthProviderApi), new Dictionary<string, string>
            {
                { "UserInfoEndpoint", string.Empty }
            });

            await Assert.ThrowsAsync<Abp.AbpException>(() => api.GetUserInfo("access-token"));
        }

        [Fact]
        public async Task Dado_MicrosoftProviderConfigurado_Quando_GetUserInfo_Entao_DeveRetornarDadosMapeados()
        {
            var handler = CriarHandler(
                MicrosoftAccountDefaults.UserInformationEndpoint,
                "{\"id\":\"456\",\"displayName\":\"Bob Jones\",\"surname\":\"Jones\",\"mail\":\"bob@example.com\"}",
                ("https://graph.microsoft.com/v1.0/me/photo/$value", HttpStatusCode.NotFound, string.Empty));
            var factory = CriarHttpClientFactory(handler);
            var api = new MicrosoftAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Microsoft", typeof(MicrosoftAuthProviderApi), new Dictionary<string, string>());

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("Microsoft");
            result.ProviderKey.ShouldBe("456");
            result.Name.ShouldBe("Bob Jones");
            result.Surname.ShouldBe("Jones");
            result.EmailAddress.ShouldBe("bob@example.com");
            result.AccessCode.ShouldBe("access-token");
        }

        [Fact]
        public async Task Dado_AuthZeroProviderConfigurado_Quando_GetUserInfo_Entao_DeveRetornarDadosMapeados()
        {
            var handler = CriarHandler(
                "https://authzero.example.com/userinfo",
                "{\"sub\":\"789\",\"name\":\"Carol Doe\",\"given_name\":\"Carol\",\"family_name\":\"Doe\",\"email\":\"carol@example.com\",\"picture\":\"https://authzero.example.com/photo.png\"}",
                ("https://authzero.example.com/photo.png", HttpStatusCode.NotFound, string.Empty));
            var factory = CriarHttpClientFactory(handler);
            var api = new AuthZeroAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("AuthZero", typeof(AuthZeroAuthProviderApi), new Dictionary<string, string>
            {
                { "Endpoint", "authzero.example.com" }
            });

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("AuthZero");
            result.ProviderKey.ShouldBe("789");
            result.Name.ShouldBe("Carol Doe");
            result.Surname.ShouldBe("Doe");
            result.EmailAddress.ShouldBe("carol@example.com");
            result.AccessCode.ShouldBe("access-token");
        }

        [Fact]
        public async Task Dado_AuthZeroProviderComEndpointHttp_Quando_GetUserInfo_Entao_DeveNormalizarDominio()
        {
            var handler = CriarHandler(
                "https://authzero.example.com/userinfo",
                "{\"sub\":\"abc\",\"name\":\"Dan\",\"email\":\"dan@example.com\"}");
            var factory = CriarHttpClientFactory(handler);
            var api = new AuthZeroAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("AuthZero", typeof(AuthZeroAuthProviderApi), new Dictionary<string, string>
            {
                { "Endpoint", "https://authzero.example.com/" }
            });

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("AuthZero");
            result.ProviderKey.ShouldBe("abc");
            result.AccessCode.ShouldBe("access-token");
        }

        [Fact]
        public async Task Dado_OpenIdConnectSemAuthority_Quando_GetUserInfo_Entao_DeveLancarExcecao()
        {
            var api = new OpenIdConnectAuthProviderApi(NullLogger.Instance);
            api.ProviderInfo = CriarProviderInfo("OpenIdConnect", typeof(OpenIdConnectAuthProviderApi), new Dictionary<string, string>
            {
                { "Authority", string.Empty }
            });

            await Assert.ThrowsAsync<ApplicationException>(() => api.GetUserInfo("access-token"));
        }

        [Fact]
        public async Task Dado_OpenIdConnectAuthorityAusente_Quando_GetUserInfo_Entao_DeveLancarKeyNotFoundException()
        {
            var api = new OpenIdConnectAuthProviderApi(NullLogger.Instance);
            api.ProviderInfo = CriarProviderInfo("OpenIdConnect", typeof(OpenIdConnectAuthProviderApi), new Dictionary<string, string>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => api.GetUserInfo("access-token"));
        }

        [Fact]
        public async Task Dado_OpenIdConnectComTokenNulo_Quando_GetUserInfo_Entao_DeveLancarArgumentNullException()
        {
            var api = new OpenIdConnectAuthProviderApi(NullLogger.Instance);
            api.ProviderInfo = CriarProviderInfo("OpenIdConnect", typeof(OpenIdConnectAuthProviderApi), new Dictionary<string, string>
            {
                { "Authority", "http://localhost:1" },
                { "ValidateIssuer", "false" }
            });

            await Assert.ThrowsAsync<ArgumentNullException>(() => api.GetUserInfo(null));
        }

        [Fact]
        public async Task Dado_MicrosoftProviderConfiguradoComFoto_Quando_GetUserInfo_Entao_DevePreencherPicture()
        {
            var handler = CriarHandler(
                MicrosoftAccountDefaults.UserInformationEndpoint,
                "{\"id\":\"456\",\"displayName\":\"Bob Jones\",\"surname\":\"Jones\",\"mail\":\"bob@example.com\"}",
                ("https://graph.microsoft.com/v1.0/me/photo/$value", HttpStatusCode.OK, "fake-image"));
            var factory = CriarHttpClientFactory(handler);
            var api = new MicrosoftAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Microsoft", typeof(MicrosoftAuthProviderApi), new Dictionary<string, string>());

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("Microsoft");
            result.Picture.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_MicrosoftProviderComErroNaFoto_Quando_GetUserInfo_Entao_DeveRetornarDadosSemPicture()
        {
            var handler = CriarHandler(
                MicrosoftAccountDefaults.UserInformationEndpoint,
                "{\"id\":\"456\",\"displayName\":\"Bob Jones\",\"surname\":\"Jones\",\"mail\":\"bob@example.com\"}");
            handler.AddException("https://graph.microsoft.com/v1.0/me/photo/$value", new HttpRequestException("photo error"));

            var factory = CriarHttpClientFactory(handler);
            var api = new MicrosoftAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Microsoft", typeof(MicrosoftAuthProviderApi), new Dictionary<string, string>());

            var result = await api.GetUserInfo("access-token");

            result.Provider.ShouldBe("Microsoft");
            result.Picture.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_ProviderApi_Quando_IsValidUserComProviderKeyCorrespondente_Entao_DeveRetornarVerdadeiro()
        {
            var handler = CriarHandler("https://google.com/userinfo", "{\"id\":\"123\",\"name\":\"Alice Smith\",\"given_name\":\"Alice\",\"family_name\":\"Smith\",\"email\":\"alice@example.com\"}");
            var factory = CriarHttpClientFactory(handler);
            var api = new GoogleAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Google", typeof(GoogleAuthProviderApi), new Dictionary<string, string>
            {
                { "UserInfoEndpoint", "https://google.com/userinfo" }
            });

            var result = await api.IsValidUser("123", "access-token");

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ProviderApi_Quando_IsValidUserComProviderKeyDiferente_Entao_DeveRetornarFalso()
        {
            var handler = CriarHandler("https://google.com/userinfo", "{\"id\":\"123\",\"name\":\"Alice Smith\",\"email\":\"alice@example.com\"}");
            var factory = CriarHttpClientFactory(handler);
            var api = new GoogleAuthProviderApi(NullLogger.Instance, factory);
            api.ProviderInfo = CriarProviderInfo("Google", typeof(GoogleAuthProviderApi), new Dictionary<string, string>
            {
                { "UserInfoEndpoint", "https://google.com/userinfo" }
            });

            var result = await api.IsValidUser("999", "access-token");

            result.ShouldBeFalse();
        }

        private static TestHttpMessageHandler CriarHandler(string expectedUri, string successContent, params (string uri, HttpStatusCode status, string content)[] additionalResponses)
        {
            var handler = new TestHttpMessageHandler();
            handler.AddResponse(expectedUri, successContent, HttpStatusCode.OK);
            foreach (var (uri, status, content) in additionalResponses)
            {
                handler.AddResponse(uri, content ?? string.Empty, status);
            }
            return handler;
        }

        private static IHttpClientFactory CriarHttpClientFactory(HttpMessageHandler handler)
        {
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("ExternalAuth").Returns(new HttpClient(handler));
            return factory;
        }

        private static ExternalLoginProviderInfo CriarProviderInfo(string name, Type providerApiType, Dictionary<string, string> additionalParams)
        {
            return new ExternalLoginProviderInfo(
                name,
                "client-id",
                "client-secret",
                null,
                providerApiType,
                additionalParams);
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Dictionary<string, (string content, HttpStatusCode status)> _responses = new Dictionary<string, (string, HttpStatusCode)>();
            private readonly Dictionary<string, Exception> _exceptions = new Dictionary<string, Exception>();

            public void AddResponse(string uri, string content, HttpStatusCode status)
            {
                _responses[uri] = (content, status);
            }

            public void AddException(string uri, Exception exception)
            {
                _exceptions[uri] = exception;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var key = request.RequestUri?.ToString() ?? string.Empty;
                if (_exceptions.TryGetValue(key, out var exception))
                {
                    throw exception;
                }

                if (_responses.TryGetValue(key, out var response))
                {
                    var message = new HttpResponseMessage(response.status)
                    {
                        Content = new StringContent(response.content, Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(message);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }
    }
}
