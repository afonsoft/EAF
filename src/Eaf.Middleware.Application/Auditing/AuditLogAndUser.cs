using Abp.Auditing;
using Eaf.Middleware.Authorization.Users;

namespace Eaf.Middleware.Auditing
{
    /// <summary>
    /// A helper class to store an <see cref="AuditLog"/> and a <see cref="User"/> object.
    /// </summary>
    public class AuditLogAndUser
    {
        /// <summary>
        /// Obtém ou define AuditLog.
        /// </summary>
        public AuditLog AuditLog { get; set; }

        /// <summary>
        /// Obtém ou define User.
        /// </summary>
        public User User { get; set; }
    }
}