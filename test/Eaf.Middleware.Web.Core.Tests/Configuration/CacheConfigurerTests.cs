using Eaf.Middleware.Web.Configuration;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    /// <summary>
    /// Testes para CacheConfigurer — Spec 81 (SRP extraction).
    /// </summary>
    public class CacheConfigurerTests
    {
        #region Estrutura da Classe

        [Fact]
        public void Dado_CacheConfigurer_Quando_VerificarVisibilidade_Entao_DeveSerInternal()
        {
            // Dado & Quando
            var type = typeof(CacheConfigurer);

            // Então
            type.IsPublic.ShouldBeFalse("CacheConfigurer deve ser internal");
            type.IsNotPublic.ShouldBeTrue();
        }

        [Fact]
        public void Dado_CacheConfigurer_Quando_VerificarEstatico_Entao_DeveSerStatic()
        {
            // Dado & Quando
            var type = typeof(CacheConfigurer);

            // Então
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_CacheConfigurer_Quando_VerificarMetodoConfigure_Entao_DeveExistirComTresParametros()
        {
            // Dado
            var type = typeof(CacheConfigurer);

            // Quando
            var configureMethod = type.GetMethod("Configure", BindingFlags.Public | BindingFlags.Static);

            // Então
            configureMethod.ShouldNotBeNull();
            configureMethod.IsStatic.ShouldBeTrue();
            configureMethod.GetParameters().Length.ShouldBe(3);
        }

        [Fact]
        public void Dado_CacheConfigurer_Quando_VerificarMetodosPrivados_Entao_DeveConterIsRedisEnabled()
        {
            // Dado
            var type = typeof(CacheConfigurer);

            // Quando
            var method = type.GetMethod("IsRedisEnabled", BindingFlags.NonPublic | BindingFlags.Static);

            // Então
            method.ShouldNotBeNull();
            method.ReturnType.ShouldBe(typeof(bool));
        }

        [Fact]
        public void Dado_CacheConfigurer_Quando_VerificarMetodosPrivados_Entao_DeveConterIsSqlServerCacheEnabled()
        {
            // Dado
            var type = typeof(CacheConfigurer);

            // Quando
            var method = type.GetMethod("IsSqlServerCacheEnabled", BindingFlags.NonPublic | BindingFlags.Static);

            // Então
            method.ShouldNotBeNull();
            method.ReturnType.ShouldBe(typeof(bool));
        }

        #endregion
    }
}
