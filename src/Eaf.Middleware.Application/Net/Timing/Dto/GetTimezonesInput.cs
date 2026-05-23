using Abp.Configuration;

namespace Eaf.Middleware.Timing.Dto
{
    /// <summary>
    /// Representa a classe GetTimezonesInput.
    /// </summary>
    public class GetTimezonesInput
    {
        /// <summary>
        /// Obtém ou define DefaultTimezoneScope.
        /// </summary>
        public SettingScopes DefaultTimezoneScope { get; set; } = SettingScopes.Application;
    }
}