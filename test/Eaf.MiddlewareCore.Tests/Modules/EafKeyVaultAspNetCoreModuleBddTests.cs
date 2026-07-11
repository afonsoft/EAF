using Abp.Dependency;
using Abp.Modules;
using Eaf.KeyVault;
using Eaf.KeyVault.AspNetCore;
using Shouldly;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Modules
{
    public class EafKeyVaultAspNetCoreModuleBddTests
    {
        [Fact]
        public void Dado_EafKeyVaultAspNetCoreModule_Quando_VerificarDependencias_Entao_DeveDependerDoEafKeyVaultModule()
        {
            var dependsOn = typeof(EafKeyVaultAspNetCoreModule).GetCustomAttributes(typeof(DependsOnAttribute), false)
                .Cast<DependsOnAttribute>()
                .SelectMany(a => a.DependedModuleTypes)
                .ToList();

            dependsOn.ShouldContain(typeof(EafKeyVaultModule));
        }

        [Fact]
        public void Dado_EafKeyVaultAspNetCoreModule_Quando_Initialize_Entao_DeveRegistrarAssemblySemExcecoes()
        {
            var iocManager = new IocManager();
            var module = new EafKeyVaultAspNetCoreModule();
            DefinirIocManager(module, iocManager);

            Should.NotThrow(() => module.Initialize());
        }

        private static void DefinirIocManager(Abp.Modules.AbpModule module, IIocManager iocManager)
        {
            var property = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
            property.ShouldNotBeNull();
            property.SetValue(module, iocManager);
        }
    }
}
