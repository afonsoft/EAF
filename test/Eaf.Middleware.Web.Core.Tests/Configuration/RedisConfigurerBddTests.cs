using Eaf.Middleware.Web.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    public class RedisConfigurerBddTests
    {
        private static IConfiguration CriarConfiguracao(Dictionary<string, string?> valores)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(valores)
                .Build();
        }

        [Fact]
        public void Dado_RedisDesabilitado_Quando_Configure_Entao_NaoDeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsRedisEnabled", "false" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldNotContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisHabilitadoComConnectionString_Quando_Configure_Entao_DeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsRedisEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisHabilitadoViaIsEnabled_Quando_Configure_Entao_DeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisHabilitadoComConnectionString_Quando_ObterOptions_Entao_DeveTerConnectionStringConfigurada()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsRedisEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            RedisConfigurer.Configure(services, configuration);

            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IOptions<Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>>();
            options.ShouldNotBeNull();
            options!.Value.Configuration.ShouldBe("localhost:6379");
        }

        [Fact]
        public void Dado_RedisHabilitadoViaIsEnabledSemConnectionString_Quando_Configure_Entao_NaoDeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsEnabled", "true" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldNotContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisDesabilitadoViaIsEnabled_Quando_Configure_Entao_NaoDeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsEnabled", "false" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldNotContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisDesabilitadoViaIsRedisEnabled_Quando_Configure_Entao_NaoDeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsRedisEnabled", "false" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldNotContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }

        [Fact]
        public void Dado_RedisHabilitadoSemConnectionString_Quando_Configure_Entao_NaoDeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = CriarConfiguracao(new Dictionary<string, string?>
            {
                { "RedisCache:IsRedisEnabled", "true" }
            });

            RedisConfigurer.Configure(services, configuration);

            services.ShouldNotContain(s => s.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));
        }
    }
}
