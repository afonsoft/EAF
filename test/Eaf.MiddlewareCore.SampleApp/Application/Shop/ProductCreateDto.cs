using System.Collections.Generic;

namespace Eaf.MiddlewareCore.SampleApp.Application.Shop
{
    public class ProductCreateDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductTranslationDto> Translations { get; set; }
    }
}