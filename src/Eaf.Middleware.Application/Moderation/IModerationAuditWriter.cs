using Eaf.Middleware.Contracts;
using System.Threading.Tasks;

namespace Eaf.Middleware.Moderation
{
    /// <summary>
    /// Writes moderation audit entries for cross-cutting compliance and forensics.
    /// </summary>
    public interface IModerationAuditWriter
    {
        /// <summary>
        /// Writes an audit entry asynchronously.
        /// </summary>
        Task WriteAsync(ModerationAuditContract entry);
    }
}
