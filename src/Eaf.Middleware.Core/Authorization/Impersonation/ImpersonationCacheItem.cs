using System;

namespace Eaf.Middleware.Authorization.Impersonation
{
    [Serializable]
    public class ImpersonationCacheItem
    {
        public const string CacheName = "AppImpersonationCache";

        /// <summary>
        /// ImpersonationCacheItem.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public ImpersonationCacheItem()
        {
        }

        /// <summary>
        /// ImpersonationCacheItem.
        /// </summary>
        /// <param name="targetTenantId">Parâmetro targetTenantId.</param>
        /// <param name="targetUserId">Parâmetro targetUserId.</param>
        /// <param name="isBackToImpersonator">Parâmetro isBackToImpersonator.</param>
        /// <returns>Resultado da operação.</returns>
        public ImpersonationCacheItem(int? targetTenantId, long targetUserId, bool isBackToImpersonator)
        {
            TargetTenantId = targetTenantId;
            TargetUserId = targetUserId;
            IsBackToImpersonator = isBackToImpersonator;
        }

        public int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// Obtém ou define ImpersonatorUserId.
        /// </summary>
        public long ImpersonatorUserId { get; set; }

        /// <summary>
        /// Obtém ou define IsBackToImpersonator.
        /// </summary>
        public bool IsBackToImpersonator { get; set; }
        public int? TargetTenantId { get; set; }

        /// <summary>
        /// Obtém ou define TargetUserId.
        /// </summary>
        public long TargetUserId { get; set; }
    }
}