using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Timing.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Timing
{
    /// <summary>
    /// Representa a interface ITimingAppService.
    /// </summary>
    public interface ITimingAppService : IApplicationService
    {
        Task<List<ComboboxItemDto>> GetTimezoneComboboxItems(GetTimezoneComboboxItemsInput input);

        Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInput input);
    }
}