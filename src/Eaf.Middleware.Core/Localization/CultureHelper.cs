using System;
using System.Globalization;
using System.Linq;

namespace Eaf.Middleware.Localization
{
    /// <summary>
    /// Representa a classe CultureHelper.
    /// </summary>
    public static class CultureHelper
    {
        /// <summary>
        /// GetCultures.
        /// </summary>
        public static CultureInfo[] AllCultures { get; } = CultureInfo.GetCultures(CultureTypes.AllCultures);

        public static bool UsingLunarCalendar { get; } = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.AlgorithmType == CalendarAlgorithmType.LunarCalendar;
        public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

        /// <summary>
        /// GetCultureInfoByChecking.
        /// </summary>
        /// <param name="name">Parâmetro name.</param>
        /// <returns>Resultado da operação.</returns>
        public static CultureInfo GetCultureInfoByChecking(string name)
        {
            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(name);

                if (!AllCultures.Any(c => c.Name.Equals(cultureInfo.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return CultureInfo.CurrentCulture;
                }

                return cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.CurrentCulture;
            }
        }
    }
}