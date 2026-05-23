using Abp;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Eaf.KeyVault
{
    [DependsOn(typeof(AbpKernelModule))]
    public class EafKeyVaultModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafKeyVaultModule).GetAssembly());
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IKeyVaultSecretManager, KeyVaultSecretManager>();
        }
    }
}