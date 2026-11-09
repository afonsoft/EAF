using Eaf.OpenIddict.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.OpenIddict.Tests
{
    public class EafOpenIddictOptionsTests
    {
        [Fact]
        // Dado uma configuração com issuer e uma aplicação, Quando bindeado, Então as opções carregam corretamente
        public void Dado_Configuracao_Completa_Quando_Bindeada_Entao_Carrega_Opcoes()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["OpenIddict:Issuer"] = "https://issuer.example/",
                    ["OpenIddict:AccessTokenLifetimeMinutes"] = "30",
                    ["OpenIddict:Applications:0:ClientId"] = "client",
                    ["OpenIddict:Applications:0:ClientSecret"] = "secret",
                    ["OpenIddict:Applications:0:DisplayName"] = "App",
                    ["OpenIddict:Applications:0:ConsentType"] = "implicit",
                    ["OpenIddict:Applications:0:Permissions:0"] = "ept:token",
                    ["OpenIddict:Applications:0:Permissions:1"] = "gt:password",
                    ["OpenIddict:Applications:0:Scopes:0"] = "default-api"
                })
                .Build();

            var options = config.GetSection(EafOpenIddictOptions.SectionName).Get<EafOpenIddictOptions>();

            options.ShouldNotBeNull();
            options.Issuer.ShouldBe("https://issuer.example/");
            options.AccessTokenLifetimeMinutes.ShouldBe(30);
            options.Applications.Count.ShouldBe(1);
            options.Applications[0].ClientId.ShouldBe("client");
            options.Applications[0].Permissions.Count.ShouldBe(2);
        }

        [Fact]
        // Dado nenhuma seção configurada, Quando bindeado, Então os padrões são aplicados
        public void Dado_Sem_Secao_Quando_Bindeada_Entao_Aplica_Defaults()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            var options = config.GetSection(EafOpenIddictOptions.SectionName).Get<EafOpenIddictOptions>()
                ?? new EafOpenIddictOptions();

            options.AccessTokenLifetimeMinutes.ShouldBe(60);
            options.AuthorizationCodeLifetimeMinutes.ShouldBe(5);
            options.Applications.ShouldBeEmpty();
        }
    }
}
