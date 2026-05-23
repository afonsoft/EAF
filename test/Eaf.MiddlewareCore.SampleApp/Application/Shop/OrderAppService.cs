using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Eaf.MiddlewareCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.MiddlewareCore.SampleApp.Application.Shop
{
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderAppService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ListResultDto<OrderListDto>> GetOrders()
        {
            var products = await _orderRepository.GetAll()
                .Include(p => p.Translations)
                .Include(p => p.Products)
                .ToListAsync();

            return new ListResultDto<OrderListDto>(ObjectMapper.Map<List<OrderListDto>>(products));
        }
    }
}