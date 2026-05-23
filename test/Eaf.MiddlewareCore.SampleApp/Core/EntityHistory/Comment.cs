using Abp.Auditing;
using Abp.Domain.Entities;

namespace Eaf.MiddlewareCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class Comment : Entity
    {
        public string Content { get; set; }
        public Post Post { get; set; }
    }
}