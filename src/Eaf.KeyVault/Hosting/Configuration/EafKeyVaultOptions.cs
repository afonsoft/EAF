using Microsoft.Extensions.Options;
using Oci.Common.Auth;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Eaf.KeyVault
{
    /// <summary>
    /// Representa a classe EafKeyVaultOptions.
    /// </summary>
    public class EafKeyVaultOptions : IOptions<EafKeyVaultOptions>
    {
        /// <summary>
        /// EafKeyVaultOptions.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public EafKeyVaultOptions()
        {
            Provider = EnumKeyVault.None;
            Azure = new AzureKeyVaultOptions();
            Oci = new OciKeyVaultOptions();
        }

        public EafKeyVaultOptions Value => this;

        /// <summary>
        /// KeyVaultProvider OCI or Azure,default is None. (Disabled)
        /// </summary>
        public EnumKeyVault Provider { get; set; }

        /// <summary>
        /// Config for Azure Key Vault
        /// </summary>
        public AzureKeyVaultOptions Azure { get; set; }

        /// <summary>
        /// Config for OCI
        /// </summary>
        public OciKeyVaultOptions Oci { get; set; }

        /// <summary>
        /// Endpoint to ocnnection on Key Vault Provider
        /// </summary>
        public Uri Endpoint { get; set; }
    }

    /// <summary>
    /// Representa a classe AzureKeyVaultOptions.
    /// </summary>
    public class AzureKeyVaultOptions
    {
        /// <summary>
        /// Certificate for conection on Azure Key Vault
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// clientId (ApplicationId)
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// TenantId of Azure
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Secret for Azure
        /// </summary>
        public string ClientSecret { get; set; }
    }

    /// <summary>
    /// Representa a classe OciKeyVaultOptions.
    /// </summary>
    public class OciKeyVaultOptions
    {
        /// <summary>
        /// SecretId (OCID)
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// VaultId for get by Name
        /// </summary>
        public string VaultId { get; set; }

        /// <summary>
        /// Profile OCI config file DEFAULT
        /// </summary>
        public string Profile { get; set; } = "DEFAULT";

        /// <summary>
        /// Path of Config for OCI
        /// </summary>
        public string ConfigFile { get; set; }

        /// <summary>
        /// TenantId of config use if not have de config file
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// UserId of config use if not have de config file
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Fingerprint of config use if not have de config file
        /// </summary>
        public string Fingerprint { get; set; }

        /// <summary>
        /// Region of config use if not have de config file
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// KeySupplier of config use if not have de config file
        /// </summary>
        public ISupplier<RsaKeyParameters> KeySupplier { get; set; }
    }

    /// <summary>
    /// KeyVault Type
    /// </summary>
    public enum EnumKeyVault
    {
        [Description("None")]
        None = -1,

        [Description("Azure")]
        Azure = 0,

        [Description("OCI")]
        OCI = 1
    }
}