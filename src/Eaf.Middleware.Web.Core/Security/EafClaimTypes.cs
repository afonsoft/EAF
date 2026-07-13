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
        public static string UserIdentifierClaimType { get; set; } = "https://aspnetzero.com/claims/useridentifier";

        /// <summary>
        /// User ExternalAuthProviderformation.
        /// Default: https://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation
        /// </summary>
        public static string ExternalAuthProviderformation { get; set; } = "https://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation";

    }
}
