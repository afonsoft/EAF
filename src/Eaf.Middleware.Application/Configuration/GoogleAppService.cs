using Abp.Authorization;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    [AbpAllowAnonymous]
    public class GoogleAppService : MiddlewareAppServiceBase, IGoogleAppService
    {
        /// <summary>
        /// GetAnalytics.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> GetAnalytics()
        {
            return await SettingManager.GetSettingValueAsync(EafMiddlewareSettingNames.Google.Analytics);
        }

        /// <summary>
        /// GetRecaptchaSiteKey.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> GetRecaptchaSiteKey()
        {
            return await SettingManager.GetSettingValueAsync(EafMiddlewareSettingNames.Google.RecaptchaSiteKey);
        }

        /// <summary>
        /// GetTagManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> GetTagManager()
        {
            return await SettingManager.GetSettingValueAsync(EafMiddlewareSettingNames.Google.TagManager);
        }
    }
}