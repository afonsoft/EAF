using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe EafMiddlewareSettingNames.
    /// </summary>
    public static class EafMiddlewareSettingNames
    {
        /// <summary>
        /// Representa a classe Google.
        /// </summary>
        public static class Google
        {
            /// <summary>
            /// "Eaf.Middleware.Google.Analytics".
            /// </summary>
            public const string Analytics = "Eaf.Middleware.Google.Analytics";

            /// <summary>
            /// Eaf.Middleware.Google.RecaptchaSiteKey
            /// </summary>
            public const string RecaptchaSiteKey = "Eaf.Middleware.Google.RecaptchaSiteKey";

            /// <summary>
            /// "Eaf.Middleware.Google.TagManager".
            /// </summary>
            public const string TagManager = "Eaf.Middleware.Google.TagManager";
        }

        /// <summary>
        /// Representa a classe UserManagement.
        /// </summary>
        public static class UserManagement
        {
            /// <summary>
            /// "Eaf.Middleware.UserManagement.IsRegisterRequiredForLogin".
            /// </summary>
            public const string IsRegisterRequiredForLogin = "Eaf.Middleware.UserManagement.IsRegisterRequiredForLogin";
        }

        /// <summary>
        /// Representa a classe LogDeleter.
        /// </summary>
        public static class LogDeleter
        {
            /// <summary>
            /// "Eaf.ExpiredEntity.LogDeleter.IsEnabled".
            /// </summary>
            public const string IsEnabled = "Eaf.ExpiredEntity.LogDeleter.IsEnabled";

            /// <summary>
            /// "Eaf.ExpiredEntity.LogDeleter.DeletedQuantity".
            /// </summary>
            public const string DeletedQuantity = "Eaf.ExpiredEntity.LogDeleter.DeletedQuantity";

            /// <summary>
            /// "Eaf.ExpiredEntity.LogDeleter.ExpiredDays".
            /// </summary>
            public const string ExpiredDays = "Eaf.ExpiredEntity.LogDeleter.ExpiredDays";
        }

        /// <summary>
        /// Representa a classe LoginImpersonator.
        /// </summary>
        public static class LoginImpersonator
        {
            /// <summary>
            /// "Eaf.ExpiredEntity.LoginAdmDB.IsEnabled".
            /// </summary>
            public const string IsEnabled = "Eaf.ExpiredEntity.LoginImpersonator.IsEnabled";
        }
    }
}