using Newtonsoft.Json.Linq;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe ExternalAuthUserInfo.
    /// </summary>
    public class ExternalAuthUserInfo
    {
        /// <summary>
        /// Obtém ou define EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define Provider.
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// Obtém ou define ProviderKey.
        /// </summary>
        public string ProviderKey { get; set; }
        /// <summary>
        /// Obtém ou define Surname.
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Obtém ou define Picture.
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// Obtém ou define AccessCode.
        /// </summary>
        public string AccessCode { get; set; }
        /// <summary>
        /// Obtém ou define Object.
        /// </summary>
        public JObject Object { get; set; }
    }
}