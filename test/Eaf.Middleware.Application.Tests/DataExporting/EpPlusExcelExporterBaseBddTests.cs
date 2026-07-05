using Abp;
using Abp.Runtime.Caching;
using Eaf.Middleware.DataExporting.Excel.EpPlus;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.DataExporting
{
    /// <summary>
    /// Testes BDD para EpPlusExcelExporterBase seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EpPlusExcelExporterBaseBddTests
    {
        private sealed class TestableExcelExporter : EpPlusExcelExporterBase
        {
            public TestableExcelExporter(ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager)
            {
            }
        }

        [Fact]
        public void Dado_TipoEpPlusExcelExporterBase_Quando_Verificar_Entao_DeveSerAbstrato()
        {
            typeof(EpPlusExcelExporterBase).IsAbstract.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TempFileCacheManager_Quando_Criar_Entao_DeveHerdarAbpServiceBase()
        {
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();

            var sut = new TestableExcelExporter(tempFileCacheManager);

            sut.ShouldBeAssignableTo<AbpServiceBase>();
        }
    }
}
