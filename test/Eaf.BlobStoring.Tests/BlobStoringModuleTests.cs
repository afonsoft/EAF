using System.Linq;
using Abp.BlobStoring;
using Abp.Modules;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes de inicialização do módulo Eaf.BlobStoring.
    /// </summary>
    public class BlobStoringModuleTests : BlobStoringTestBase
    {
        /// <summary>
        /// Dado o módulo, quando verificar dependências, então deve depender do AbpBlobStoringModule.
        /// </summary>
        [Fact]
        public void Dado_Modulo_Quando_VerificarDependencias_Entao_DeveDependerDeAbpBlobStoring()
        {
            var dependsOn = typeof(EafBlobStoringModule)
                .GetCustomAttributes(typeof(DependsOnAttribute), false)
                .Cast<DependsOnAttribute>()
                .FirstOrDefault();

            dependsOn.ShouldNotBeNull();
            dependsOn.DependedModuleTypes.ShouldContain(typeof(AbpBlobStoringModule));
        }

        /// <summary>
        /// Dado o módulo inicializado, quando resolver IBlobContainer, então deve retornar uma instância.
        /// </summary>
        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverIBlobContainer_Entao_Deve_RetornarInstancia()
        {
            var container = Resolve<IBlobContainer>();

            container.ShouldNotBeNull();
        }
    }
}
