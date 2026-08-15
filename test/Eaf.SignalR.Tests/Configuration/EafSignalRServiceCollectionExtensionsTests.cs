using System.Collections.Generic;
using System.Linq;
using Eaf.SignalR.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Eaf.SignalR.Tests.Configuration
{
    public class EafSignalRServiceCollectionExtensionsTests
    {
        [Fact]
        public void Dado_ConfiguracaoPadrao_Quando_AddEafSignalR_Entao_OpcoesDevemEstarRegistradas()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EafSignalR:ClientTimeoutIntervalSeconds", "120" },
                    { "EafSignalR:UseRedisBackplane", "false" }
                })
                .Build();

            services.AddEafSignalR(configuration);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<EafSignalROptions>>().Value;

            options.ClientTimeoutIntervalSeconds.ShouldBe(120);
            options.UseRedisBackplane.ShouldBeFalse();
        }

        [Fact]
        public void Dado_RedisBackplaneHabilitado_Quando_AddEafSignalR_Entao_ServicoRedisDeveEstarRegistrado()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EafSignalR:UseRedisBackplane", "true" },
                    { "EafSignalR:RedisConnectionString", "localhost:6379" }
                })
                .Build();

            services.AddEafSignalR(configuration);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<EafSignalROptions>>().Value;

            options.UseRedisBackplane.ShouldBeTrue();
            options.RedisConnectionString.ShouldBe("localhost:6379");
        }
    }
}
