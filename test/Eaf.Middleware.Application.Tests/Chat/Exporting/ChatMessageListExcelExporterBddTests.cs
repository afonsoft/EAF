using Abp;
using Abp.Timing.Timezone;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Chat.Exporting;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat.Exporting
{
    public class ChatMessageListExcelExporterBddTests
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly ChatMessageListExcelExporter _sut;

        public ChatMessageListExcelExporterBddTests()
        {
            _timeZoneConverter = Substitute.For<ITimeZoneConverter>();
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();

            _sut = new ChatMessageListExcelExporter(_timeZoneConverter, _tempFileCacheManager);
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            // Dado / Quando
            var sut = new ChatMessageListExcelExporter(_timeZoneConverter, _tempFileCacheManager);

            // Então
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ListaVaziaDeMensagens_Quando_Exportar_Entao_DeveCriarArquivoComNomeAnonimoEPersistirNoCache()
        {
            // Dado
            var user = new UserIdentifier(1, 42);
            _timeZoneConverter
                .Convert(Arg.Any<DateTime>(), Arg.Any<int?>(), Arg.Any<long>())
                .Returns(DateTime.UtcNow);

            // Quando
            var result = _sut.ExportToFile(user, new List<ChatMessageExportDto>());

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("Chat_Anonymous_Anonymous.xlsx");
            _tempFileCacheManager.Received(1).SetFile(result.FileToken, Arg.Is<byte[]>(b => b.Length > 0));
        }

        [Fact]
        public void Dado_MensagensEntreUsuarios_Quando_Exportar_Entao_DeveUsarTimeZoneConverterEPersistirNoCache()
        {
            // Dado
            var user = new UserIdentifier(1, 42);
            var creationTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            _timeZoneConverter
                .Convert(Arg.Any<DateTime>(), Arg.Any<int?>(), Arg.Any<long>())
                .Returns(DateTime.UtcNow);

            var messages = new List<ChatMessageExportDto>
            {
                new ChatMessageExportDto
                {
                    CreationTime = creationTime,
                    Message = "Hello",
                    ReadState = ChatMessageReadState.Read,
                    ReceiverReadState = ChatMessageReadState.Unread,
                    Side = ChatSide.Sender,
                    TargetTenantId = 1,
                    TargetTenantName = "Default",
                    TargetUserId = 43,
                    TargetUserName = "mary"
                },
                new ChatMessageExportDto
                {
                    CreationTime = creationTime,
                    Message = "Reply",
                    ReadState = ChatMessageReadState.Read,
                    ReceiverReadState = ChatMessageReadState.Read,
                    Side = ChatSide.Receiver,
                    TargetTenantId = 1,
                    TargetTenantName = "Default",
                    TargetUserId = 43,
                    TargetUserName = "mary"
                }
            };

            // Quando
            var result = _sut.ExportToFile(user, messages);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("Chat_Default_mary.xlsx");
            _tempFileCacheManager.Received(1).SetFile(result.FileToken, Arg.Is<byte[]>(b => b.Length > 0));
            _timeZoneConverter.Received(messages.Count).Convert(
                Arg.Is<DateTime>(x => x == creationTime),
                Arg.Is<int?>(x => x == user.TenantId),
                Arg.Is<long>(x => x == user.UserId)
            );
        }
    }
}
