using Microsoft.IdentityModel.Tokens;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Representa a classe TokenAuthConfiguration.
    /// </summary>
    public class TokenAuthConfiguration
    {
        /// <summary>
        /// Obtém ou define Audience.
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// Obtém ou define Issuer.
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// Obtém ou define SecurityKey.
        /// </summary>
        public SymmetricSecurityKey SecurityKey { get; set; }
        /// <summary>
        /// Obtém ou define SigningCredentials.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}