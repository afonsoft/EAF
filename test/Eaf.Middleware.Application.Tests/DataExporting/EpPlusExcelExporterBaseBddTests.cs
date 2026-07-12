using Abp;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Caching;
using Eaf.Middleware.DataExporting.Excel.EpPlus;
using Eaf.Middleware.Storage;
using NSubstitute;
using OfficeOpenXml;
using Shouldly;
using System.Globalization;
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

            public string PublicL(string name, params object[] args) => L(name, args);
            public string PublicL(string name, CultureInfo culture) => L(name, culture);
            public void PublicAddHeader(ExcelWorksheet sheet, params string[] headerTexts) => AddHeader(sheet, headerTexts);
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

        [Fact]
        public void Dado_LocalizationManagerComArgs_Quando_L_Entao_DeveRetornarTextoLocalizado()
        {
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            var sut = new TestableExcelExporter(tempFileCacheManager);
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Key", Arg.Any<CultureInfo>()).Returns("Hello {0}");
            localizationManager.GetSource("EafCore").Returns(source);
            sut.LocalizationManager = localizationManager;

            var result = sut.PublicL("Key", "World");

            result.ShouldBe("Hello World");
        }

        [Fact]
        public void Dado_LocalizationManagerComCulture_Quando_L_Entao_DeveRetornarTextoLocalizado()
        {
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            var sut = new TestableExcelExporter(tempFileCacheManager);
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Key", CultureInfo.InvariantCulture).Returns("Localized");
            localizationManager.GetSource("EafCore").Returns(source);
            sut.LocalizationManager = localizationManager;

            var result = sut.PublicL("Key", CultureInfo.InvariantCulture);

            result.ShouldBe("Localized");
        }

        [Fact]
        public void Dado_HeadersVazio_Quando_AddHeader_Entao_DeveRetornarSemAlterarPlanilha()
        {
            ExcelPackage.License.SetNonCommercialOrganization("EAF");
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            var sut = new TestableExcelExporter(tempFileCacheManager);

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Test");

            Should.NotThrow(() => sut.PublicAddHeader(sheet));
        }
    }
}
