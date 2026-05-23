using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Eaf.Middleware.Ldap.Configuration;
using Abp.Modules;
using System;
using System.Reflection;
using Abp.Reflection.Extensions;
using Abp;
using Abp.Zero;
using Abp.Localization.Dictionaries;

namespace Eaf.Middleware.Ldap
{
    /// <summary>
    /// This module extends module middleware to add LDAP authentication.
    /// </summary>
    [DependsOn(typeof(AbpZeroCommonModule))]
    public class EafMiddlewareLdapModule : AbpModule
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
            IocManager.Register<IEafMiddlewareLdapModuleConfig, EafMiddlewareLdapModuleConfig>();

            Configuration.Localization.Sources.Add(
              new DictionaryBasedLocalizationSource(
                  "EafLdap",
                  new XmlEmbeddedFileLocalizationDictionaryProvider(
                      typeof(EafMiddlewareLdapModule).GetAssembly(),
                      "Eaf.Middleware.Ldap.Localization.Source"
                  )
              )
          );

            Configuration.Settings.Providers.Add<LdapSettingProvider>();
        }
    }
}