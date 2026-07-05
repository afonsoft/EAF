using System;
using Abp.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    /// <summary>
    /// Testes BDD para tipos internos da camada Application (GoogleProvider, MiddlewareCustomDtoMapper)
    /// seguindo o padrão Dado/Quando/Então. Como são internos, usam-se reflexão para validação.
    /// </summary>
    public class InternalTypesBddTests
    {
        private static readonly System.Reflection.Assembly ApplicationAssembly =
            typeof(MiddlewareAppServiceBase).Assembly;

        [Fact]
        public void Dado_TipoGoogleProvider_Quando_ObterViaReflexao_Entao_DeveSerSettingProvider()
        {
            var tipo = ApplicationAssembly.GetType("Eaf.Middleware.Configuration.GoogleProvider");

            tipo.ShouldNotBeNull();
            typeof(SettingProvider).IsAssignableFrom(tipo).ShouldBeTrue();
        }

        [Fact]
        public void Dado_TipoMiddlewareCustomDtoMapper_Quando_ObterViaReflexao_Entao_DeveSerEstaticoComCreateMappings()
        {
            var tipo = ApplicationAssembly.GetType("Eaf.Middleware.MiddlewareCustomDtoMapper");

            tipo.ShouldNotBeNull();
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
            tipo.GetMethod("CreateMappings").ShouldNotBeNull();
        }
    }
}
