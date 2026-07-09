using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Features;
using Eaf.Middleware.MultiTenancy;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Features
{
    public class FeatureValueStoreBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_CriarFeatureValueStore_Entao_DeveInicializarCorretamente()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            var tenantFeatureSettingRepository = Substitute.For<IRepository<TenantFeatureSetting, long>>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var editionFeatureRepository = Substitute.For<IRepository<EditionFeatureSetting, long>>();
            var featureManager = Substitute.For<IFeatureManager>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();

            var store = new FeatureValueStore(
                cacheManager,
                tenantFeatureSettingRepository,
                tenantRepository,
                editionFeatureRepository,
                featureManager,
                unitOfWorkManager
            );

            store.ShouldNotBeNull();
            store.ShouldBeOfType<FeatureValueStore>();
        }
    }
}
