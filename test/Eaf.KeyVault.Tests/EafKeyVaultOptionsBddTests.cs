using Eaf.KeyVault;
using Shouldly;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    /// <summary>
    /// Testes BDD para EafKeyVaultOptions seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EafKeyVaultOptionsBddTests
    {
        #region Testes do Construtor

        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarEafKeyVaultOptions_Entao_DeveInicializarComValoresPadrao()
        {
            // Dado & Quando
            var options = new EafKeyVaultOptions();

            // Então
            options.Provider.ShouldBe(EnumKeyVault.None);
            options.Azure.ShouldNotBeNull();
            options.Oci.ShouldNotBeNull();
            options.Endpoint.ShouldBeNull();
            options.Value.ShouldBe(options);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_VerificarPropriedadeValue_Entao_DeveRetornarPropriInstancia()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            var value = options.Value;

            // Então
            value.ShouldBe(options);
            ReferenceEquals(value, options).ShouldBeTrue();
        }

        #endregion

        #region Testes de Propriedades Básicas

        [Theory]
        [InlineData(EnumKeyVault.None)]
        [InlineData(EnumKeyVault.Azure)]
        [InlineData(EnumKeyVault.OCI)]
        public void Dado_ProviderValido_Quando_DefinirProvider_Entao_DeveArmazenarCorretamente(EnumKeyVault provider)
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Provider = provider;

            // Então
            options.Provider.ShouldBe(provider);
        }

        [Fact]
        public void Dado_EndpointValido_Quando_DefinirEndpoint_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var endpoint = new Uri("https://test-vault.vault.azure.net/");

            // Quando
            options.Endpoint = endpoint;

            // Então
            options.Endpoint.ShouldBe(endpoint);
            options.Endpoint.ToString().ShouldBe("https://test-vault.vault.azure.net/");
        }

        [Fact]
        public void Dado_EndpointNulo_Quando_DefinirEndpoint_Entao_DeveAceitarValorNulo()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Endpoint = null;

            // Então
            options.Endpoint.ShouldBeNull();
        }

        #endregion

        #region Testes de AzureKeyVaultOptions

        [Fact]
        public void Dado_NovaInstanciaAzure_Quando_VerificarPropriedadesPadrao_Entao_DeveEstarVazia()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            var azureOptions = options.Azure;

            // Então
            azureOptions.ShouldNotBeNull();
            azureOptions.ApplicationId.ShouldBeNull();
            azureOptions.TenantId.ShouldBeNull();
            azureOptions.ClientSecret.ShouldBeNull();
            azureOptions.Certificate.ShouldBeNull();
        }

        [Fact]
        public void Dado_AzureOptions_Quando_DefinirApplicationId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var applicationId = "12345678-1234-1234-1234-123456789012";

            // Quando
            options.Azure.ApplicationId = applicationId;

            // Então
            options.Azure.ApplicationId.ShouldBe(applicationId);
        }

        [Fact]
        public void Dado_AzureOptions_Quando_DefinirTenantId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var tenantId = "87654321-4321-4321-4321-210987654321";

            // Quando
            options.Azure.TenantId = tenantId;

            // Então
            options.Azure.TenantId.ShouldBe(tenantId);
        }

        [Fact]
        public void Dado_AzureOptions_Quando_DefinirClientSecret_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var clientSecret = "super-secret-key-123";

            // Quando
            options.Azure.ClientSecret = clientSecret;

            // Então
            options.Azure.ClientSecret.ShouldBe(clientSecret);
        }

        [Fact]
        public void Dado_AzureOptions_Quando_DefinirCertificate_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(5));

            // Quando
            options.Azure.Certificate = certificate;

            // Então
            options.Azure.Certificate.ShouldBe(certificate);
        }

        [Fact]
        public void Dado_AzureOptionsCompletas_Quando_DefinirTodasPropriedades_Entao_DeveArmazenarTodas()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var applicationId = "app-id-123";
            var tenantId = "tenant-id-456";
            var clientSecret = "secret-789";
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(5));

            // Quando
            options.Azure.ApplicationId = applicationId;
            options.Azure.TenantId = tenantId;
            options.Azure.ClientSecret = clientSecret;
            options.Azure.Certificate = certificate;

            // Então
            options.Azure.ApplicationId.ShouldBe(applicationId);
            options.Azure.TenantId.ShouldBe(tenantId);
            options.Azure.ClientSecret.ShouldBe(clientSecret);
            options.Azure.Certificate.ShouldBe(certificate);
        }

        #endregion

        #region Testes de OciKeyVaultOptions

        [Fact]
        public void Dado_NovaInstanciaOci_Quando_VerificarPropriedadesPadrao_Entao_DeveEstarComValoresPadrao()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            var ociOptions = options.Oci;

            // Então
            ociOptions.ShouldNotBeNull();
            ociOptions.SecretId.ShouldBeNull();
            ociOptions.VaultId.ShouldBeNull();
            ociOptions.Profile.ShouldBe("DEFAULT");
            ociOptions.ConfigFile.ShouldBeNull();
            ociOptions.TenantId.ShouldBeNull();
            ociOptions.UserId.ShouldBeNull();
            ociOptions.Fingerprint.ShouldBeNull();
            ociOptions.Region.ShouldBeNull();
            ociOptions.KeySupplier.ShouldBeNull();
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirSecretId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var secretId = "ocid1.vaultsecret.oc1.iad.example";

            // Quando
            options.Oci.SecretId = secretId;

            // Então
            options.Oci.SecretId.ShouldBe(secretId);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirVaultId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var vaultId = "ocid1.vault.oc1.iad.example";

            // Quando
            options.Oci.VaultId = vaultId;

            // Então
            options.Oci.VaultId.ShouldBe(vaultId);
        }

        [Theory]
        [InlineData("DEFAULT")]
        [InlineData("CUSTOM")]
        [InlineData("PRODUCTION")]
        public void Dado_OciOptions_Quando_DefinirProfile_Entao_DeveArmazenarCorretamente(string profile)
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Oci.Profile = profile;

            // Então
            options.Oci.Profile.ShouldBe(profile);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirConfigFile_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var configFile = "/home/user/.oci/config";

            // Quando
            options.Oci.ConfigFile = configFile;

            // Então
            options.Oci.ConfigFile.ShouldBe(configFile);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirTenantId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var tenantId = "ocid1.tenancy.oc1..example";

            // Quando
            options.Oci.TenantId = tenantId;

            // Então
            options.Oci.TenantId.ShouldBe(tenantId);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirUserId_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var userId = "ocid1.user.oc1..example";

            // Quando
            options.Oci.UserId = userId;

            // Então
            options.Oci.UserId.ShouldBe(userId);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirFingerprint_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var fingerprint = "12:34:56:78:90:ab:cd:ef:12:34:56:78:90:ab:cd:ef";

            // Quando
            options.Oci.Fingerprint = fingerprint;

            // Então
            options.Oci.Fingerprint.ShouldBe(fingerprint);
        }

        [Fact]
        public void Dado_OciOptions_Quando_DefinirRegion_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var region = "us-ashburn-1";

            // Quando
            options.Oci.Region = region;

            // Então
            options.Oci.Region.ShouldBe(region);
        }

        [Fact]
        public void Dado_OciOptionsCompletas_Quando_DefinirTodasPropriedades_Entao_DeveArmazenarTodas()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var secretId = "secret-123";
            var vaultId = "vault-456";
            var profile = "CUSTOM";
            var configFile = "/path/to/config";
            var tenantId = "tenant-789";
            var userId = "user-012";
            var fingerprint = "fingerprint-345";
            var region = "us-east-1";

            // Quando
            options.Oci.SecretId = secretId;
            options.Oci.VaultId = vaultId;
            options.Oci.Profile = profile;
            options.Oci.ConfigFile = configFile;
            options.Oci.TenantId = tenantId;
            options.Oci.UserId = userId;
            options.Oci.Fingerprint = fingerprint;
            options.Oci.Region = region;

            // Então
            options.Oci.SecretId.ShouldBe(secretId);
            options.Oci.VaultId.ShouldBe(vaultId);
            options.Oci.Profile.ShouldBe(profile);
            options.Oci.ConfigFile.ShouldBe(configFile);
            options.Oci.TenantId.ShouldBe(tenantId);
            options.Oci.UserId.ShouldBe(userId);
            options.Oci.Fingerprint.ShouldBe(fingerprint);
            options.Oci.Region.ShouldBe(region);
        }

        #endregion

        #region Testes de EnumKeyVault

        [Fact]
        public void Dado_EnumKeyVault_Quando_VerificarValores_Entao_DeveConterTodosValoresEsperados()
        {
            // Dado & Quando & Então
            ((int)EnumKeyVault.None).ShouldBe(-1);
            ((int)EnumKeyVault.Azure).ShouldBe(0);
            ((int)EnumKeyVault.OCI).ShouldBe(1);
        }

        [Fact]
        public void Dado_EnumKeyVault_Quando_VerificarDescricoes_Entao_DeveConterDescricoesCorretas()
        {
            // Dado
            var noneField = typeof(EnumKeyVault).GetField("None");
            var azureField = typeof(EnumKeyVault).GetField("Azure");
            var ociField = typeof(EnumKeyVault).GetField("OCI");

            // Quando
            var noneDescription = noneField?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            var azureDescription = azureField?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            var ociDescription = ociField?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            // Então
            noneDescription?.Length.ShouldBe(1);
            noneDescription?[0].Description.ShouldBe("None");

            azureDescription?.Length.ShouldBe(1);
            azureDescription?[0].Description.ShouldBe("Azure");

            ociDescription?.Length.ShouldBe(1);
            ociDescription?[0].Description.ShouldBe("OCI");
        }

        [Theory]
        [InlineData(EnumKeyVault.None, "None")]
        [InlineData(EnumKeyVault.Azure, "Azure")]
        [InlineData(EnumKeyVault.OCI, "OCI")]
        public void Dado_ValorEnum_Quando_ConverterParaString_Entao_DeveRetornarNomeCorreto(EnumKeyVault valor, string nomeEsperado)
        {
            // Dado & Quando
            var nome = valor.ToString();

            // Então
            nome.ShouldBe(nomeEsperado);
        }

        #endregion

        #region Testes de Cenários Complexos

        [Fact]
        public void Dado_ConfiguracaoCompleta_Quando_DefinirTodasPropriedades_Entao_DeveManterTodosValores()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var endpoint = new Uri("https://test.vault.azure.net/");

            // Quando
            options.Provider = EnumKeyVault.Azure;
            options.Endpoint = endpoint;

            options.Azure.ApplicationId = "app-123";
            options.Azure.TenantId = "tenant-456";
            options.Azure.ClientSecret = "secret-789";

            options.Oci.SecretId = "oci-secret-123";
            options.Oci.VaultId = "oci-vault-456";
            options.Oci.Profile = "PRODUCTION";

            // Então
            options.Provider.ShouldBe(EnumKeyVault.Azure);
            options.Endpoint.ShouldBe(endpoint);

            options.Azure.ApplicationId.ShouldBe("app-123");
            options.Azure.TenantId.ShouldBe("tenant-456");
            options.Azure.ClientSecret.ShouldBe("secret-789");

            options.Oci.SecretId.ShouldBe("oci-secret-123");
            options.Oci.VaultId.ShouldBe("oci-vault-456");
            options.Oci.Profile.ShouldBe("PRODUCTION");
        }

        [Fact]
        public void Dado_MultiplasMudancasDeProvider_Quando_AlterarProvider_Entao_DeveManterUltimoValor()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Provider = EnumKeyVault.Azure;
            options.Provider.ShouldBe(EnumKeyVault.Azure);

            options.Provider = EnumKeyVault.OCI;
            options.Provider.ShouldBe(EnumKeyVault.OCI);

            options.Provider = EnumKeyVault.None;

            // Então
            options.Provider.ShouldBe(EnumKeyVault.None);
        }

        [Fact]
        public void Dado_ConfiguracaoAzureEOci_Quando_DefinirAmbas_Entao_DeveManterAmbasIndependentes()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Azure.ApplicationId = "azure-app";
            options.Azure.TenantId = "azure-tenant";

            options.Oci.SecretId = "oci-secret";
            options.Oci.VaultId = "oci-vault";

            // Então
            options.Azure.ApplicationId.ShouldBe("azure-app");
            options.Azure.TenantId.ShouldBe("azure-tenant");
            options.Oci.SecretId.ShouldBe("oci-secret");
            options.Oci.VaultId.ShouldBe("oci-vault");
        }

        #endregion

        #region Testes de Validação de Tipos

        [Fact]
        public void Dado_EafKeyVaultOptions_Quando_VerificarTipos_Entao_DeveImplementarIOptions()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando & Então
            options.ShouldBeAssignableTo<Microsoft.Extensions.Options.IOptions<EafKeyVaultOptions>>();
        }

        [Fact]
        public void Dado_AzureKeyVaultOptions_Quando_VerificarTipo_Entao_DeveSerClassePublica()
        {
            // Dado
            var azureOptions = new AzureKeyVaultOptions();

            // Quando & Então
            azureOptions.ShouldNotBeNull();
            azureOptions.GetType().IsPublic.ShouldBeTrue();
        }

        [Fact]
        public void Dado_OciKeyVaultOptions_Quando_VerificarTipo_Entao_DeveSerClassePublica()
        {
            // Dado
            var ociOptions = new OciKeyVaultOptions();

            // Quando & Então
            ociOptions.ShouldNotBeNull();
            ociOptions.GetType().IsPublic.ShouldBeTrue();
        }

        [Fact]
        public void Dado_EnumKeyVault_Quando_VerificarTipo_Entao_DeveSerEnumPublico()
        {
            // Dado
            var enumType = typeof(EnumKeyVault);

            // Quando & Então
            enumType.IsEnum.ShouldBeTrue();
            enumType.IsPublic.ShouldBeTrue();
        }

        #endregion

        #region Testes de Valores Extremos

        [Fact]
        public void Dado_StringsVazias_Quando_DefinirPropriedades_Entao_DeveAceitarStringsVazias()
        {
            // Dado
            var options = new EafKeyVaultOptions();

            // Quando
            options.Azure.ApplicationId = "";
            options.Azure.TenantId = "";
            options.Azure.ClientSecret = "";

            options.Oci.SecretId = "";
            options.Oci.VaultId = "";
            options.Oci.Profile = "";

            // Então
            options.Azure.ApplicationId.ShouldBe("");
            options.Azure.TenantId.ShouldBe("");
            options.Azure.ClientSecret.ShouldBe("");

            options.Oci.SecretId.ShouldBe("");
            options.Oci.VaultId.ShouldBe("");
            options.Oci.Profile.ShouldBe("");
        }

        [Fact]
        public void Dado_StringsLongas_Quando_DefinirPropriedades_Entao_DeveAceitarStringsLongas()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var stringLonga = new string('A', 1000);

            // Quando
            options.Azure.ApplicationId = stringLonga;
            options.Oci.SecretId = stringLonga;

            // Então
            options.Azure.ApplicationId.ShouldBe(stringLonga);
            options.Azure.ApplicationId.Length.ShouldBe(1000);
            options.Oci.SecretId.ShouldBe(stringLonga);
            options.Oci.SecretId.Length.ShouldBe(1000);
        }

        [Fact]
        public void Dado_CaracteresEspeciais_Quando_DefinirPropriedades_Entao_DeveAceitarCaracteresEspeciais()
        {
            // Dado
            var options = new EafKeyVaultOptions();
            var stringEspecial = "!@#$%^&*()_+-=[]{}|;':\",./<>?`~";

            // Quando
            options.Azure.ApplicationId = stringEspecial;
            options.Oci.SecretId = stringEspecial;

            // Então
            options.Azure.ApplicationId.ShouldBe(stringEspecial);
            options.Oci.SecretId.ShouldBe(stringEspecial);
        }

        #endregion
    }
}