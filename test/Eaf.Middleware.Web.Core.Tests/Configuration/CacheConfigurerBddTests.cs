using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Eaf.Middleware.Web.Configuration;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class CacheConfigurerBddTests
    {
        [Fact]
        public void Dado_ConfiguracaoSemRedisESemSqlServer_Quando_Configure_Entao_DeveAplicarSlidingExpireTime()
        {
            var (configuration, iocManager) = CriarDependencias();
            var appConfiguration = CriarAppConfiguration(new Dictionary<string, string?>());
            var cacheOptions = Substitute.For<ICacheOptions>();
            var cachingConfiguration = configuration.Caching;
            cachingConfiguration.ConfigureAll(Arg.Do<Action<ICacheOptions>>(action => action(cacheOptions)));

            CacheConfigurer.Configure(configuration, appConfiguration, iocManager);

            cacheOptions.Received(1).DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
        }

        [Fact]
        public void Dado_RedisHabilitado_Quando_Configure_Entao_DeveConfigurarRedis()
        {
            var (configuration, iocManager) = CriarDependencias();
            var appConfiguration = CriarAppConfiguration(new Dictionary<string, string?>
            {
                { "RedisCache:IsEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" },
                { "RedisCache:DatabaseId", "1" }
            });

            var cacheOptions = Substitute.For<ICacheOptions>();
            var cachingConfiguration = configuration.Caching;
            cachingConfiguration.ConfigureAll(Arg.Do<Action<ICacheOptions>>(action => action(cacheOptions)));

            CacheConfigurer.Configure(configuration, appConfiguration, iocManager);

            cacheOptions.Received(1).DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            iocManager.IsRegistered<Abp.Runtime.Caching.Redis.AbpRedisCacheOptions>().ShouldBeTrue();
            iocManager.IsRegistered<Abp.Runtime.Caching.Redis.IAbpPerRequestRedisCache>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_SqlServerCacheHabilitado_Quando_Configure_Entao_DeveConfigurarSqlServerCache()
        {
            var (configuration, iocManager) = CriarDependencias();
            var appConfiguration = CriarAppConfiguration(new Dictionary<string, string?>
            {
                { "SqlServerCache:IsEnabled", "true" },
                { "SqlServerCache:ConnectionString", "Server=.;Database=Cache;" },
                { "SqlServerCache:SchemaName", "dbo" },
                { "SqlServerCache:TableName", "CustomCache" }
            });

            var cacheOptions = Substitute.For<ICacheOptions>();
            var cachingConfiguration = configuration.Caching;
            cachingConfiguration.ConfigureAll(Arg.Do<Action<ICacheOptions>>(action => action(cacheOptions)));

            CacheConfigurer.Configure(configuration, appConfiguration, iocManager);

            cacheOptions.Received(1).DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            iocManager.Resolve<SqlServerCacheOptions>().ConnectionString.ShouldBe("Server=.;Database=Cache;");
            iocManager.Resolve<SqlServerCacheOptions>().SchemaName.ShouldBe("dbo");
            iocManager.Resolve<SqlServerCacheOptions>().TableName.ShouldBe("CustomCache");
        }

        [Fact]
        public void Dado_SqlServerCacheComDefaultConnection_Quando_Configure_Entao_DeveUsarConnectionStringPadrao()
        {
            var (configuration, iocManager) = CriarDependencias();
            configuration.DefaultNameOrConnectionString.Returns("Server=.;Database=EafDefault;");
            var appConfiguration = CriarAppConfiguration(new Dictionary<string, string?>
            {
                { "SqlServerCache:IsEnabled", "true" }
            });

            var cacheOptions = Substitute.For<ICacheOptions>();
            var cachingConfiguration = configuration.Caching;
            cachingConfiguration.ConfigureAll(Arg.Do<Action<ICacheOptions>>(action => action(cacheOptions)));

            CacheConfigurer.Configure(configuration, appConfiguration, iocManager);

            iocManager.Resolve<SqlServerCacheOptions>().ConnectionString.ShouldBe("Server=.;Database=EafDefault;");
            iocManager.Resolve<SqlServerCacheOptions>().TableName.ShouldBe("EafCache");
        }

        [Fact]
        public void Dado_RedisESqlServerHabilitados_Quando_Configure_Entao_DeveConfigurarAmbos()
        {
            var (configuration, iocManager) = CriarDependencias();
            var appConfiguration = CriarAppConfiguration(new Dictionary<string, string?>
            {
                { "RedisCache:IsEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" },
                { "SqlServerCache:IsEnabled", "true" },
                { "SqlServerCache:ConnectionString", "Server=.;Database=Cache;" }
            });

            var cacheOptions = Substitute.For<ICacheOptions>();
            var cachingConfiguration = configuration.Caching;
            cachingConfiguration.ConfigureAll(Arg.Do<Action<ICacheOptions>>(action => action(cacheOptions)));

            CacheConfigurer.Configure(configuration, appConfiguration, iocManager);

            iocManager.IsRegistered<Abp.Runtime.Caching.Redis.AbpRedisCacheOptions>().ShouldBeTrue();
            iocManager.Resolve<SqlServerCacheOptions>().ConnectionString.ShouldBe("Server=.;Database=Cache;");
        }

        private static (IAbpStartupConfiguration configuration, IIocManager iocManager) CriarDependencias()
        {
            var iocManager = new IocManager();
            iocManager.Register<SqlServerCacheOptions>();

            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var cachingConfiguration = Substitute.For<ICachingConfiguration>();
            configuration.Caching.Returns(cachingConfiguration);
            configuration.IocManager.Returns(iocManager);
            configuration.DefaultNameOrConnectionString.Returns(string.Empty);
            cachingConfiguration.AbpConfiguration.Returns(configuration);

            return (configuration, iocManager);
        }

        private static IConfigurationRoot CriarAppConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        }
    }
}
