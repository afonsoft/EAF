namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe ImpersonatedAuthenticateResultModel.
    /// </summary>
    public class ImpersonatedAuthenticateResultModel
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
    }
}