using Abp.Domain.Entities;

namespace Eaf.MiddlewareCore.SampleApp.Core.Shop
{
    public class ProductTranslation : Entity, IEntityTranslation<Product>
    {
        public virtual Product Core { get; set; }
        public virtual int CoreId { get; set; }
        public virtual string Language { get; set; }
        public virtual string Name { get; set; }
    }
}