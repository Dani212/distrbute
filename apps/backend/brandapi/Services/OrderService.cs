using Microsoft.EntityFrameworkCore;
using BrandApi.Data;
using BrandApi.Models;

namespace BrandApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly BrandDbContext _context;

        public OrderService(BrandDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Brand)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Brand)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(int id, Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return null;

            existingOrder.DistributorId = order.DistributorId;
            existingOrder.Items = order.Items;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Currency = order.Currency;
            existingOrder.Status = order.Status;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.BillingAddress = order.BillingAddress;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetOrdersByBrandIdAsync(int brandId)
        {
            return await _context.Orders
                .Include(o => o.Brand)
                .Where(o => o.BrandId == brandId)
                .ToListAsync();
        }
    }
}
