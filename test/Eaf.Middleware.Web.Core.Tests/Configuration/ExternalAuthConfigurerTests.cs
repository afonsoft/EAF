using Eaf.Middleware.Web.Configuration;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    /// <summary>
    /// Testes para ExternalAuthConfigurer — Spec 81 (SRP extraction).
    /// </summary>
    public class ExternalAuthConfigurerTests
    {
        [Fact]
        public void Dado_ExternalAuthConfigurer_Quando_VerificarVisibilidade_Entao_DeveSerInternal()
        {
            // Dado & Quando
            var type = typeof(ExternalAuthConfigurer);

            // Então
            type.IsPublic.ShouldBeFalse("ExternalAuthConfigurer deve ser internal");
        }

        [Fact]
        public void Dado_ExternalAuthConfigurer_Quando_VerificarEstatico_Entao_DeveSerStatic()
        {
            // Dado & Quando
            var type = typeof(ExternalAuthConfigurer);

            // Então
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ExternalAuthConfigurer_Quando_VerificarMetodoConfigure_Entao_DeveExistirComUmParametro()
        {
            // Dado
            var type = typeof(ExternalAuthConfigurer);

            // Quando
            var configureMethod = type.GetMethod("Configure", BindingFlags.Public | BindingFlags.Static);

            // Então
            configureMethod.ShouldNotBeNull();
            configureMethod.IsStatic.ShouldBeTrue();
            configureMethod.GetParameters().Length.ShouldBe(1);
        }

        [Fact]
        public void Dado_ExternalAuthConfigurer_Quando_VerificarParametroConfigure_Entao_DeveSerIIocManager()
        {
            // Dado
            var type = typeof(ExternalAuthConfigurer);
            var configureMethod = type.GetMethod("Configure", BindingFlags.Public | BindingFlags.Static);

            // Quando
            var parameters = configureMethod?.GetParameters();

            // Então
            parameters.ShouldNotBeNull();
            parameters.Length.ShouldBe(1);
            parameters[0].ParameterType.Name.ShouldBe("IIocManager");
        }
    }
}
