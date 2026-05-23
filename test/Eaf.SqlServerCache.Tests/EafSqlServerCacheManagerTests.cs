using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Castle.Core.Logging;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.SqlServerCache.Tests
{
    public class EafSqlServerCacheManagerTests
    {
        private readonly IIocManager _iocManager;
        private readonly ICachingConfiguration _configuration;

        public EafSqlServerCacheManagerTests()
        {
            _iocManager = Substitute.For<IIocManager>();
            _configuration = Substitute.For<ICachingConfiguration>();
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);

            // Assert
            manager.ShouldNotBeNull();
            manager.Logger.ShouldNotBeNull();
            _iocManager.Received(1).RegisterIfNot<EafSqlServerCache>(DependencyLifeStyle.Transient);
        }

        [Fact]
        public void Logger_CanBeSet()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var logger = Substitute.For<ILogger>();

            // Act
            manager.Logger = logger;

            // Assert
            manager.Logger.ShouldBe(logger);
        }

        [Fact]
        public void GetCache_ShouldCallIocManagerResolve()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var cacheName = "TestCache";

            // Create a mock that can be returned as EafSqlServerCache
            var mockDistributedCache = Substitute.For<IDistributedCache>();
            var realCache = new EafSqlServerCache(cacheName, mockDistributedCache);
            _iocManager.Resolve<EafSqlServerCache>(Arg.Any<object>()).Returns(realCache);

            // Act
            var cache = manager.GetCache(cacheName);

            // Assert
            cache.ShouldNotBeNull();
            cache.Name.ShouldBe(cacheName);
            _iocManager.Received(1).Resolve<EafSqlServerCache>(Arg.Any<object>());
        }

        [Fact]
        public void GetCache_WithSameName_ShouldReturnSameInstance()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var cacheName = "TestCache";

            var mockDistributedCache = Substitute.For<IDistributedCache>();
            var realCache = new EafSqlServerCache(cacheName, mockDistributedCache);
            _iocManager.Resolve<EafSqlServerCache>(Arg.Any<object>()).Returns(realCache);

            // Act
            var cache1 = manager.GetCache(cacheName);
            var cache2 = manager.GetCache(cacheName);

            // Assert
            cache1.ShouldBe(cache2);
        }

        [Fact]
        public void GetCache_WithDifferentNames_ShouldCallResolveForEach()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var cacheName1 = "TestCache1";
            var cacheName2 = "TestCache2";

            var mockDistributedCache1 = Substitute.For<IDistributedCache>();
            var mockDistributedCache2 = Substitute.For<IDistributedCache>();
            var realCache1 = new EafSqlServerCache(cacheName1, mockDistributedCache1);
            var realCache2 = new EafSqlServerCache(cacheName2, mockDistributedCache2);

            _iocManager.Resolve<EafSqlServerCache>(Arg.Any<object>())
                .Returns(realCache1, realCache2);

            // Act
            var cache1 = manager.GetCache(cacheName1);
            var cache2 = manager.GetCache(cacheName2);

            // Assert
            cache1.ShouldNotBeNull();
            cache2.ShouldNotBeNull();
            cache1.ShouldNotBe(cache2);
            _iocManager.Received(2).Resolve<EafSqlServerCache>(Arg.Any<object>());
        }

        [Fact]
        public void Dispose_ShouldDisposeAllCaches()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var cacheName = "TestCache";

            var mockDistributedCache = Substitute.For<IDistributedCache>();
            var realCache = new EafSqlServerCache(cacheName, mockDistributedCache);
            _iocManager.Resolve<EafSqlServerCache>(Arg.Any<object>()).Returns(realCache);

            // Act
            manager.GetCache(cacheName);
            manager.Dispose();

            // Assert
            _iocManager.Received(1).Resolve<EafSqlServerCache>(Arg.Any<object>());
        }

        [Fact]
        public void GetAllCaches_ShouldReturnEmptyListInitially()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);

            // Act
            var caches = manager.GetAllCaches();

            // Assert
            caches.ShouldNotBeNull();
            caches.Count.ShouldBe(0);
        }

        [Fact]
        public void GetAllCaches_AfterGetCache_ShouldReturnOneCache()
        {
            // Arrange
            var manager = new EafSqlServerCacheManager(_iocManager, _configuration);
            var cacheName = "TestCache";

            var mockDistributedCache = Substitute.For<IDistributedCache>();
            var realCache = new EafSqlServerCache(cacheName, mockDistributedCache);
            _iocManager.Resolve<EafSqlServerCache>(Arg.Any<object>()).Returns(realCache);

            // Act
            manager.GetCache(cacheName);
            var caches = manager.GetAllCaches();

            // Assert
            caches.ShouldNotBeNull();
            caches.Count.ShouldBe(1);
        }
    }
}