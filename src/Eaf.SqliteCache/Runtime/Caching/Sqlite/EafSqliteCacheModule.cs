using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using System;

namespace Abp.Runtime.Caching.Sqlite
{
    /// <summary>
    /// This modules is used to replace eaf's cache system with Sqlite server.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafSqliteCacheModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafSqliteCacheModule).GetAssembly());
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.Register<EafSqliteCacheOptions>();
        }
    }
}