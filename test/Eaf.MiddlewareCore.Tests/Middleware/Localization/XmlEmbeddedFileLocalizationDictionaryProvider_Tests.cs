using Abp;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Localization
{
    public class MyCustomXmlLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Clear();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AbpZeroCommonModule).GetAssembly(), "Eaf.Middleware.Localization.Source"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomXmlLangModule).GetAssembly(), "Eaf.Middleware.Localization.Sources.Extensions.Xml.Eaf"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomXmlLangModule).GetAssembly(), "Eaf.Middleware.Localization.Sources.Extensions.Xml.EafMiddleware"
                    )
                )
            );
        }
    }

    public class XmlEmbeddedFileLocalizationDictionaryProvider_Tests : AbpIntegratedTestBase<MyCustomXmlLangModule>
    {
        [Fact]
        public void Test_Xml_Override()
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
}