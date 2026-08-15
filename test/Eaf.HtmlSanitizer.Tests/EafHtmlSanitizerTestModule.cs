using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Eaf.HtmlSanitizer.Tests
{
    /// <summary>
    /// Módulo de teste para Eaf.HtmlSanitizer.
    /// </summary>
    [DependsOn(typeof(EafHtmlSanitizerModule))]
    public class EafHtmlSanitizerTestModule : AbpModule
    {
        /// <summary>
        /// Inicializa o módulo de teste registrando tipos por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafHtmlSanitizerTestModule).GetAssembly());
        }
    }
}
