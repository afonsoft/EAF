using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.AuthZero;

namespace Eaf.Middleware.Authorization.External.AuthZero
{
    /// <summary>
    /// Representa a classe AuthZeroExternalLoginInfoProvider.
    /// </summary>
    public class AuthZeroExternalLoginInfoProvider : IExternalLoginInfoProvider
    {
        /// <summary>
        /// AuthZeroExternalLoginInfoProvider.
        /// </summary>
        /// <param name="consumerKey">Parâmetro consumerKey.</param>
        /// <param name="consumerSecret">Parâmetro consumerSecret.</param>
        /// <param name="consumerTenant">Parâmetro consumerTenant.</param>
        /// <returns>Resultado da operação.</returns>
        public AuthZeroExternalLoginInfoProvider(string consumerKey, string consumerSecret, string consumerTenant)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.ConsumerTenant = consumerTenant;
            this.CreateExternalLoginInfo();
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; } = "AuthZero";

        protected string ConsumerKey { get; set; }

        protected string ConsumerSecret { get; set; }
        protected string ConsumerTenant { get; set; }

        protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }

        /// <summary>
        /// GetExternalLoginInfo.
        /// </summary>
        public virtual ExternalLoginProviderInfo GetExternalLoginInfo() => this.ExternalLoginProviderInfo;

        private void CreateExternalLoginInfo() => this.ExternalLoginProviderInfo = new ExternalLoginProviderInfo("AuthZero", this.ConsumerKey, this.ConsumerSecret, this.ConsumerTenant, typeof(AuthZeroAuthProviderApi));
    }
}