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

        private static readonly Regex _emailRegex = new(EmailRegex, RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

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

            return _emailRegex.IsMatch(value);
        }
    }
}