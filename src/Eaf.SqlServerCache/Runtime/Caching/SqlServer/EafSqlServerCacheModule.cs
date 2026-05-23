using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Caching.SqlServer;

namespace Eaf.Runtime.Caching.SqlServer
{
    /// <summary>
    /// This modules is used to replace eaf's cache system with Sqlite server.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafSqlServerCacheModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafSqlServerCacheModule).GetAssembly());
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.Register<SqlServerCacheOptions>();
        }

        /// <summary>
        /// PostInitialize.
        /// </summary>
        public override void PostInitialize()
        {
        }
    }
}