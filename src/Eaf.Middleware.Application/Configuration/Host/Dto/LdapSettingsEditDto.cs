using Abp.Auditing;

namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe LdapSettingsEditDto.
    /// </summary>
    public class LdapSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define Domain.
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Obtém ou define IsEnabled.
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsModuleEnabled.
        /// </summary>
        public bool IsModuleEnabled { get; set; }

        [DisableAuditing]
        public string Password { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}