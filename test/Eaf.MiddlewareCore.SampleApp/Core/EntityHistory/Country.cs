using Abp.Domain.Entities.Auditing;

namespace Eaf.MiddlewareCore.SampleApp.Core.EntityHistory
{
    public class Country : FullAuditedEntity
    {
        public string CountryCode { get; set; }
    }
}