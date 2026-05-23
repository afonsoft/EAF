using Abp;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Localization
{
    public class JsonEmbeddedFileLocalizationDictionaryProvider_Tests : AbpIntegratedTestBase<MyCustomJsonLangModule>
    {
        [Fact]
        public void Test_Json_Override()
        {
            var mananger = LocalIocManager.Resolve<ILocalizationManager>();

            using (CultureInfoHelper.Use("en"))
            {
                var eafSource = mananger.GetSource(AbpConsts.LocalizationSourceName);
                eafSource.GetString("TimeZone").ShouldBe("[Time zone]");

                var eafMiddlewareSource = mananger.GetSource(AbpConsts.LocalizationSourceName);
                eafMiddlewareSource.GetString("Email").ShouldBe("[Email]");
            }
        }
    }

    public class MyCustomJsonLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Clear();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(),
                        "Eaf.Middleware.Localization.Sources.Base.Eaf"
                    )
                )
            );

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    "EafMiddleware",
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(),
                        "Eaf.Middleware.Localization.Sources.Base.EafMiddleware"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(), "Eaf.Middleware.Localization.Sources.Extensions.Json.Eaf"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(), "Eaf.Middleware.Localization.Sources.Extensions.Json.EafMiddleware"
                    )
                )
            );
        }
    }
}