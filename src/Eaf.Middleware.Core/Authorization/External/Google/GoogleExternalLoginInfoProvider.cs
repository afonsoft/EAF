using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.External.Google
{
    /// <summary>
    /// Representa a classe GoogleExternalLoginInfoProvider.
    /// </summary>
    public class GoogleExternalLoginInfoProvider : IExternalLoginInfoProvider
    {
        /// <summary>
        /// GoogleExternalLoginInfoProvider.
        /// </summary>
        /// <param name="clientId">Parâmetro clientId.</param>
        /// <param name="clientSecret">Parâmetro clientSecret.</param>
        /// <param name="userInfoEndpoint">Parâmetro userInfoEndpoint.</param>
        /// <returns>Resultado da operação.</returns>
        public GoogleExternalLoginInfoProvider(
          string clientId,
          string clientSecret,
          string userInfoEndpoint)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.UserInfoEndpoint = userInfoEndpoint;
            this.CreateExternalLoginInfo();
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; } = "Google";

        protected string ClientId { get; set; }

        protected string ClientSecret { get; set; }

        protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }
        protected string UserInfoEndpoint { get; set; }

        /// <summary>
        /// GetExternalLoginInfo.
        /// </summary>
        public virtual ExternalLoginProviderInfo GetExternalLoginInfo() => this.ExternalLoginProviderInfo;

        private void CreateExternalLoginInfo() => this.ExternalLoginProviderInfo = new ExternalLoginProviderInfo("Google", this.ClientId, this.ClientSecret, null, typeof(GoogleAuthProviderApi), new Dictionary<string, string>()
    {
      {
        "UserInfoEndpoint",
        this.UserInfoEndpoint
      }
    });
    }
}