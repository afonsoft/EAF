using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Auditing.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing.Exporting
{
    public class AuditLogListExcelExporterBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            var session = Substitute.For<IAbpSession>();
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();

            var sut = new AuditLogListExcelExporter(timeZoneConverter, session, tempFileCacheManager);
            sut.ShouldNotBeNull();
        }
    }
}
