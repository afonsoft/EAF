using Newtonsoft.Json.Linq;
using System;

namespace Eaf.Middleware.Core.Authentication.External.Microsoft
{
    /// <summary>
    /// Representa a classe MicrosoftAccountHelper.
    /// </summary>
    public static class MicrosoftAccountHelper
    {
        /// <summary>
        /// GetDisplayName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetDisplayName(JObject user) => user != null ? user.Value<string>((object)"displayName") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetEmail.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <returns>Resultado da operação.</returns>
        public static string GetEmail(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return user.Value<string>((object)"mail") ?? user.Value<string>((object)"userPrincipalName");
        }

        /// <summary>
        /// GetGivenName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetGivenName(JObject user) => user != null ? user.Value<string>((object)"givenName") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetId.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetId(JObject user) => user != null ? user.Value<string>((object)"id") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetSurname.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetSurname(JObject user) => user != null ? user.Value<string>((object)"surname") : throw new ArgumentNullException(nameof(user));
    }
}