using RedMango_API.Models.Dto;
using RedMango_API.Models;

namespace EcomApi.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderHeader>> GetOrdersAsync(string userId, string searchString, string status, int pageNumber, int pageSize);
        Task<OrderHeader> GetOrderByIdAsync(int id);
        Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO orderHeaderDTO);
        Task<bool> UpdateOrderHeaderAsync(int id, OrderHeaderUpdateDTO orderHeaderUpdateDTO);
    
}
}
