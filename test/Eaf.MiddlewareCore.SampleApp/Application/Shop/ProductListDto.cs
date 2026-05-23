using Abp.Application.Services.Dto;

namespace Eaf.MiddlewareCore.SampleApp.Application.Shop
{
    public class OrderListDto : EntityDto
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ProductCount { get; set; }
    }

    public class ProductListDto : EntityDto
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}