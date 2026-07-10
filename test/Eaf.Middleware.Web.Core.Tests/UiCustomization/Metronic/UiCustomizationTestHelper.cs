using Abp;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Tests.WebCore.UiCustomization.Metronic
{
    internal static class UiCustomizationTestHelper
    {
        public static string DefaultValueFor(string name, string themeName)
        {
            if (name.EndsWith("UiManagement.Theme"))
                return themeName;

            if (name.Contains("Fixed") || name.Contains("Allow") || name.Contains("Default"))
                return "true";

            return themeName;
        }

        public static SettingManager CreateSettingManager(string themeName)
        {
            var definitionManager = Substitute.For<ISettingDefinitionManager>();
            definitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci =>
                new SettingDefinition(ci.Arg<string>(), DefaultValueFor(ci.Arg<string>(), themeName)));

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
    }
}
