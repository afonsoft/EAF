using Newtonsoft.Json.Linq;
using System;

namespace Eaf.Middleware.Authorization.External.AuthZero
{
    /// <summary>
    /// Representa a classe AuthZeroAccountHelper.
    /// </summary>
    public static class AuthZeroAccountHelper
    {
        /// <summary>
        /// GetDisplayName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetDisplayName(JObject user) => user != null ? user.Value<string>((object)"name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetEmail.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <returns>Resultado da operação.</returns>
        public static string GetEmail(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return user.Value<string>((object)"email") ?? "";
        }

        /// <summary>
        /// GetGivenName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetGivenName(JObject user) => user != null ? user.Value<string>((object)"given_name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetId.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetId(JObject user) => user != null ? user.Value<string>((object)"sub") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetSurname.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetSurname(JObject user) => user != null ? user.Value<string>((object)"family_name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetPicture.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetPicture(JObject user) => user != null ? user.Value<string>((object)"picture") : throw new ArgumentNullException(nameof(user));
    }
}