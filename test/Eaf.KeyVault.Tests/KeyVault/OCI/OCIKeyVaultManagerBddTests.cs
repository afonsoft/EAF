using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Oci.Common;
using Oci.Common.Auth;
using Oci.Common.Http;
using Oci.SecretsService;
using Org.BouncyCastle.Crypto.Parameters;
using Shouldly;
using System;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.KeyVault.Tests.KeyVault.OCI
{
    /// <summary>
    /// Testes BDD para OCIKeyVaultManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class OCIKeyVaultManagerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarInterface_Entao_DeveImplementarIKeyVaultManager()
        {
            typeof(OCIKeyVaultManager).ShouldNotBeNull();
            typeof(IKeyVaultManager).IsAssignableFrom(typeof(OCIKeyVaultManager)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_OptionsComConfigFileInvalido_Quando_Construir_Entao_DeveLogarErroELancarExcecao()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            options.Oci.ConfigFile = "/tmp/oci-config-invalid-" + Guid.NewGuid();
            var logger = Substitute.For<ILogger>();

            // Quando & Então
            Should.Throw<Exception>(() => new OCIKeyVaultManager(options, logger));
            logger.Received(1).ErrorFormat(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void Dado_Instancia_Quando_SetValue_Entao_DeveLancarNotImplementedException()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            Should.Throw<NotImplementedException>(() => sut.SetValue("key", "value"));
        }

        [Fact]
        public void Dado_Instancia_Quando_SetValueAsync_Entao_DeveLancarNotImplementedException()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            Should.Throw<NotImplementedException>(() => sut.SetValueAsync("key", "value"));
        }

        [Fact]
        public void Dado_StringBase64_Quando_Base64Decode_Entao_DeveRetornarStringOriginal()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            var method = typeof(OCIKeyVaultManager).GetMethod("Base64Decode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            method.ShouldNotBeNull();

            var original = "test-value";
            var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(original));
            var result = method.Invoke(sut, new object[] { base64 });

            result.ShouldBe(original);
        }

        [Fact]
        public void Dado_StringNaoBase64_Quando_Base64Decode_Entao_DeveRetornarEntradaOriginal()
        {
            var sut = (OCIKeyVaultManager)RuntimeHelpers.GetUninitializedObject(typeof(OCIKeyVaultManager));
            var method = typeof(OCIKeyVaultManager).GetMethod("Base64Decode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            method.ShouldNotBeNull();

            var result = method.Invoke(sut, new object[] { "plain-text" });

            result.ShouldBe("plain-text");
        }

        [Fact]
        public void Dado_OptionsComAutenticacaoExplicita_Quando_Construir_Entao_DeveCriarCliente()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();

            var sut = Should.NotThrow(() => new OCIKeyVaultManager(options, logger));
            var clientField = typeof(OCIKeyVaultManager).GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);
            clientField.ShouldNotBeNull();
            clientField.GetValue(sut).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OptionsSemConfiguracao_Quando_Construir_Entao_DeveLogarErroELancarExcecao()
        {
            var options = new EafKeyVaultOptions();
            var logger = Substitute.For<ILogger>();

            Should.Throw<Exception>(() => new OCIKeyVaultManager(options, logger));
            logger.Received(1).ErrorFormat(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void Dado_ClienteComAutenticacaoExplicita_Quando_GetKeyValues_Entao_DeveRetornarDicionarioVazioSemLancarExcecao()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Oci.SecretId = "secret-id";
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();
            var sut = new OCIKeyVaultManager(options, logger);

            var result = Should.NotThrow(() => sut.GetKeyValues());
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_ClienteComAutenticacaoExplicita_Quando_GetValue_Entao_DeveLancarExcecaoQuandoServicoFalhar()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Oci.SecretId = "secret-id";
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();
            var sut = new OCIKeyVaultManager(options, logger);

            Should.Throw<Exception>(() => sut.GetValue("secret-name"));
        }

        [Fact]
        public void Dado_ClienteComVaultId_Quando_GetValue_Entao_DeveRetornarValorDecodificado()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Oci.SecretId = "secret-id";
            options.Oci.VaultId = "vault-id";
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();
            var sut = new OCIKeyVaultManager(options, logger);

            SubstituirHttpClient(sut, request =>
            {
                if (request.Method == HttpMethod.Post && request.RequestUri!.AbsolutePath.Contains("actions/getByName"))
                    return CriarResponseSecretBundle("secret-value");

                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            });

            var result = sut.GetValue("secret-name");
            result.ShouldBe("secret-value");
        }

        [Fact]
        public void Dado_ClienteSemVaultId_Quando_GetValue_Entao_DeveRetornarValorDecodificado()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Oci.SecretId = "secret-id";
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();
            var sut = new OCIKeyVaultManager(options, logger);

            SubstituirHttpClient(sut, request =>
            {
                if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.Contains("/secretbundles/"))
                    return CriarResponseSecretBundle("secret-value");

                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            });

            var result = sut.GetValue("secret-name");
            result.ShouldBe("secret-value");
        }

        [Fact]
        public void Dado_ListaDeVersoes_Quando_GetKeyValues_Entao_DeveRetornarDicionarioComValores()
        {
            var options = new EafKeyVaultOptions();
            options.Oci.UserId = "user-id";
            options.Oci.TenantId = "tenant-id";
            options.Oci.Region = "us-ashburn-1";
            options.Oci.Fingerprint = "fingerprint";
            options.Oci.KeySupplier = Substitute.For<ISupplier<RsaKeyParameters>>();
            options.Oci.SecretId = "secret-id";
            options.Oci.VaultId = "vault-id";
            options.Endpoint = new Uri("https://secrets.us-ashburn-1.oci.oraclecloud.com");

            var logger = Substitute.For<ILogger>();
            var sut = new OCIKeyVaultManager(options, logger);

            SubstituirHttpClient(sut, request =>
            {
                if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.Contains("/versions"))
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent("[{\"secretId\":\"secret-id\",\"versionNumber\":1}]", System.Text.Encoding.UTF8, "application/json")
                    };

                if (request.Method == HttpMethod.Post && request.RequestUri!.AbsolutePath.Contains("actions/getByName"))
                    return CriarResponseSecretBundle("secret-value");

                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            });

            var result = sut.GetKeyValues();
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result["1"].ShouldBe("secret-value");
        }

        private static HttpResponseMessage CriarResponseSecretBundle(string value)
        {
            var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
            var json = $"{{\"secretId\":\"secret-id\",\"versionNumber\":1,\"secretBundleContent\":{{\"contentType\":\"BASE64\",\"content\":\"{base64}\"}}}}";
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }

        private static void SubstituirHttpClient(OCIKeyVaultManager sut, Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            var clientField = typeof(OCIKeyVaultManager).GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);
            var secretsClient = clientField!.GetValue(sut)!;

            var httpClient = new HttpClient(new TestHandler(responseFactory))
            {
                BaseAddress = new Uri("https://test/")
            };

            var restClient = (RestClient)Activator.CreateInstance(typeof(RestClient), true)!;
            var httpClientField = typeof(RestClient).GetField("httpClient", BindingFlags.NonPublic | BindingFlags.Instance);
            httpClientField!.SetValue(restClient, httpClient);

            var restClientField = typeof(ClientBase).GetField("restClient", BindingFlags.NonPublic | BindingFlags.Instance);
            restClientField!.SetValue(secretsClient, restClient);
        }

        private class TestHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

            public TestHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_responseFactory(request));
            }
        }
    }
}
