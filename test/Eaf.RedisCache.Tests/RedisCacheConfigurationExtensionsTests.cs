using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Eaf.Runtime.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes para as extensões de configuração do cache Redis.
    /// </summary>
    public class RedisCacheConfigurationExtensionsTests
    {
        private readonly ICachingConfiguration _cachingConfiguration;
        private readonly IIocManager _iocManager;
        private readonly IAbpStartupConfiguration _abpConfiguration;

        public RedisCacheConfigurationExtensionsTests()
        {
            _iocManager = Substitute.For<IIocManager>();
            _abpConfiguration = Substitute.For<IAbpStartupConfiguration>();
            _cachingConfiguration = Substitute.For<ICachingConfiguration>();

            _cachingConfiguration.AbpConfiguration.Returns(_abpConfiguration);
            _abpConfiguration.IocManager.Returns(_iocManager);
        }

        [Fact]
        public void UseRedis_WithoutOptions_ShouldRegisterCacheManager()
        {
            // Act
            _cachingConfiguration.UseRedis();

            // Assert
            _iocManager.Received(1).RegisterIfNot<IDistributedCache, global::Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache>();
            _iocManager.Received(1).RegisterIfNot<ICacheManager, EafRedisCacheManager>();
        }

        [Fact]
        public void UseRedis_WithOptions_ShouldRegisterCacheManagerAndConfigureOptions()
        {
            // Arrange
            var options = new EafRedisCacheOptions();
            _iocManager.Resolve<EafRedisCacheOptions>().Returns(options);
            var optionsCalled = false;

            // Act
            _cachingConfiguration.UseRedis(opt =>
            {
                optionsCalled = true;
                opt.ConnectionString = "localhost:6379";
                opt.InstanceName = "EAF";
            });

            // Assert
            _iocManager.Received(1).RegisterIfNot<IDistributedCache, global::Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache>();
            _iocManager.Received(1).RegisterIfNot<ICacheManager, EafRedisCacheManager>();
            _iocManager.Received(1).Resolve<EafRedisCacheOptions>();
            optionsCalled.ShouldBeTrue();
            options.ConnectionString.ShouldBe("localhost:6379");
            options.InstanceName.ShouldBe("EAF");
        }

        [Fact]
        public void UseRedis_WithNullCachingConfiguration_ShouldThrowException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => RedisCacheConfigurationExtensions.UseRedis(null));
        }

        [Fact]
        public void UseRedis_MultipleCalls_ShouldRegisterOnlyOnce()
        {
            // Act
            _cachingConfiguration.UseRedis();
            _cachingConfiguration.UseRedis();

            // Assert
            _iocManager.Received(2).RegisterIfNot<IDistributedCache, global::Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache>();
            _iocManager.Received(2).RegisterIfNot<ICacheManager, EafRedisCacheManager>();
        }
    }
}
