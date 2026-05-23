using Abp.Domain.Entities;
using System.Collections.Generic;

namespace Eaf.MiddlewareCore.SampleApp.Core.Shop
{
    public class Order : Entity, IMultiLingualEntity<OrderTranslation>
    {
        public Order()
        {
            Products = new List<Product>();
        }

        public virtual decimal Price { get; set; }

        public List<Product> Products { get; set; }
        public ICollection<OrderTranslation> Translations { get; set; }
    }
}