using Abp;
using Abp.Timing.Timezone;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.DataExporting.Excel.EpPlus;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Chat.Exporting
{
    /// <summary>
    /// Representa a classe ChatMessageListExcelExporter.
    /// </summary>
    public class ChatMessageListExcelExporter : EpPlusExcelExporterBase, IChatMessageListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;

        /// <summary>
        /// ChatMessageListExcelExporter.
        /// </summary>
        /// <param name="timeZoneConverter">Parâmetro timeZoneConverter.</param>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatMessageListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            ITempFileCacheManager tempFileCacheManager
            ) : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
        }

        /// <summary>
        /// ExportToFile.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="messages">Parâmetro messages.</param>
        /// <returns>Resultado da operação.</returns>
        public FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages)
        {
            var tenancyName = messages.Count > 0 ? messages[0].TargetTenantName : L("Anonymous");
            var userName = messages.Count > 0 ? messages[0].TargetUserName : L("Anonymous");

            return CreateExcelPackage(
                $"Chat_{tenancyName}_{userName}.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Messages"));

                    AddHeader(
                        sheet,
                        L("ChatMessage_From"),
                        L("ChatMessage_To"),
                        L("Message"),
                        L("ReadState"),
                        L("CreationTime")
                    );

                    AddObjects(
                        sheet, 2, messages,
                        _ => _.Side == ChatSide.Receiver ? (_.TargetTenantName + "/" + _.TargetUserName) : L("You"),
                        _ => _.Side == ChatSide.Receiver ? L("You") : (_.TargetTenantName + "/" + _.TargetUserName),
                        _ => _.Message,
                        _ => _.Side == ChatSide.Receiver ? _.ReadState : _.ReceiverReadState,
                        _ => _timeZoneConverter.Convert(_.CreationTime, user.TenantId, user.UserId)
                    );
                });
        }
    }
}