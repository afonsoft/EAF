using Abp.Dependency;
using Eaf.Middleware.Contracts;
using System.Threading.Tasks;

namespace Eaf.Middleware.Moderation
{
    /// <summary>
    /// Default no-op implementation of <see cref="IModerationAuditWriter"/>.
    /// Hosts may replace this with a persistent writer.
    /// </summary>
    public class NullModerationAuditWriter : IModerationAuditWriter, ITransientDependency
    {
        public Task WriteAsync(ModerationAuditContract entry)
        {
            return Task.CompletedTask;
        }
    }
}
