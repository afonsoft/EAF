using Abp.EntityHistory;
using Eaf.Middleware.Authorization.Users;

namespace Eaf.Middleware.Auditing
{
    /// <summary>
    /// A helper class to store an <see cref="EntityChange"/> and a <see cref="User"/> object.
    /// </summary>
    public class EntityChangeAndUser
    {
        /// <summary>
        /// Obtém ou define EntityChange.
        /// </summary>
        public EntityChange EntityChange { get; set; }

        /// <summary>
        /// Obtém ou define User.
        /// </summary>
        public User User { get; set; }
    }
}