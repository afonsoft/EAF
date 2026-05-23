using Abp;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Chat.Exporting
{
    /// <summary>
    /// Representa a interface IChatMessageListExcelExporter.
    /// </summary>
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}