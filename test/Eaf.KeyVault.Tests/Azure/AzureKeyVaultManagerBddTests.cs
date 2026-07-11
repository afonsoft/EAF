using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests.Azure
{
    /// <summary>
    /// Testes BDD para AzureKeyVaultManager cobrindo caminhos felizes e de exceção.
    /// </summary>
    public class AzureKeyVaultManagerBddTests
    {
        private const string Endpoint = "https://test.vault.azure.net/";

        private readonly EafKeyVaultOptions _options;
        private readonly ILogger _logger;

        public AzureKeyVaultManagerBddTests()
        {
            _options = new EafKeyVaultOptions
            {
                Endpoint = new Uri(Endpoint),
                Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "app-id",
                    TenantId = "tenant-id",
                    ClientSecret = "client-secret"
                }
            };
            _logger = Substitute.For<ILogger>();
        }

        private static AzureKeyVaultManager CriarManagerComClientSubstituto(EafKeyVaultOptions? options = null, ILogger? logger = null)
        {
            var manager = new AzureKeyVaultManager(options ?? new EafKeyVaultOptions
            {
                Endpoint = new Uri(Endpoint),
                Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "app-id",
                    TenantId = "tenant-id",
                    ClientSecret = "client-secret"
                }
            }, logger ?? Substitute.For<ILogger>());

            var field = typeof(AzureKeyVaultManager).GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);
            field.ShouldNotBeNull();

            var client = Substitute.For<SecretClient>(new object[] { new Uri(Endpoint), Substitute.For<TokenCredential>() });

            var response = Substitute.For<Response>();
            response.Status.Returns(200);

            var secret = new KeyVaultSecret("name", "value");
            client.GetSecret(Arg.Any<string>()).Returns(Response.FromValue(secret, response));
            client.GetSecretAsync(Arg.Any<string>()).Returns(Task.FromResult(Response.FromValue(secret, response)));

            var properties = new List<SecretProperties>
            {
                new SecretProperties("name") { Enabled = true }
            };
            var pageable = Substitute.For<Pageable<SecretProperties>>();
            pageable.GetEnumerator().Returns(properties.GetEnumerator());
            pageable.AsPages().Returns(properties.ToPages());
            client.GetPropertiesOfSecrets().Returns(pageable);

            client.SetSecret(Arg.Any<string>(), Arg.Any<string>()).Returns(Response.FromValue(secret, response));
            client.SetSecretAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(Response.FromValue(secret, response)));

            field.SetValue(manager, client);
            return manager;
        }

        [Fact]
        public void Dado_SecretClientSubstituto_Quando_GetValue_Entao_DeveRetornarValor()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando
            var result = manager.GetValue("name");

            // Então
            result.ShouldBe("value");
        }

        [Fact]
        public async Task Dado_SecretClientSubstituto_Quando_GetValueAsync_Entao_DeveRetornarValor()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando
            var result = await manager.GetValueAsync("name");

            // Então
            result.ShouldBe("value");
        }

        [Fact]
        public void Dado_SecretClientSubstituto_Quando_GetKeyValues_Entao_DeveRetornarDicionarioComValores()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando
            var result = manager.GetKeyValues();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result["name"].ShouldBe("value");
        }

        [Fact]
        public async Task Dado_SecretClientSubstituto_Quando_GetKeyValuesAsync_Entao_DeveRetornarDicionarioComValores()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando
            var result = await manager.GetKeyValuesAsync();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result["name"].ShouldBe("value");
        }

        [Fact]
        public void Dado_SecretClientSubstituto_Quando_SetValue_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando & Então
            Should.NotThrow(() => manager.SetValue("name", "value"));
        }

        [Fact]
        public async Task Dado_SecretClientSubstituto_Quando_SetValueAsync_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var manager = CriarManagerComClientSubstituto(_options, _logger);

            // Quando & Então
            await Should.NotThrowAsync(async () => await manager.SetValueAsync("name", "value"));
        }

        [Fact]
        public void Dado_GetValueFalhando_Quando_GetValue_Entao_DeveLogarELancarExcecao()
        {
            // Dado
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Quando & Então
            Should.Throw<Exception>(() => manager.GetValue("name"));
            _logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public void Dado_GetPropertiesOfSecretsFalhando_Quando_GetKeyValues_Entao_DeveLogarELancarExcecao()
        {
            // Dado
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Quando & Então
            Should.Throw<Exception>(() => manager.GetKeyValues());
            _logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>());
        }
    }

    internal static class AzureKeyVaultManagerBddTestsExtensions
    {
        public static IEnumerable<Page<SecretProperties>> ToPages(this IEnumerable<SecretProperties> properties)
        {
            yield return Page<SecretProperties>.FromValues(properties.ToList(), null, Substitute.For<Response>());
        }
    }
}
