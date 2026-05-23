using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Auditing
{
    /// <summary>
    /// Representa a interface IAuditLogAppService.
    /// </summary>
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input);

        Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input);

        Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input);

        Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input);

        List<NameValueDto> GetEntityHistoryObjectTypes();

        Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(long entityChangeId);

        Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input);
    }
}