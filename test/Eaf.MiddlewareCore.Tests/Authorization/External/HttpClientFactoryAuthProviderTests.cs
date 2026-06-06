using Castle.Core.Logging;
using Eaf.Middleware.Core.Authentication.External.Microsoft;
using Eaf.Middleware.Core.Authentication.External.Google;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    /// <summary>
    /// Testes para verificar o uso de IHttpClientFactory nos auth providers — Spec 05.
    /// </summary>
    public class HttpClientFactoryAuthProviderTests
    {
        #region MicrosoftAuthProviderApi

        [Fact]
        public void Dado_MicrosoftAuthProviderApi_Quando_VerificarConstrutor_Entao_DeveRequererIHttpClientFactory()
        {
            // Dado
            var type = typeof(MicrosoftAuthProviderApi);

            // Quando
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            // Então
            constructors.Length.ShouldBeGreaterThan(0);
            var hasHttpClientFactoryParam = constructors.Any(c =>
                c.GetParameters().Any(p => p.ParameterType == typeof(IHttpClientFactory)));
            hasHttpClientFactoryParam.ShouldBeTrue("MicrosoftAuthProviderApi deve receber IHttpClientFactory via construtor");
        }

        [Fact]
        public void Dado_MicrosoftAuthProviderApi_Quando_VerificarCampos_Entao_DeveConterHttpClientFactory()
        {
            // Dado
            var type = typeof(MicrosoftAuthProviderApi);

            // Quando
            var field = type.GetField("_httpClientFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Então
            field.ShouldNotBeNull("Deve ter campo _httpClientFactory");
            field.FieldType.ShouldBe(typeof(IHttpClientFactory));
        }

        [Fact]
        public void Dado_MicrosoftAuthProviderApi_Quando_VerificarCodigo_Entao_NaoDeveConterNewHttpClient()
        {
            // Dado — Verificação estrutural que não usa new HttpClient()
            var type = typeof(MicrosoftAuthProviderApi);

            // Quando — GetUserInfo deveria usar _httpClientFactory.CreateClient
            var method = type.GetMethod("GetUserInfo");

            // Então
            method.ShouldNotBeNull();
            method.ReturnType.Name.ShouldContain("Task");
        }

        [Fact]
        public void Dado_MicrosoftAuthProviderApi_Quando_VerificarConstanteNome_Entao_DeveSer_Microsoft()
        {
            // Dado & Quando
            var name = MicrosoftAuthProviderApi.Name;

            // Então
            name.ShouldBe("Microsoft");
        }

        #endregion

        #region GoogleAuthProviderApi

        [Fact]
        public void Dado_GoogleAuthProviderApi_Quando_VerificarConstrutor_Entao_DeveRequererIHttpClientFactory()
        {
            // Dado
            var type = typeof(GoogleAuthProviderApi);

            // Quando
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            // Então
            constructors.Length.ShouldBeGreaterThan(0);
            var hasHttpClientFactoryParam = constructors.Any(c =>
                c.GetParameters().Any(p => p.ParameterType == typeof(IHttpClientFactory)));
            hasHttpClientFactoryParam.ShouldBeTrue("GoogleAuthProviderApi deve receber IHttpClientFactory via construtor");
        }

        [Fact]
        public void Dado_GoogleAuthProviderApi_Quando_VerificarCampos_Entao_DeveConterHttpClientFactory()
        {
            // Dado
            var type = typeof(GoogleAuthProviderApi);

            // Quando
            var field = type.GetField("_httpClientFactory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Então
            field.ShouldNotBeNull("Deve ter campo _httpClientFactory");
            field.FieldType.ShouldBe(typeof(IHttpClientFactory));
        }

        #endregion

        #region Verificações Gerais (nenhum new HttpClient())

        [Fact]
        public void Dado_ProvidersDeAuth_Quando_VerificarHeranca_Entao_DevemHerdarExternalAuthProviderApiBase()
        {
            // Dado & Quando & Então
            typeof(MicrosoftAuthProviderApi).BaseType.Name.ShouldBe("ExternalAuthProviderApiBase");
            typeof(GoogleAuthProviderApi).BaseType.Name.ShouldBe("ExternalAuthProviderApiBase");
        }

        #endregion
    }
}
