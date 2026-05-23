using Abp.Auditing;

namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe EmailSettingsEditDto.
    /// </summary>
    public class EmailSettingsEditDto
    {
        //No validation is done, since we may don't want to use email system.

        /// <summary>
        /// Obtém ou define DefaultFromAddress.
        /// </summary>
        public string DefaultFromAddress { get; set; }

        /// <summary>
        /// Obtém ou define DefaultFromDisplayName.
        /// </summary>
        public string DefaultFromDisplayName { get; set; }

        /// <summary>
        /// Obtém ou define SmtpDomain.
        /// </summary>
        public string SmtpDomain { get; set; }
        /// <summary>
        /// Obtém ou define SmtpEnableSsl.
        /// </summary>
        public bool SmtpEnableSsl { get; set; }
        /// <summary>
        /// Obtém ou define SmtpHost.
        /// </summary>
        public string SmtpHost { get; set; }

        [DisableAuditing]
        public string SmtpPassword { get; set; }

        /// <summary>
        /// Obtém ou define SmtpPort.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Obtém ou define SmtpUseDefaultCredentials.
        /// </summary>
        public bool SmtpUseDefaultCredentials { get; set; }
        /// <summary>
        /// Obtém ou define SmtpUserName.
        /// </summary>
        public string SmtpUserName { get; set; }
    }
}