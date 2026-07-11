using Abp.Dependency;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.SqlServer;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Modules
{
    public class EafSqlServerCacheModuleBddTests
    {
        [Fact]
        public void Dado_EafSqlServerCacheModule_Quando_PreInitialize_Entao_DeveRegistrarSqlServerCacheOptions()
        {
            var iocManager = new IocManager();
            var module = new EafSqlServerCacheModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();

            iocManager.Resolve<SqlServerCacheOptions>().ShouldNotBeNull();
            iocManager.IsRegistered<SqlServerCacheOptions>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EafSqlServerCacheModule_Quando_Initialize_Entao_DeveRegistrarAssembly()
        {
            var iocManager = new IocManager();
            var module = new EafSqlServerCacheModule();
            DefinirIocManager(module, iocManager);

            module.PreInitialize();
            module.Initialize();

            iocManager.IsRegistered<SqlServerCacheOptions>().ShouldBeTrue();
        }

        private static void DefinirIocManager(Abp.Modules.AbpModule module, IIocManager iocManager)
        {
            var property = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
            property.ShouldNotBeNull();
            property.SetValue(module, iocManager);
        }
    }
}
