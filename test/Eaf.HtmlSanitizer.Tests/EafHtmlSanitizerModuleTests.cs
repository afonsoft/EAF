using Abp;
using Abp.Dependency;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Eaf.HtmlSanitizer.Tests
{
    public class EafHtmlSanitizerModuleTests
    {
        [Fact]
        public void Dado_EafHtmlSanitizerModule_Quando_Inicializar_Entao_Servicos_E_Opcoes_Sao_Registrados()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var container = bootstrapper.IocManager;
            container.Resolve<IHtmlSanitizer>().ShouldNotBeNull();
            container.Resolve<EafHtmlSanitizerOptions>().ShouldNotBeNull();
            container.Resolve<IOptions<EafHtmlSanitizerOptions>>().ShouldNotBeNull();
        }

        private static AbpBootstrapper CriarBootstrapper()
        {
            return AbpBootstrapper.Create<EafHtmlSanitizerTestModule>(options =>
            {
                options.IocManager = new IocManager();
            });
        }
    }
}
