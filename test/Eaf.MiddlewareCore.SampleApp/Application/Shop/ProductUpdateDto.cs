using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Eaf.MiddlewareCore.SampleApp.Application.Shop
{
    public class ProductUpdateDto : EntityDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductTranslationDto> Translations { get; set; }
    }
}