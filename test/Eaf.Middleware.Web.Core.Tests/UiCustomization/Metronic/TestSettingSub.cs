using Abp;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.UiCustomization.Metronic
{
    public class TestSettingSub
    {
        private static string DefaultValueFor(string name)
        {
            if (name.Contains("Fixed") || name.Contains("Allow") || name.Contains("Default"))
                return "true";
            return "theme2";
        }

        private static SettingManager CreateSettingManager()
        {
            var definitionManager = Substitute.For<ISettingDefinitionManager>();
            definitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci =>
                new SettingDefinition(ci.Arg<string>(), DefaultValueFor(ci.Arg<string>())));

            var emptyDictionary = new Dictionary<string, SettingInfo>();

            var cache = Substitute.For<ICache>();
            cache.GetAsync("ApplicationSettings", Arg.Any<Func<string, Task<object>>>())
                .Returns(Task.FromResult((object)emptyDictionary));

            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("AbpApplicationSettingsCache").Returns(cache);

            var multiTenancyConfig = Substitute.For<IMultiTenancyConfig>();
            multiTenancyConfig.IsEnabled.Returns(true);

            var tenantStore = Substitute.For<ITenantStore>();
            var settingEncryptionService = Substitute.For<ISettingEncryptionService>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();

            return Substitute.For<SettingManager>(new object[]
            {
                definitionManager,
                cacheManager,
                multiTenancyConfig,
                tenantStore,
                settingEncryptionService,
                unitOfWorkManager
            });
        }

        [Fact]
        public async Task Dado_CriarSettingManager_Quando_ChamarGetSettingValueAsync_Entao_DeveRetornarValorPadrao()
        {
            var sm = CreateSettingManager();

            var result = await sm.GetSettingValueAsync("theme2.App.UiManagement.Theme");

            result.ShouldBe("theme2");
        }

        [Fact]
        public async Task Dado_CriarSettingManager_Quando_ChamarGetSettingValueAsyncBool_Entao_DeveRetornarBooleanoConvertido()
        {
            var sm = CreateSettingManager();

            var result = await sm.GetSettingValueAsync<bool>("theme2.App.UiManagement.LeftAside.FixedAside");

            result.ShouldBeTrue();
        }
    }
}
