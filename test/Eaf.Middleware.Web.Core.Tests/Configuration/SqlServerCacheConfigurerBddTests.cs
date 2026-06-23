using Eaf.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Configuration
{
    /// <summary>
    /// Testes BDD para SqlServerCacheConfigurer seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class SqlServerCacheConfigurerBddTests
    {
        #region Configure - SQL desabilitado

        [Fact]
        public void Dado_SqlDesabilitado_Quando_Configure_Entao_NaoDeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "SqlServer:IsSqlEnabled", "false" }
            });

            // Quando
            SqlServerCacheConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_SemConfiguracaoSql_Quando_Configure_Entao_NaoDeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>());

            // Quando
            SqlServerCacheConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBe(0);
        }

        #endregion

        #region Configure - SQL habilitado

        [Fact]
        public void Dado_SqlHabilitadoViaIsSqlEnabled_Quando_Configure_Entao_DeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "SqlServer:IsSqlEnabled", "true" },
                { "SqlServerCache:ConnectionString", "Server=.;Database=Cache;" }
            });

            // Quando
            SqlServerCacheConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_SqlHabilitadoViaIsEnabled_Quando_Configure_Entao_DeveAdicionarServico()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "SqlServerCache:IsEnabled", "true" },
                { "SqlServerCache:ConnectionString", "Server=.;Database=Cache;" }
            });

            // Quando
            SqlServerCacheConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_SqlHabilitadoComDefaultConnection_Quando_Configure_Entao_DeveUsarDefaultConnectionString()
        {
            // Dado
            var services = new ServiceCollection();
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "SqlServer:IsSqlEnabled", "true" },
                { "ConnectionStrings:Default", "Server=.;Database=EafDefault;" }
            });

            // Quando
            SqlServerCacheConfigurer.Configure(services, config);

            // Entao
            services.Count.ShouldBeGreaterThan(0);
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
