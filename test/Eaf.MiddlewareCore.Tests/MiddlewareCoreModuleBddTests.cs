using Abp.Modules;
using Eaf.Middleware;
using Shouldly;
using System.Linq;
using Xunit;

namespace Eaf.MiddlewareCore.Tests
{
    /// <summary>
    /// Testes BDD para MiddlewareCoreModule seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareCoreModuleBddTests
    {
        #region Modulo

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_VerificarHeranca_Entao_DeveSerAbpModule()
        {
            typeof(AbpModule).IsAssignableFrom(typeof(MiddlewareCoreModule)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_VerificarDependsOn_Entao_DeveConterDependencias()
        {
            var attrs = typeof(MiddlewareCoreModule)
                .GetCustomAttributes(typeof(DependsOnAttribute), false)
                .Cast<DependsOnAttribute>()
                .ToList();

            attrs.ShouldNotBeEmpty();
        }

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_VerificarDependsOn_Entao_DeveDependerAbpZeroCoreModule()
        {
            var attrs = typeof(MiddlewareCoreModule)
                .GetCustomAttributes(typeof(DependsOnAttribute), false)
                .Cast<DependsOnAttribute>()
                .SelectMany(a => a.DependedModuleTypes)
                .ToList();

            attrs.ShouldContain(typeof(Abp.Zero.AbpZeroCoreModule));
        }

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            typeof(MiddlewareCoreModule).GetConstructors().Length.ShouldBeGreaterThan(0);
        }

        #endregion
    }
}
