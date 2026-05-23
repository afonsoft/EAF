using Eaf.Middleware.Authorization.Users;
using System.Security.Claims;

namespace Eaf.Middleware.Authorization.Impersonation
{
    /// <summary>
    /// Representa a classe UserAndIdentity.
    /// </summary>
    public class UserAndIdentity
    {
        /// <summary>
        /// UserAndIdentity.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="identity">Parâmetro identity.</param>
        /// <returns>Resultado da operação.</returns>
        public UserAndIdentity(User user, ClaimsIdentity identity)
        {
            User = user;
            Identity = identity;
        }

        /// <summary>
        /// Obtém ou define Identity.
        /// </summary>
        public ClaimsIdentity Identity { get; set; }
        /// <summary>
        /// Obtém ou define User.
        /// </summary>
        public User User { get; set; }
    }
}