using Microsoft.EntityFrameworkCore;
using DistributorApi.Data;
using DistributorApi.Models;

namespace DistributorApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly DistributorDbContext _context;

        public OrderService(DistributorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Distributor)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Distributor)
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

        public async Task<IEnumerable<Order>> GetOrdersByDistributorIdAsync(int distributorId)
        {
            return await _context.Orders
                .Include(o => o.Distributor)
                .Where(o => o.DistributorId == distributorId)
                .ToListAsync();
        }
    }
}
