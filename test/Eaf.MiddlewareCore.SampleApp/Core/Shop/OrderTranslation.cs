using Abp.Domain.Entities;

namespace Eaf.MiddlewareCore.SampleApp.Core.Shop
{
    public class OrderTranslation : Entity, IEntityTranslation<Order>
    {
        public virtual Order Core { get; set; }
        public virtual int CoreId { get; set; }
        public virtual string Language { get; set; }
        public virtual string Name { get; set; }
    }
}