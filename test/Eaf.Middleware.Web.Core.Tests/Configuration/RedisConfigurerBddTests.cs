using Eaf.Middleware.Web.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
