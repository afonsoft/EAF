using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class OracleCacheConfigurationExtensionsBddTests
    {
        [Fact]
        public void Dado_CachingConfiguration_Quando_UseSqlServer_Entao_DeveRegistrarDistributedCacheECacheManager()
        {
            var iocManager = new IocManager();
            iocManager.Register<SqlServerCacheOptions>();

            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var cachingConfiguration = Substitute.For<ICachingConfiguration>();
            cachingConfiguration.AbpConfiguration.Returns(configuration);
            configuration.IocManager.Returns(iocManager);
            configuration.Caching.Returns(cachingConfiguration);

            cachingConfiguration.UseSqlServer(options => options.ConnectionString = "Server=.;Database=Cache;");

            iocManager.IsRegistered<IDistributedCache>().ShouldBeTrue();
            iocManager.IsRegistered<Abp.Runtime.Caching.ICacheManager>().ShouldBeTrue();
            iocManager.Resolve<SqlServerCacheOptions>().ConnectionString.ShouldBe("Server=.;Database=Cache;");
        }

        [Fact]
        public void Dado_CachingConfiguration_Quando_UseSqlServerComSchemaETabela_Entao_DeveConfigurarOpcoes()
        {
            var iocManager = new IocManager();
            iocManager.Register<SqlServerCacheOptions>();

            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var cachingConfiguration = Substitute.For<ICachingConfiguration>();
            cachingConfiguration.AbpConfiguration.Returns(configuration);
            configuration.IocManager.Returns(iocManager);
            configuration.Caching.Returns(cachingConfiguration);

            cachingConfiguration.UseSqlServer(options =>
            {
                options.ConnectionString = "Server=.;Database=Cache;";
                options.SchemaName = "cache";
                options.TableName = "CustomCache";
            });

            iocManager.Resolve<SqlServerCacheOptions>().SchemaName.ShouldBe("cache");
            iocManager.Resolve<SqlServerCacheOptions>().TableName.ShouldBe("CustomCache");
        }
    }
}
