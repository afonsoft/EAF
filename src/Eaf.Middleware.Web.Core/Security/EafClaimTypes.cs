namespace Eaf.Security
{
    /// <summary>
    /// Used to get eaf-specific claim type names.
    /// </summary>
    public static class EafClaimTypes
    {
        /// <summary>
        /// Obtém ou define UserIdentifierClaimType.
        /// </summary>
        public static string UserIdentifierClaimType { get; set; } = "http://aspnetzero.com/claims/useridentifier";

        /// <summary>
        /// User ExternalAuthProviderformation.
        /// Default: http://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation
        /// </summary>
        public static string ExternalAuthProviderformation { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation";

    }
}
