using Abp.Timing.Timezone;
using Eaf.Middleware.Chat.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat.Exporting
{
    public class ChatMessageListExcelExporterBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();

            var sut = new ChatMessageListExcelExporter(timeZoneConverter, tempFileCacheManager);
            sut.ShouldNotBeNull();
        }
    }
}
