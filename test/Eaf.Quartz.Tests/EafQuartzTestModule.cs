using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Eaf.Quartz.Tests
{
    /// <summary>
    /// Módulo de teste para Eaf.Quartz.
    /// </summary>
    [DependsOn(typeof(EafQuartzModule))]
    public class EafQuartzTestModule : AbpModule
    {
        /// <summary>
        /// Inicializa o módulo de teste registrando tipos por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafQuartzTestModule).GetAssembly());
        }
    }
}
