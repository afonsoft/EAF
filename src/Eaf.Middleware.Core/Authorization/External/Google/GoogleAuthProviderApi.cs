using Abp;
using Castle.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Eaf.Middleware.Core.Authentication.External.Google
{
    /// <summary>
    /// Representa a classe GoogleAuthProviderApi.
    /// </summary>
    public class GoogleAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "Google";
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// GoogleAuthProviderApi.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="httpClientFactory">Fábrica de HttpClient para evitar socket exhaustion.</param>
        /// <returns>Resultado da operação.</returns>
        public GoogleAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
        {
            Logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// GetUserInfo.
        /// </summary>
        /// <param name="accessCode">Parâmetro accessCode.</param>
        /// <returns>Resultado da operação.</returns>
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            string additionalParam = this.ProviderInfo.AdditionalParams["UserInfoEndpoint"];
            if (string.IsNullOrEmpty(additionalParam))
                throw new AbpException("Authentication:Google:UserInfoEndpoint configuration is required.");

            using var client = CreateExternalAuthClient(_httpClientFactory);
            JObject user = await GetUserInfoAsync(client, additionalParam, accessCode);
            var externalAuthUserInfo = new ExternalAuthUserInfo()
            {
                Name = GoogleHelper.GetName(user),
                EmailAddress = GoogleHelper.GetEmail(user),
                Surname = GoogleHelper.GetFamilyName(user),
                ProviderKey = GoogleHelper.GetId(user),
                Provider = "Google",
                AccessCode = accessCode,
                Object = user
            };

            return externalAuthUserInfo;
        }
    }
}
