using Newtonsoft.Json.Linq;
using System;

namespace Eaf.Middleware.Core.Authentication.External.Google
{
    /// <summary>
    /// Representa a classe GoogleHelper.
    /// </summary>
    public static class GoogleHelper
    {
        /// <summary>
        /// GetEmail.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetEmail(JObject user) => user != null ? user.Value<string>((object)"email") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetFamilyName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetFamilyName(JObject user) => user != null ? user.Value<string>((object)"family_name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetGivenName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetGivenName(JObject user) => user != null ? user.Value<string>((object)"given_name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetId.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetId(JObject user) => user != null ? user.Value<string>((object)"id") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetName.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetName(JObject user) => user != null ? user.Value<string>((object)"name") : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// GetProfile.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        public static string GetProfile(JObject user) => user != null ? user.Value<string>((object)"link") : throw new ArgumentNullException(nameof(user));
    }
}