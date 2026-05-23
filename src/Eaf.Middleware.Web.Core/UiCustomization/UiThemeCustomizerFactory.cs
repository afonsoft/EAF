using Abp.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.UiCustomization;
using Eaf.Middleware.Web.UiCustomization.Metronic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.UiCustomization
{
    /// <summary>
    /// Representa a classe UiThemeCustomizerFactory.
    /// </summary>
    public class UiThemeCustomizerFactory : IUiThemeCustomizerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// UiThemeCustomizerFactory.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="serviceProvider">Parâmetro serviceProvider.</param>
        /// <returns>Resultado da operação.</returns>
        public UiThemeCustomizerFactory(
            ISettingManager settingManager,
            IServiceProvider serviceProvider
        )
        {
            _settingManager = settingManager;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// GetCurrentUiCustomizer.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<IUiCustomizer> GetCurrentUiCustomizer()
        {
            var theme = await _settingManager.GetSettingValueAsync(AppSettings.UiManagement.Theme);
            return GetUiCustomizerInternal(theme);
        }

        /// <summary>
        /// GetUiCustomizer.
        /// </summary>
        /// <param name="theme">Parâmetro theme.</param>
        /// <returns>Resultado da operação.</returns>
        public IUiCustomizer GetUiCustomizer(string theme)
        {
            return GetUiCustomizerInternal(theme);
        }

        private IUiCustomizer GetUiCustomizerInternal(string theme)
        {
            if (theme.Equals(MiddlewareAppConsts.Theme2, StringComparison.InvariantCultureIgnoreCase))
            {
                return _serviceProvider.GetService<Theme2UiCustomizer>();
            }

            if (theme.Equals(MiddlewareAppConsts.Theme3, StringComparison.InvariantCultureIgnoreCase))
            {
                return _serviceProvider.GetService<Theme3UiCustomizer>();
            }

            if (theme.Equals(MiddlewareAppConsts.Theme4, StringComparison.InvariantCultureIgnoreCase))
            {
                return _serviceProvider.GetService<Theme4UiCustomizer>();
            }

            return _serviceProvider.GetService<ThemeDefaultUiCustomizer>();
        }
    }
}