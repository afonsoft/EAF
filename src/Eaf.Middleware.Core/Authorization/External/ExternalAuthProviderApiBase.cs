using Castle.Core.Logging;
using Abp.Dependency;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe ExternalAuthProviderApiBase.
    /// </summary>
    public abstract class ExternalAuthProviderApiBase : IExternalAuthProviderApi, ITransientDependency
    {
        protected ExternalAuthProviderApiBase()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Obtém ou define Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Obtém ou define ProviderInfo.
        /// </summary>
        public ExternalLoginProviderInfo ProviderInfo { get; set; }

        /// <summary>
        /// GetUserInfo.
        /// </summary>
        /// <param name="accessCode">Parâmetro accessCode.</param>
        public abstract Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="providerInfo">Parâmetro providerInfo.</param>
        public void Initialize(ExternalLoginProviderInfo providerInfo) => this.ProviderInfo = providerInfo;

        /// <summary>
        /// IsValidUser.
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        /// <param name="accessCode">Parâmetro accessCode.</param>
        public async Task<bool> IsValidUser(string userId, string accessCode) => (await this.GetUserInfo(accessCode)).ProviderKey == userId;

        /// <summary>
        /// Cria e configura um <see cref="HttpClient" /> para chamadas OAuth externas.
        /// </summary>
        /// <param name="httpClientFactory">Fábrica de HttpClient.</param>
        /// <returns>HttpClient configurado.</returns>
        protected HttpClient CreateExternalAuthClient(IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("ExternalAuth");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.Timeout = TimeSpan.FromSeconds(30.0);
            client.MaxResponseContentBufferSize = 10485760L;
            return client;
        }

        /// <summary>
        /// Recupera informações do usuário a partir de um endpoint OAuth.
        /// </summary>
        /// <param name="client">HttpClient configurado.</param>
        /// <param name="url">URL do endpoint.</param>
        /// <param name="accessCode">Token de acesso.</param>
        /// <returns>Objeto JSON com as informações do usuário.</returns>
        protected async Task<JObject> GetUserInfoAsync(HttpClient client, string url, string accessCode)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessCode) }
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Recupera o conteúdo binário de um recurso externo.
        /// </summary>
        /// <param name="client">HttpClient configurado.</param>
        /// <param name="url">URL do recurso.</param>
        /// <param name="accessCode">Token de acesso (opcional).</param>
        /// <returns>Bytes do recurso.</returns>
        protected async Task<byte[]> GetUserBytesAsync(HttpClient client, string url, string accessCode = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(accessCode))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
