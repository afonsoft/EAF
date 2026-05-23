namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ImpersonateOutput.
    /// </summary>
    public class ImpersonateOutput
    {
        /// <summary>
        /// Obtém ou define ImpersonationToken.
        /// </summary>
        public string ImpersonationToken { get; set; }

        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
    }
}