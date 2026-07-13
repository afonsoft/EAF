using Castle.Core.Logging;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Eaf.Middleware.Core.Authentication.External.Microsoft
{
    /// <summary>
    /// Representa a classe MicrosoftAuthProviderApi.
    /// </summary>
    public class MicrosoftAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "Microsoft";
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// MicrosoftAuthProviderApi.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="httpClientFactory">Fábrica de HttpClient para evitar socket exhaustion.</param>
        /// <returns>Resultado da operação.</returns>
        public MicrosoftAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
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
            using var client = CreateExternalAuthClient(_httpClientFactory);
            JObject user = await GetUserInfoAsync(client, MicrosoftAccountDefaults.UserInformationEndpoint, accessCode);
            var externalAuthUserInfo = new ExternalAuthUserInfo()
            {
                Name = MicrosoftAccountHelper.GetDisplayName(user),
                EmailAddress = MicrosoftAccountHelper.GetEmail(user),
                Surname = MicrosoftAccountHelper.GetSurname(user),
                Provider = "Microsoft",
                ProviderKey = MicrosoftAccountHelper.GetId(user),
                AccessCode = accessCode,
                Object = user,
            };

            try
            {
                var bytes = await GetUserBytesAsync(client, MicrosoftAccountDefaults.UserInformationEndpoint + "/photo/$value", accessCode);
                externalAuthUserInfo.Picture = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "Error on retrive Profile Picture {0}", externalAuthUserInfo.EmailAddress);
            }

            return externalAuthUserInfo;
        }
    }
}
