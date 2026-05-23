using Abp.Domain.Entities;
using System.Collections.Generic;

namespace Eaf.MiddlewareCore.SampleApp.Core.Shop
{
    public class Product : Entity, IMultiLingualEntity<ProductTranslation>
    {
        public virtual decimal Price { get; set; }

        public virtual int Stock { get; set; }

        public virtual ICollection<ProductTranslation> Translations { get; set; }
    }
}