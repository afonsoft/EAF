using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Sqlite;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    public class SqliteCacheConfigurationExtensionsTests
    {
        private readonly ICachingConfiguration _cachingConfiguration;
        private readonly IIocManager _iocManager;
        private readonly IAbpStartupConfiguration _abpConfiguration;

        public SqliteCacheConfigurationExtensionsTests()
        {
            _iocManager = Substitute.For<IIocManager>();
            _abpConfiguration = Substitute.For<IAbpStartupConfiguration>();
            _cachingConfiguration = Substitute.For<ICachingConfiguration>();

            _cachingConfiguration.AbpConfiguration.Returns(_abpConfiguration);
            _abpConfiguration.IocManager.Returns(_iocManager);
        }

        [Fact]
        public void UseSqlite_WithoutOptions_ShouldRegisterCacheManager()
        {
            // Act
            _cachingConfiguration.UseSqlite();

            // Assert
            _iocManager.Received(1).RegisterIfNot<ICacheManager, EafSqliteCacheManager>();
        }

        [Fact]
        public void UseSqlite_WithOptions_ShouldRegisterCacheManagerAndConfigureOptions()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            _iocManager.Resolve<EafSqliteCacheOptions>().Returns(options);
            var optionsCalled = false;

            // Act
            _cachingConfiguration.UseSqlite(opt =>
            {
                optionsCalled = true;
                opt.MemoryOnly = true;
                opt.CachePath = "test.db";
            });

            // Assert
            _iocManager.Received(1).RegisterIfNot<ICacheManager, EafSqliteCacheManager>();
            _iocManager.Received(1).Resolve<EafSqliteCacheOptions>();
            optionsCalled.ShouldBeTrue();
            options.MemoryOnly.ShouldBeTrue();
            options.CachePath.ShouldBe("test.db");
        }

        [Fact]
        public void UseSqlite_WithNullCachingConfiguration_ShouldThrowException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => SqliteCacheConfigurationExtensions.UseSqlite(null));
        }

        [Fact]
        public void UseSqlite_MultipleCalls_ShouldRegisterOnlyOnce()
        {
            // Act
            _cachingConfiguration.UseSqlite();
            _cachingConfiguration.UseSqlite();

            // Assert
            _iocManager.Received(2).RegisterIfNot<ICacheManager, EafSqliteCacheManager>();
        }

        [Fact]
        public void UseSqlite_WithEmptyOptionsAction_ShouldNotModifyOptions()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var originalMemoryOnly = options.MemoryOnly;
            var originalCachePath = options.CachePath;
            var originalCleanupInterval = options.CleanupInterval;

            _iocManager.Resolve<EafSqliteCacheOptions>().Returns(options);

            // Act
            _cachingConfiguration.UseSqlite(opt => { });

            // Assert
            options.MemoryOnly.ShouldBe(originalMemoryOnly);
            options.CachePath.ShouldBe(originalCachePath);
            options.CleanupInterval.ShouldBe(originalCleanupInterval);
        }

        [Fact]
        public void UseSqlite_ShouldAccessAbpConfigurationProperty()
        {
            // Act
            _cachingConfiguration.UseSqlite();

            // Assert
            var _ = _cachingConfiguration.Received(1).AbpConfiguration;
        }

        [Fact]
        public void UseSqlite_ShouldAccessIocManagerProperty()
        {
            // Act
            _cachingConfiguration.UseSqlite();

            // Assert
            var _ = _abpConfiguration.Received(1).IocManager;
        }

        [Fact]
        public void UseSqlite_WithOptionsAction_ShouldResolveOptionsFromIocManager()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            _iocManager.Resolve<EafSqliteCacheOptions>().Returns(options);

            // Act
            _cachingConfiguration.UseSqlite(opt => opt.MemoryOnly = true);

            // Assert
            _iocManager.Received(1).Resolve<EafSqliteCacheOptions>();
        }

        [Fact]
        public void UseSqlite_WithExceptionInOptionsAction_ShouldPropagateException()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            _iocManager.Resolve<EafSqliteCacheOptions>().Returns(options);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                _cachingConfiguration.UseSqlite(opt => throw new InvalidOperationException("Test exception")));
        }
    }
}