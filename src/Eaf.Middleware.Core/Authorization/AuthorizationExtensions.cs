using Abp;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Representa a classe AuthorizationExtensions.
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// GetExternalTokenInformation.
        /// </summary>
        /// <param name="session">Parâmetro session.</param>
        /// <returns>Resultado da operação.</returns>
        public static string GetExternalTokenInformation(this IAbpSession session)
        {
            if (!session.UserId.HasValue)
                throw new AbpException("Session.UserId is null! Probably, user is not logged in.");
            var cacheManager = IocManager.Instance.Resolve<ICacheManager>();
            return (cacheManager.GetCache("ExternalTokenInformationCache").GetOrDefault(session.ToUserIdentifier().ToString()) ?? null) as string;
        }
    }
}