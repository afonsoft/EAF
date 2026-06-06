using Castle.Core.Logging;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
            ExternalAuthUserInfo externalAuthUserInfo;
            using var client = _httpClientFactory.CreateClient("ExternalAuth");
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.Timeout = TimeSpan.FromSeconds(30.0);
                client.MaxResponseContentBufferSize = 10485760L;

                HttpResponseMessage httpResponseMessage = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, MicrosoftAccountDefaults.UserInformationEndpoint)
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessCode) }
                });

                httpResponseMessage.EnsureSuccessStatusCode();
                JObject user = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                externalAuthUserInfo = new ExternalAuthUserInfo()
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
                    httpResponseMessage = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, MicrosoftAccountDefaults.UserInformationEndpoint + "/photo/$value")
                    {
                        Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessCode) }
                    });
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                        byte[] bytes = new byte[stream.Length];
                        int bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length);
                        externalAuthUserInfo.Picture = Convert.ToBase64String(bytes, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat(ex, "Error on retrive Profile Picture {0}", externalAuthUserInfo.EmailAddress);
                }
            }
            return externalAuthUserInfo;
        }
    }
}