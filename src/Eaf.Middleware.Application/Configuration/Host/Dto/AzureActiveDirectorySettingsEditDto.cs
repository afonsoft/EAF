namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe AzureActiveDirectorySettingsEditDto.
    /// </summary>
    public class AzureActiveDirectorySettingsEditDto
    {
        /// <summary>
        /// Obtém ou define ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Obtém ou define ClientSecret.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Obtém ou define IsEnabled.
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsModuleEnabled.
        /// </summary>
        public bool IsModuleEnabled { get; set; }
        /// <summary>
        /// Obtém ou define Tenant.
        /// </summary>
        public string Tenant { get; set; }
    }
}