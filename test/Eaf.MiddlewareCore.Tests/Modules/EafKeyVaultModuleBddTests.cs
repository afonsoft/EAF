using Abp.Dependency;
using Eaf.KeyVault;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Modules
{
    public class EafKeyVaultModuleBddTests
    {
        [Fact]
        public void Dado_EafKeyVaultModule_Quando_PreInitialize_Entao_DeveRegistrarKeyVaultSecretManager()
        {
            var iocManager = new IocManager();
            var module = new EafKeyVaultModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();

            iocManager.IsRegistered<IKeyVaultSecretManager>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EafKeyVaultModule_Quando_Initialize_Entao_DeveRegistrarKeyVaultSecretManager()
        {
            var iocManager = new IocManager();
            var module = new EafKeyVaultModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();
            module.Initialize();

            iocManager.IsRegistered<IKeyVaultSecretManager>().ShouldBeTrue();
        }

        private static void DefinirIocManager(Abp.Modules.AbpModule module, IIocManager iocManager)
        {
            var property = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
            property.ShouldNotBeNull();
            property.SetValue(module, iocManager);
        }
    }
}
