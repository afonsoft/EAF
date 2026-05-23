namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe UserLockOutSettingsEditDto.
    /// </summary>
    public class UserLockOutSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define DefaultAccountLockoutSeconds.
        /// </summary>
        public int DefaultAccountLockoutSeconds { get; set; }
        /// <summary>
        /// Obtém ou define IsEnabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Obtém ou define MaxFailedAccessAttemptsBeforeLockout.
        /// </summary>
        public int MaxFailedAccessAttemptsBeforeLockout { get; set; }
    }
}