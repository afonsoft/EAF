using Abp.Dependency;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe ExternalAuthManager.
    /// </summary>
    public class ExternalAuthManager : IExternalAuthManager, ITransientDependency
    {
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// ExternalAuthManager.
        /// </summary>
        /// <param name="iocResolver">Parâmetro iocResolver.</param>
        /// <param name="externalAuthConfiguration">Parâmetro externalAuthConfiguration.</param>
        /// <returns>Resultado da operação.</returns>
        public ExternalAuthManager(IIocResolver iocResolver, IExternalAuthConfiguration externalAuthConfiguration)
        {
            this._iocResolver = iocResolver;
            this._externalAuthConfiguration = externalAuthConfiguration;
        }

        /// <summary>
        /// CreateProviderApi.
        /// </summary>
        /// <param name="provider">Parâmetro provider.</param>
        /// <returns>Resultado da operação.</returns>
        public IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> CreateProviderApi(string provider)
        {
            ExternalLoginProviderInfo providerInfo = this._externalAuthConfiguration.ExternalLoginInfoProviders.FirstOrDefault(infoProvider => infoProvider.Name == provider)?.GetExternalLoginInfo();
            if (providerInfo == null)
                throw new ArgumentNullException("Unknown external auth provider: " + provider);

            IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> dependencyObjectWrapper = IocResolverExtensions.ResolveAsDisposable<IExternalAuthProviderApi>(this._iocResolver, providerInfo.ProviderApiType);
            dependencyObjectWrapper.Object.Initialize(providerInfo);
            return dependencyObjectWrapper;
        }

        /// <summary>
        /// GetUserInfo.
        /// </summary>
        /// <param name="provider">Parâmetro provider.</param>
        /// <param name="accessCode">Parâmetro accessCode.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<ExternalAuthUserInfo> GetUserInfo(string provider, string accessCode)
        {
            using (IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> providerApi = this.CreateProviderApi(provider))
                return providerApi.Object.GetUserInfo(accessCode);
        }

        /// <summary>
        /// IsValidUser.
        /// </summary>
        /// <param name="provider">Parâmetro provider.</param>
        /// <param name="providerKey">Parâmetro providerKey.</param>
        /// <param name="providerAccessCode">Parâmetro providerAccessCode.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode)
        {
            using (IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> providerApi = this.CreateProviderApi(provider))
                return providerApi.Object.IsValidUser(providerKey, providerAccessCode);
        }
    }
}