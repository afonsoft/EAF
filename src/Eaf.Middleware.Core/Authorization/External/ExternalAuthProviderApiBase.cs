using Castle.Core.Logging;
using Abp.Dependency;
using System.Threading.Tasks;

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
    }
}