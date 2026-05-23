using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Core.Authentication.External;

namespace Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders
{
    /// <summary>
    /// Representa a classe TenantBasedExternalLoginInfoProviderBase.
    /// </summary>
    public abstract class TenantBasedExternalLoginInfoProviderBase : IExternalLoginInfoProvider
    {
        private readonly ICacheManager _cacheManager;
        private readonly IAbpSession _AbpSession;

        protected TenantBasedExternalLoginInfoProviderBase(
            IAbpSession eafSession,
            ICacheManager cacheManager)
        {
            _AbpSession = eafSession;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// GetExternalLoginInfo.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual ExternalLoginProviderInfo GetExternalLoginInfo()
        {
            if (_AbpSession.TenantId.HasValue && TenantHasSettings())
            {
                return _cacheManager.GetExternalLoginInfoProviderCache()
                    .Get(GetCacheKey(), GetTenantInformation);
            }

            return _cacheManager.GetExternalLoginInfoProviderCache()
                    .Get(GetCacheKey(), GetHostInformation);
        }

        protected abstract ExternalLoginProviderInfo GetHostInformation();

        protected abstract ExternalLoginProviderInfo GetTenantInformation();

        protected abstract bool TenantHasSettings();

        private string GetCacheKey()
        {
            if (_AbpSession.TenantId.HasValue)
            {
                return $"{Name}-{_AbpSession.TenantId.Value}";
            }

            return $"{Name}";
        }
    }
}