using Abp.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Eaf.Middleware.Validation
{
    /// <summary>
    /// Representa a classe ValidationHelper.
    /// </summary>
    public static class ValidationHelper
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        /// IsEmail.
        /// </summary>
        /// <param name="value">Parâmetro value.</param>
        /// <returns>Resultado da operação.</returns>
        public static bool IsEmail(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            var regex = new Regex(EmailRegex, RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            return regex.IsMatch(value);
        }
    }
}