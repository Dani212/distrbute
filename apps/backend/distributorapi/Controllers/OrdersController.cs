using Microsoft.AspNetCore.Mvc;
using DistributorApi.Models;
using DistributorApi.Services;

namespace DistributorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            var createdOrder = await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
            if (updatedOrder == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByDistributor(int distributorId)
        {
            var orders = await _orderService.GetOrdersByDistributorIdAsync(distributorId);
            return Ok(orders);
        }
    }
}
