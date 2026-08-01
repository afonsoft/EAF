using Eaf.Configuration.Host.Dto;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe HostSettingsEditDto.
    /// </summary>
    public class HostSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define AzureActiveDirectory.
        /// </summary>
        public AzureActiveDirectorySettingsEditDto AzureActiveDirectory { get; set; }

        [Required]
        public EmailSettingsEditDto Email { get; set; }

        /// <summary>
        /// Obtém ou define ExternalLoginProviderSettings.
        /// </summary>
        public ExternalLoginProviderSettingsEditDto ExternalLoginProviderSettings { get; set; }

        [Required]
        public GeneralSettingsEditDto General { get; set; }

        /// <summary>
        /// Obtém ou define Google.
        /// </summary>
        public GoogleSettingsEditDto Google { get; set; }

        /// <summary>
        /// Obtém ou define Ldap.
        /// </summary>
        public LdapSettingsEditDto Ldap { get; set; }

        [Required]
        public SecuritySettingsEditDto Security { get; set; }

        [Required]
        public TenantManagementSettingsEditDto TenantManagement { get; set; }

        [Required]
        public HostUserManagementSettingsEditDto UserManagement { get; set; }

        /// <summary>
        /// Obtém ou define LogDeleter.
        /// </summary>
        public ExpiredEntityLogDeleterSettingsEditDto LogDeleter { get; set; }

        /// <summary>
        /// Obtém ou define LoginImpersonator.
        /// </summary>
        public ExpiredEntityLoginImpersonatorSettingsEditDto LoginImpersonator { get; set; }
    }
}