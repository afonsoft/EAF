using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using System.Reflection;

namespace Eaf.Middleware.AzureActiveDirectory
{
    /// <summary>
    /// This module extends module middleware to add AzureActiveDirectory authentication.
    /// </summary>
    [DependsOn(typeof(AbpZeroCommonModule))]
    public class EafMiddlewareAzureActiveDirectoryModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.Register<IEafMiddlewareAzureActiveDirectoryModuleConfig, EafMiddlewareAzureActiveDirectoryModuleConfig>();

            Configuration.Localization.Sources.Add(
              new DictionaryBasedLocalizationSource(
                  "EafAzureActiveDirectory",
                  new XmlEmbeddedFileLocalizationDictionaryProvider(
                      typeof(EafMiddlewareAzureActiveDirectoryModule).GetAssembly(),
                      "Eaf.Middleware.AzureActiveDirectory.Localization.Source"
                  )
              )
          );

            Configuration.Settings.Providers.Add<AzureActiveDirectorySettingProvider>();
        }
    }
}