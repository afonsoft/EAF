namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ResolveTenantIdInput.
    /// </summary>
    public class ResolveTenantIdInput
    {
        // An encrypted text which contains tenantId={value} string
        /// <summary>
        /// Obtém ou define c.
        /// </summary>
        public string c { get; set; }
    }
}