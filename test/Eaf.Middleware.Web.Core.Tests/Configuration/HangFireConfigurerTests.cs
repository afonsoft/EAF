using Eaf.Hangfire;
using Eaf.Middleware.Web.Startup;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    public class HangFireConfigurerTests
    {
        private static IConfiguration BuildConfiguration(Dictionary<string, string> settings)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        #region ResolveStorageType - SQL Server

        [Fact]
        public void Dado_ProviderSqlServer_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "SqlServer" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_ProviderMSSQL_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "MSSQL" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_ProviderSqlServerCaseInsensitive_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "sqlserver" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_ProviderNaoDefinido_Quando_ResolverStorage_Entao_DeveRetornarSqlServerPadrao()
        {
            var config = BuildConfiguration(new Dictionary<string, string>());

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        #endregion

        #region ResolveStorageType - Redis

        [Fact]
        public void Dado_ProviderPostgreSQL_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarRedis()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "PostgreSQL" },
                { "RedisCache:IsEnabled", "true" },
                { "RedisCache:ConnectionString", "localhost:6379" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_ProviderMySQL_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarRedis()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "MySQL" },
                { "RedisCache:IsEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_ProviderPostgres_E_IsRedisEnabled_Quando_ResolverStorage_Entao_DeveRetornarRedis()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "Postgres" },
                { "RedisCache:IsRedisEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.Redis);
        }

        #endregion

        #region ResolveStorageType - InMemory

        [Fact]
        public void Dado_ProviderPostgreSQL_E_RediDesabilitado_Quando_ResolverStorage_Entao_DeveRetornarInMemory()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "PostgreSQL" },
                { "RedisCache:IsEnabled", "false" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_ProviderMySQL_E_SemRedis_Quando_ResolverStorage_Entao_DeveRetornarInMemory()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "MySQL" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_ForceInMemory_E_SqlServer_Quando_ResolverStorage_Entao_DeveRetornarInMemory()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "SqlServer" },
                { "Hangfire:IsInMemoryDatabase", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_ForceInMemory_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarInMemory()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "PostgreSQL" },
                { "Hangfire:IsInMemoryDatabase", "true" },
                { "RedisCache:IsEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        #endregion

        #region ResolveStorageType - Edge Cases

        [Fact]
        public void Dado_SqlServer_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "SqlServer" },
                { "RedisCache:IsEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_MSSQL_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "MSSQL" },
                { "RedisCache:IsEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_IsInMemoryDatabaseFalse_E_SqlServer_Quando_ResolverStorage_Entao_DeveRetornarSqlServer()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "SqlServer" },
                { "Hangfire:IsInMemoryDatabase", "false" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_ProviderOracle_E_SemRedis_Quando_ResolverStorage_Entao_DeveRetornarInMemory()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "Oracle" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_ProviderOracle_E_RedisHabilitado_Quando_ResolverStorage_Entao_DeveRetornarRedis()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "Oracle" },
                { "RedisCache:IsEnabled", "true" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_RedisConfigInvalido_Quando_ResolverStorage_Entao_DeveRetornarInMemorySemException()
        {
            var config = BuildConfiguration(new Dictionary<string, string>
            {
                { "Database:Provider", "PostgreSQL" },
                { "RedisCache:IsEnabled", "yes" }
            });

            var result = HangFireConfigurer.ResolveStorageType(config);

            result.ShouldBe(HangfireStorageType.InMemory);
        }

        #endregion
    }
}
