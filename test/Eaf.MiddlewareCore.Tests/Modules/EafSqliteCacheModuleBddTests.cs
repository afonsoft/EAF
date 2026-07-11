using Abp.Dependency;
using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Modules
{
    public class EafSqliteCacheModuleBddTests
    {
        [Fact]
        public void Dado_EafSqliteCacheModule_Quando_PreInitialize_Entao_DeveRegistrarSqliteCacheOptions()
        {
            var iocManager = new IocManager();
            var module = new EafSqliteCacheModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();

            iocManager.Resolve<EafSqliteCacheOptions>().ShouldNotBeNull();
            iocManager.IsRegistered<EafSqliteCacheOptions>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EafSqliteCacheModule_Quando_Initialize_Entao_DeveRegistrarAssembly()
        {
            var iocManager = new IocManager();
            var module = new EafSqliteCacheModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();
            module.Initialize();

            iocManager.IsRegistered<EafSqliteCacheOptions>().ShouldBeTrue();
        }

        private static void DefinirIocManager(Abp.Modules.AbpModule module, IIocManager iocManager)
        {
            var property = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
            property.ShouldNotBeNull();
            property.SetValue(module, iocManager);
        }
    }
}
