using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Auditing.Exporting
{
    /// <summary>
    /// Representa a interface IAuditLogListExcelExporter.
    /// </summary>
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}