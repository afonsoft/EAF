using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Eaf.Middleware.Timing
{
    /// <summary>
    /// Representa a classe TimeZoneService.
    /// </summary>
    public class TimeZoneService : ITimeZoneService, ITransientDependency
    {
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// TimeZoneService.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="settingDefinitionManager">Parâmetro settingDefinitionManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TimeZoneService(
            ISettingManager settingManager,
            ISettingDefinitionManager settingDefinitionManager)
        {
            _settingManager = settingManager;
            _settingDefinitionManager = settingDefinitionManager;
        }

        /// <summary>
        /// FindTimeZoneById.
        /// </summary>
        /// <param name="timezoneId">Parâmetro timezoneId.</param>
        /// <returns>Resultado da operação.</returns>
        public TimeZoneInfo FindTimeZoneById(string timezoneId)
        {
            return TZConvert.GetTimeZoneInfo(timezoneId);
        }

        /// <summary>
        /// GetDefaultTimezoneAsync.
        /// </summary>
        /// <param name="scope">Parâmetro scope.</param>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId)
        {
            if (scope == SettingScopes.User)
            {
                if (tenantId.HasValue)
                {
                    return await _settingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, tenantId.Value);
                }

                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            }

            if (scope == SettingScopes.Tenant)
            {
                return await _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone);
            }

            if (scope == SettingScopes.Application)
            {
                var timezoneSettingDefinition = _settingDefinitionManager.GetSettingDefinition(TimingSettingNames.TimeZone);
                return timezoneSettingDefinition.DefaultValue;
            }

            throw new Exception("Unknown scope for default timezone setting.");
        }

        /// <summary>
        /// GetWindowsTimezones.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public List<NameValueDto> GetWindowsTimezones()
        {
            return TZConvert.KnownWindowsTimeZoneIds.OrderBy(tz => tz)
                .Select(tz => new NameValueDto
                {
                    Value = tz,
                    Name = tz
                }).ToList();
        }
    }
}