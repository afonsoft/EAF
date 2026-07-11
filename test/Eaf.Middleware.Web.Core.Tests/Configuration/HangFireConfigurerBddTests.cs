using Eaf.Hangfire;
using Eaf.Middleware.Web.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class HangFireConfigurerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(HangFireConfigurer).IsAbstract.ShouldBeTrue();
            typeof(HangFireConfigurer).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_HangfireAtivado_Quando_Configure_Entao_DeveRegistrarServicosHangfire()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsEnabled"] = "true"
                })
                .Build();

            // Quando
            Should.NotThrow(() => HangFireConfigurer.Configure(services, configuration));

            // Então
            services.ShouldContain(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("Hangfire"));
        }

        [Fact]
        public void Dado_HangfireDesativado_Quando_Configure_Entao_NaoDeveRegistrarHangfire()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsEnabled"] = "false"
                })
                .Build();

            // Quando
            HangFireConfigurer.Configure(services, configuration);

            // Então
            services.ShouldNotContain(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("Hangfire"));
        }

        [Theory]
        [InlineData("SqlServer", HangfireStorageType.SqlServer)]
        [InlineData("MSSQL", HangfireStorageType.SqlServer)]
        [InlineData("PostgreSQL", HangfireStorageType.InMemory)]
        public void Dado_Provider_Quando_ResolveStorageType_Entao_DeveRetornarTipoEsperado(string databaseProvider, HangfireStorageType expected)
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Provider"] = databaseProvider
                })
                .Build();

            // Quando
            var result = HangFireConfigurer.ResolveStorageType(configuration);

            // Então
            result.ShouldBe(expected);
        }

        [Fact]
        public void Dado_InMemoryForcado_Quando_ResolveStorageType_Entao_DeveRetornarInMemory()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsInMemoryDatabase"] = "true",
                    ["Database:Provider"] = "SqlServer"
                })
                .Build();

            // Quando
            var result = HangFireConfigurer.ResolveStorageType(configuration);

            // Então
            result.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_RedisHabilitado_Quando_ResolveStorageType_Entao_DeveRetornarRedis()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Provider"] = "PostgreSQL",
                    ["RedisCache:IsRedisEnabled"] = "true"
                })
                .Build();

            // Quando
            var result = HangFireConfigurer.ResolveStorageType(configuration);

            // Então
            result.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_RedisIsEnabled_Quando_ResolveStorageType_Entao_DeveRetornarRedis()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Provider"] = "PostgreSQL",
                    ["RedisCache:IsEnabled"] = "true"
                })
                .Build();

            // Quando
            var result = HangFireConfigurer.ResolveStorageType(configuration);

            // Então
            result.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_HangfireAtivadoComProviderNaoSql_Quando_Configure_Entao_DeveRegistrarServicosHangfire()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsEnabled"] = "true",
                    ["Database:Provider"] = "PostgreSQL"
                })
                .Build();

            // Quando
            Should.NotThrow(() => HangFireConfigurer.Configure(services, configuration));

            // Então
            services.ShouldContain(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("Hangfire"));
        }

        [Fact]
        public void Dado_HangfireAtivadoComSqlServer_Quando_Configure_Entao_DeveRegistrarServicosHangfire()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsEnabled"] = "true",
                    ["Database:Provider"] = "SqlServer"
                })
                .Build();

            // Quando
            Should.NotThrow(() => HangFireConfigurer.Configure(services, configuration));

            // Então
            services.ShouldContain(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("Hangfire"));
        }

        [Fact]
        public void Dado_HangfireAtivadoComRedis_Quando_Configure_Entao_DeveRegistrarServicosHangfire()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Hangfire:IsEnabled"] = "true",
                    ["Database:Provider"] = "PostgreSQL",
                    ["RedisCache:IsEnabled"] = "true"
                })
                .Build();

            // Quando
            Should.NotThrow(() => HangFireConfigurer.Configure(services, configuration));

            // Então
            services.ShouldContain(s => s.ServiceType.FullName != null && s.ServiceType.FullName.Contains("Hangfire"));
        }
    }
}
