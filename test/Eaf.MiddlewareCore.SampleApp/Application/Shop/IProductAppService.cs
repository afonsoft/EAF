using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace Eaf.MiddlewareCore.SampleApp.Application.Shop
{
    public interface IProductAppService : IApplicationService
    {
        Task CreateProduct(ProductCreateDto input);

        Task<ListResultDto<ProductListDto>> GetProducts();

        Task Translate(int productId, ProductTranslationDto input);

        Task UpdateProduct(ProductUpdateDto input);
    }
}