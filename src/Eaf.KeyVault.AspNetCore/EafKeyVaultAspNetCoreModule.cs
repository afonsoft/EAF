using Abp.Modules;
using Abp.Reflection.Extensions;
using System;

namespace Eaf.KeyVault.AspNetCore
{
    /// <summary>
    /// Módulo ABP que configura e inicializa EafKeyVaultAspNetCore.
    /// </summary>
    [DependsOn(typeof(EafKeyVaultModule))]
    public class EafKeyVaultAspNetCoreModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafKeyVaultAspNetCoreModule).GetAssembly());
        }
    }
}