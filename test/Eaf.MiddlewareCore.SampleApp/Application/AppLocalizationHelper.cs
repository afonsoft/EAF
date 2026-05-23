using Abp.Localization;

namespace Eaf.MiddlewareCore.SampleApp.Application
{
    public static class AppLocalizationHelper
    {
        public static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AppConsts.LocalizationSourceName);
        }
    }
}