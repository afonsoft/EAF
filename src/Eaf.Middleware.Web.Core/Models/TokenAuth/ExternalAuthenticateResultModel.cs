namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe ExternalAuthenticateResultModel.
    /// </summary>
    public class ExternalAuthenticateResultModel
    {
        /// <summary>
        /// Obtém ou define AccessToken.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Obtém ou define EncryptedAccessToken.
        /// </summary>
        public string EncryptedAccessToken { get; set; }

        /// <summary>
        /// Obtém ou define ExpireInSeconds.
        /// </summary>
        public int ExpireInSeconds { get; set; }

        /// <summary>
        /// Obtém ou define ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// Obtém ou define WaitingForActivation.
        /// </summary>
        public bool WaitingForActivation { get; set; }

        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }
    }
}