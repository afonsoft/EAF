using Eaf.OpenIddict;
using Eaf.OpenIddict.Configuration;
using Eaf.OpenIddict.EntityFrameworkCore;
using Eaf.OpenIddict.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.OpenIddict.Tests
{
    public class EafOpenIddictServiceCollectionExtensionsTests
    {
        private static IConfiguration CriarConfig(Dictionary<string, string> values)
            => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

        [Fact]
        // Dado uma configuração válida, Quando AddEafOpenIddict, Então registra opções, DbContext, managers e o seed worker
        public void Dado_ConfiguracaoValida_Quando_AddEafOpenIddict_Entao_RegistraServicos()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddEafOpenIddict(CriarConfig(new Dictionary<string, string>
            {
                ["ConnectionStrings:Default"] = "Server=(localdb)\\test;Database=oidc;",
                ["OpenIddict:Issuer"] = "https://issuer.example/",
                ["OpenIddict:Applications:0:ClientId"] = "client",
            }));

            var provider = services.BuildServiceProvider();

            provider.GetService<IOptions<EafOpenIddictOptions>>().ShouldNotBeNull();
            provider.GetService<EafOpenIddictDbContext>().ShouldNotBeNull();
            provider.GetService<IOpenIddictApplicationManager>().ShouldNotBeNull();
            services.Any(d => d.ServiceType == typeof(IHostedService) &&
                d.ImplementationType == typeof(EafOpenIddictSeedWorker)).ShouldBeTrue();
        }

        [Fact]
        // Dado issuer inválido, Quando AddEafOpenIddict, Então não quebra na construção do provider
        public void Dado_IssuerInvalido_Quando_AddEafOpenIddict_Entao_NaoQuebra()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddEafOpenIddict(CriarConfig(new Dictionary<string, string>
            {
                ["ConnectionStrings:Default"] = "Server=(localdb)\\test;Database=oidc;",
                ["OpenIddict:Issuer"] = "nao-e-uma-url",
            }));

            Should.NotThrow(() => services.BuildServiceProvider());
        }
    }
}
