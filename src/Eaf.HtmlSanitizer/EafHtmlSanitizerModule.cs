using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Eaf.HtmlSanitizer.Html;
using Microsoft.Extensions.Options;

namespace Eaf.HtmlSanitizer
{
    /// <summary>
    /// Módulo EAF que fornece sanitização de HTML segura contra XSS.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafHtmlSanitizerModule : AbpModule
    {
        /// <summary>
        /// Pré-inicializa o módulo registrando opções padrão.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafHtmlSanitizerOptions>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafHtmlSanitizerOptions, IOptions<EafHtmlSanitizerOptions>>()
                             .ImplementedBy<EafHtmlSanitizerOptions>()
                             .LifestyleSingleton()
                );
            }
        }

        /// <summary>
        /// Inicializa o módulo registrando o serviço <see cref="IHtmlSanitizer"/>.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafHtmlSanitizerModule).GetAssembly());
        }
    }
}
