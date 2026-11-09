using Abp.Dapper;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Eaf.Dapper
{
    /// <summary>
    /// Módulo Eaf.Dapper — consultas SQL raw sobre o DbContext do ABP via Dapper.
    /// </summary>
    [DependsOn(typeof(AbpDapperModule), typeof(AbpEntityFrameworkCoreModule))]
    public class EafDapperModule : AbpModule
    {
        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafDapperModule).GetAssembly());
        }
    }
}
