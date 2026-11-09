using Eaf.OpenIddict.Configuration;
using Eaf.OpenIddict.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenIddict.Abstractions;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.OpenIddict.Tests
{
    public class EafOpenIddictSeedWorkerTests
    {
        private static (EafOpenIddictSeedWorker worker, IOpenIddictApplicationManager manager) CriarWorker(
            Dictionary<string, string> configValues)
        {
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var options = config.GetSection(EafOpenIddictOptions.SectionName)
                .Get<EafOpenIddictOptions>() ?? new EafOpenIddictOptions();

            var manager = Substitute.For<IOpenIddictApplicationManager>();

            var services = new ServiceCollection();
            services.AddSingleton(Options.Create(options));
            services.AddSingleton(manager);
            services.AddLogging();

            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<EafOpenIddictSeedWorker>>();

            return (new EafOpenIddictSeedWorker(provider, logger), manager);
        }

        [Fact]
        // Dado uma aplicação configurada inexistente no store, Quando o worker inicia, Então cria o descritor com permissões padrão
        public async Task Dado_AppNova_Quando_StartAsync_Entao_CriaComPermissoesPadrao()
        {
            var (worker, manager) = CriarWorker(new Dictionary<string, string>
            {
                ["OpenIddict:Applications:0:ClientId"] = "spa",
                ["OpenIddict:Applications:0:ClientSecret"] = "secret",
                ["OpenIddict:Applications:0:DisplayName"] = "SPA",
                ["OpenIddict:Applications:0:ConsentType"] = "explicit",
                ["OpenIddict:Applications:0:RedirectUris:0"] = "https://app/cb",
            });
            manager.FindByClientIdAsync("spa", Arg.Any<CancellationToken>())
                .Returns(new ValueTask<object>((object)null));

            await worker.StartAsync(CancellationToken.None);

            await manager.Received(1).CreateAsync(
                Arg.Is<OpenIddictApplicationDescriptor>(d =>
                    d.ClientId == "spa" &&
                    d.DisplayName == "SPA" &&
                    d.ConsentType == OpenIddictConstants.ConsentTypes.Explicit &&
                    d.RedirectUris.Contains(new System.Uri("https://app/cb")) &&
                    d.Permissions.Contains(OpenIddictConstants.Permissions.Endpoints.Token) &&
                    d.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.Password) &&
                    d.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials) &&
                    d.Permissions.Contains(OpenIddictConstants.Permissions.Prefixes.Scope + "default-api")),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        // Dado uma aplicação já existente no store, Quando o worker inicia, Então atualiza o descritor em vez de duplicar
        public async Task Dado_AppExistente_Quando_StartAsync_Entao_AtualizaSemDuplicar()
        {
            var (worker, manager) = CriarWorker(new Dictionary<string, string>
            {
                ["OpenIddict:Applications:0:ClientId"] = "client",
                ["OpenIddict:Applications:0:Permissions:0"] = "ept:token",
            });
            var existing = new object();
            manager.FindByClientIdAsync("client", Arg.Any<CancellationToken>())
                .Returns(new ValueTask<object>(existing));

            await worker.StartAsync(CancellationToken.None);

            await manager.DidNotReceive().CreateAsync(
                Arg.Any<OpenIddictApplicationDescriptor>(), Arg.Any<CancellationToken>());
            await manager.Received(1).UpdateAsync(
                existing,
                Arg.Is<OpenIddictApplicationDescriptor>(d =>
                    d.ClientId == "client" &&
                    d.Permissions.Contains("ept:token")),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        // Dado uma aplicação sem ClientId, Quando o worker inicia, Então ignora sem chamar o store
        public async Task Dado_AppSemClientId_Quando_StartAsync_Entao_Ignora()
        {
            var (worker, manager) = CriarWorker(new Dictionary<string, string>
            {
                ["OpenIddict:Applications:0:DisplayName"] = "sem client id",
            });

            await worker.StartAsync(CancellationToken.None);

            await manager.DidNotReceive().FindByClientIdAsync(
                Arg.Any<string>(), Arg.Any<CancellationToken>());
            await manager.DidNotReceive().CreateAsync(
                Arg.Any<OpenIddictApplicationDescriptor>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        // Dado consent type desconhecido, Quando o worker inicia, Então usa Implicit como fallback
        public async Task Dado_ConsentTypeDesconhecido_Quando_StartAsync_Entao_UsaImplicit()
        {
            var (worker, manager) = CriarWorker(new Dictionary<string, string>
            {
                ["OpenIddict:Applications:0:ClientId"] = "c",
                ["OpenIddict:Applications:0:ConsentType"] = "qualquer-coisa",
            });
            manager.FindByClientIdAsync("c", Arg.Any<CancellationToken>())
                .Returns(new ValueTask<object>((object)null));

            await worker.StartAsync(CancellationToken.None);

            await manager.Received(1).CreateAsync(
                Arg.Is<OpenIddictApplicationDescriptor>(d =>
                    d.ConsentType == OpenIddictConstants.ConsentTypes.Implicit),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        // Dado nenhuma aplicação configurada, Quando o worker inicia, Então não toca o store
        public async Task Dado_SemApps_Quando_StartAsync_Entao_NaoFazNada()
        {
            var (worker, manager) = CriarWorker(new Dictionary<string, string>());

            await worker.StartAsync(CancellationToken.None);

            await manager.DidNotReceive().CreateAsync(
                Arg.Any<OpenIddictApplicationDescriptor>(), Arg.Any<CancellationToken>());
        }
    }
}
