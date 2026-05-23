namespace Eaf.Middleware.Sessions.Dto
{
    /// <summary>
    /// Representa a classe UpdateUserSignInTokenOutput.
    /// </summary>
    public class UpdateUserSignInTokenOutput
    {
        /// <summary>
        /// Obtém ou define EncodedTenantId.
        /// </summary>
        public string EncodedTenantId { get; set; }
        /// <summary>
        /// Obtém ou define EncodedUserId.
        /// </summary>
        public string EncodedUserId { get; set; }
        /// <summary>
        /// Obtém ou define SignInToken.
        /// </summary>
        public string SignInToken { get; set; }
    }
}