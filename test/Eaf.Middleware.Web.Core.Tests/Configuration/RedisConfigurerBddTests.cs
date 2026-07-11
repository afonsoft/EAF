using Eaf.Middleware.Web.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Configuration
{
    /// <summary>
    /// Testes BDD para RedisConfigurer seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class RedisConfigurerBddTests
    {
        #region Configure - Redis desabilitado

        [Fact]
        public void Dado_RedisDesabilitado_Quando_Configure_Entao_NaoDeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "RedisCache:IsRedisEnabled", "false" }
            });

            // Quando
            RedisConfigurer.Configure(services, config);

            // Entao - nao deve lançar exceção
            services.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_SemConfiguracaoRedis_Quando_Configure_Entao_NaoDeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>());

            // Quando
            RedisConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBe(0);
        }

        #endregion

        #region Configure - Redis habilitado

        [Fact]
        public void Dado_RedisHabilitadoViaIsRedisEnabled_Quando_Configure_Entao_DeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "RedisCache:IsRedisEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            // Quando
            RedisConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_RedisHabilitadoViaIsEnabled_Quando_Configure_Entao_DeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "RedisCache:IsEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            // Quando
            RedisConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_RedisHabilitadoSemConnectionString_Quando_Configure_Entao_NaoDeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "RedisCache:IsRedisEnabled", "true" }
            });

            // Quando
            RedisConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_RedisHabilitadoComValorInvalido_Quando_Configure_Entao_DeveLancarFormatException()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "RedisCache:IsRedisEnabled", "invalid" }
            });

            // Quando & Então
            Should.Throw<FormatException>(() => RedisConfigurer.Configure(services, config));
        }

        #endregion

        private static IConfiguration BuildConfiguration(Dictionary<string, string> data)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();
        }
    }
}
