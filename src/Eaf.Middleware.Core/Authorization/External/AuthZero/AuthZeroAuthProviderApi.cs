using Abp;
using Abp.Extensions;
using Castle.Core.Logging;
using Eaf.Middleware.Authorization.External.AuthZero;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Authentication.External.AuthZero
{
    /// <summary>
    /// Representa a classe AuthZeroAuthProviderApi.
    /// </summary>
    public class AuthZeroAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "AuthZero";
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// AuthZeroAuthProviderApi.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <param name="httpClientFactory">Fábrica de HttpClient para evitar socket exhaustion.</param>
        /// <returns>Resultado da operação.</returns>
        public AuthZeroAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
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
            string additionalParam = this.ProviderInfo.AdditionalParams["Endpoint"];
            if (string.IsNullOrEmpty(additionalParam))
                throw new AbpException("Authentication:AuthZero:Endpoint configuration is required.");
            string domain = additionalParam;

            if (!additionalParam.Contains("https"))
                domain = $"https://{additionalParam}";

            domain = domain.RemovePostFix("/");

            ExternalAuthUserInfo externalAuthUserInfo;
            using var client = _httpClientFactory.CreateClient("ExternalAuth");
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.Timeout = TimeSpan.FromSeconds(30.0);
                client.MaxResponseContentBufferSize = 10485760L;

                HttpResponseMessage httpResponseMessage = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, domain + "/userinfo")
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessCode) }
                });

                httpResponseMessage.EnsureSuccessStatusCode();
                JObject user = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                externalAuthUserInfo = new ExternalAuthUserInfo()
                {
                    Name = AuthZeroAccountHelper.GetDisplayName(user),
                    EmailAddress = AuthZeroAccountHelper.GetEmail(user),
                    Surname = AuthZeroAccountHelper.GetSurname(user),
                    Provider = "AuthZero",
                    ProviderKey = AuthZeroAccountHelper.GetId(user),
                    AccessCode = accessCode,
                    Picture = AuthZeroAccountHelper.GetPicture(user),
                    Object = user,
                };

                try
                {
                    if (!externalAuthUserInfo.Picture.IsNullOrEmpty())
                    {
                        var bytes = await client.GetByteArrayAsync(externalAuthUserInfo.Picture);
                        if (bytes != null && bytes.Any())
                            externalAuthUserInfo.Picture = Convert.ToBase64String(bytes);
                    }
                }
                catch (Exception ex)
                {
                    externalAuthUserInfo.Picture = "";
                    Logger.DebugFormat(ex, "Error on retrive Profile Picture {0}", externalAuthUserInfo.EmailAddress);
                }
            }
            return externalAuthUserInfo;
        }
    }
}