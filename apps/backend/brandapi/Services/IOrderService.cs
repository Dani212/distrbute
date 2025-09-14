using BrandApi.Models;

namespace BrandApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(int id, Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByBrandIdAsync(int brandId);
    }
}
