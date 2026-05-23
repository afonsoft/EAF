using Abp.Application.Services.Dto;
using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Timing
{
    /// <summary>
    /// Representa a interface ITimeZoneService.
    /// </summary>
    public interface ITimeZoneService
    {
        TimeZoneInfo FindTimeZoneById(string timezoneId);

        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);

        List<NameValueDto> GetWindowsTimezones();
    }
}