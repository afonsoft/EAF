using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;

namespace Eaf.Middleware.Features
{
    /// <summary>
    /// Representa a classe FeatureValueStore.
    /// </summary>
    public class FeatureValueStore : AbpFeatureValueStore<Tenant, User>
    {
        /// <summary>
        /// FeatureValueStore.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <param name="tenantFeatureSettingRepository">Parâmetro tenantFeatureSettingRepository.</param>
        /// <param name="tenantRepository">Parâmetro tenantRepository.</param>
        /// <param name="editionFeatureRepository">Parâmetro editionFeatureRepository.</param>
        /// <param name="featureManager">Parâmetro featureManager.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <returns>Resultado da operação.</returns>
        public FeatureValueStore(
            ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureSettingRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(cacheManager,
                  tenantFeatureSettingRepository,
                  tenantRepository,
                  editionFeatureRepository,
                  featureManager,
                  unitOfWorkManager)
        {
        }
    }
}