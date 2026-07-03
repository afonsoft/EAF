using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Eaf.Middleware.Authorization.Users.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Exporting
{
    public class UserListExcelExporterBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            var session = Substitute.For<IAbpSession>();
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();

            var sut = new UserListExcelExporter(timeZoneConverter, session, tempFileCacheManager);
            sut.ShouldNotBeNull();
        }
    }
}
