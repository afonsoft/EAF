using Abp.Modules;
using Eaf.Runtime.Caching.Redis;
using Shouldly;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes para o módulo de cache Redis.
    /// </summary>
    public class EafRedisCacheModuleTests
    {
        [Fact]
        public void EafRedisCacheModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafRedisCacheModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void EafRedisCacheModule_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(EafRedisCacheModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeEmpty();
        }

        [Fact]
        public void EafRedisCacheModule_ShouldBeInstantiable()
        {
            // Act
            var module = new EafRedisCacheModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafRedisCacheModule>();
        }
    }
}
